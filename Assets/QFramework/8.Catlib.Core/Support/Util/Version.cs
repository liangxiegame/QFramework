/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using System;
using System.Text.RegularExpressions;

namespace CatLib
{
    /// <summary>
    /// 版本（遵循semver）
    /// </summary>
    public class Version
    {
        /// <summary>
        /// 版本匹配正则式
        /// </summary>
        private static Regex versionMatcher;

        /// <summary>
        /// 版本匹配正则式
        /// </summary>
        private static Regex VersionMatcher
        {
            get
            {
                if (versionMatcher == null)
                {
                    versionMatcher = new Regex(
                        @"^(?<major>((?![0])\d+?|[0]))\.(?<minor>((?![0])\d+?|[0]))\.(?<revised>((?![0])\d+?|[0]))(?:-(?!\.)(?<pre_release>([a-zA-Z]\w*?|(?![0])\d+?|[0])(\.([a-zA-Z]\w*?|(?![0])\d+?|[0]))*?))?(?:\+(?!\.)(?<build_metadata>([a-zA-Z]\w*?|(?![0])\d+?|[0])(\.([a-zA-Z]\w*?|(?![0])\d+?|[0]))*?))?$");
                }
                return versionMatcher;
            }
        }

        /// <summary>
        /// 原始版本信息
        /// </summary>
        private readonly string version;

        /// <summary>
        /// 版本信息
        /// </summary>
        private VersionData current;

        /// <summary>
        /// 版本信息
        /// </summary>
        private class VersionData
        {
            /// <summary>
            /// 主版本号
            /// </summary>
            public readonly int Major;

            /// <summary>
            /// 次版本号
            /// </summary>
            public readonly int Minor;

            /// <summary>
            /// 修订版本号
            /// </summary>
            public readonly int Revised;

            /// <summary>
            /// 先行版本号
            /// </summary>
            public readonly string PreRelease;

            /// <summary>
            /// 版本编译信息
            /// </summary>
            public readonly string BuildMetadata;

            /// <summary>
            /// 版本信息
            /// </summary>
            /// <param name="version"></param>
            public VersionData(string version)
            {
                var match = VersionMatcher.Match(version);
                Major = int.Parse(match.Groups["major"].ToString());
                Minor = int.Parse(match.Groups["minor"].ToString());
                Revised = int.Parse(match.Groups["revised"].ToString());
                PreRelease = match.Groups["pre_release"].ToString();
                BuildMetadata = match.Groups["build_metadata"].ToString();
            }
        }

        /// <summary>
        /// 构造一个版本
        /// </summary>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <param name="revised">修订版本号</param>
        public Version(int major, int minor, int revised)
            : this(major + "." + minor + "." + revised)
        {
        }

        /// <summary>
        /// 构造一个版本
        /// </summary>
        /// <param name="version">版本号</param>
        public Version(string version)
        {
            this.version = version;
        }

        /// <summary>
        /// 将当前版本和输入版本进行比较
        /// <para>输入版本大于当前版本则返回<code>-1</code></para>
        /// <para>输入版本等于当前版本则返回<code>0</code></para>
        /// <para>输入版本小于当前版本则返回<code>1</code></para>
        /// </summary>
        /// <param name="version">输入版本</param>
        /// <returns>比较结果</returns>
        public int Compare(string version)
        {
            if (current == null)
            {
                GuardVersion(this.version);
                current = new VersionData(this.version);
            }

            GuardVersion(version);

            var compared = new VersionData(version);

            // 第一个差异值用来决定优先层级：主版本号、次版本号及修订号以数值比较
            // 例如：1.0.0 < 2.0.0 < 2.1.0 < 2.1.1
            if (current.Major != compared.Major)
            {
                return current.Major < compared.Major ? -1 : 1;
            }

            if (current.Minor != compared.Minor)
            {
                return current.Minor < compared.Minor ? -1 : 1;
            }

            if (current.Revised != compared.Revised)
            {
                return current.Revised < compared.Revised ? -1 : 1;
            }

            // 当主版本号、次版本号及修订号都相同时，先行版低于正常的版本
            // 例如：1.0.0-alpha < 1.0.0
            if ((string.IsNullOrEmpty(current.PreRelease) && !string.IsNullOrEmpty(compared.PreRelease))
                || (!string.IsNullOrEmpty(current.PreRelease) && string.IsNullOrEmpty(compared.PreRelease)))
            {
                return string.IsNullOrEmpty(compared.PreRelease) ? -1 : 1;
            }

            // 当先行版版本完全相等时则认为相等
            if (current.PreRelease == compared.PreRelease)
            {
                return 0;
            }

            // 有相同主版本号、次版本号及修订号的两个先行版本号，
            // 其优先层级“必须 MUST ”透过由左到右的每个被句点分隔的标识符号来比较，
            // 直到找到一个差异值后决定：只有数字的标识符号以数值高低比较，有字母或连
            // 接号时则逐字以 ASCII 的排序来比较。数字的标识符号比非数字的标识符号优先
            // 层级低。若开头的标识符号都相同时，栏位比较多的先行版本号优先层级比较高。
            // 例如：1.0.0-alpha < 1.0.0-alpha.1 < 1.0.0-alpha.beta < 1.0.0-beta < 1.0.0-beta.2 < 1.0.0-beta.11 < 1.0.0-rc.1 < 1.0.0

            var currentBlocks = current.PreRelease.Split('.');
            var comparedBlocks = compared.PreRelease.Split('.');

            var length = Math.Max(currentBlocks.Length, comparedBlocks.Length);

            for (var index = 0; index < length; index++)
            {
                if (index >= currentBlocks.Length)
                {
                    return -1;
                }
                if (index >= comparedBlocks.Length)
                {
                    return 1;
                }
                var result = CompareBlock(currentBlocks[index], comparedBlocks[index]);
                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }

        /// <summary>
        /// 比较左值和右值
        /// <para>如果左值大于右值那么返回1</para>
        /// <para>如果左值等于右值那么返回0</para>
        /// <para>如果左值小于右值那么返回-1</para>
        /// </summary>
        /// <param name="left">左值</param>
        /// <param name="right">右值</param>
        private int CompareBlock(string left, string right)
        {
            int leftInt;
            var leftIsInt = int.TryParse(left, out leftInt);
            int rightInt;
            var rightIsInt = int.TryParse(right, out rightInt);

            if (rightIsInt && leftIsInt)
            {
                return leftInt.CompareTo(rightInt);
            }

            if (leftIsInt || rightIsInt)
            {
                return rightIsInt ? 1 : -1;
            }

            return left.CompareTo(right);
        }

        /// <summary>
        /// 验证输入版本
        /// </summary>
        /// <param name="version">输入版本</param>
        private void GuardVersion(string version)
        {
            if (!VersionMatcher.IsMatch(version))
            {
                throw new RuntimeException("version is invalid");
            }
        }

        /// <summary>
        /// 转为字符串
        /// </summary>
        /// <returns>版本信息</returns>
        public override string ToString()
        {
            return version;
        }
    }
}
