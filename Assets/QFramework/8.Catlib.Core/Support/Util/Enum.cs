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

namespace CatLib
{
    /// <summary>
    /// 自定义枚举
    /// </summary>
    public abstract class Enum
    {
        /// <summary>
        /// 名字
        /// </summary>
        private readonly string name;

        /// <summary>
        /// 构造一个枚举
        /// </summary>
        protected Enum(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 转为字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return obj.ToString() == ToString();
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns>哈希吗</returns>
        public override int GetHashCode()
        {
            return (GetType() + "." + ToString()).GetHashCode();
        }

        /// <summary>
        /// 自定义枚举转字符串
        /// </summary>
        /// <param name="e">自定义枚举转字符串</param>
        public static implicit operator string(Enum e)
        {
            return e.ToString();
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="left">左值</param>
        /// <param name="right">右值</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(Enum left, Enum right)
        {
            return !Equals(left, null) && left.Equals(right);
        }

        /// <summary>
        /// 是否不等
        /// </summary>
        /// <param name="left">左值</param>
        /// <param name="right">右值</param>
        /// <returns>是否不等</returns>
        public static bool operator !=(Enum left, Enum right)
        {
            return !(left == right);
        }
    }
}
