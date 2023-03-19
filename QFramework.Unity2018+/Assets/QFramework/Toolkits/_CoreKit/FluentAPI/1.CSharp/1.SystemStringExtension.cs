/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("01.FluentAPI.CSharp", "System.String", 1)]
    [APIDescriptionCN("针对 System.String 提供的链式扩展，理论上任何集合都可以使用")]
    [APIDescriptionEN("The chain extension provided by System.Collections can theoretically be used by any collection")]
#endif
    public static class SystemStringExtension
    {
#if UNITY_EDITOR
        // v1 No.18
        [MethodAPI]
        [APIDescriptionCN("检测是否为空或 Empty")]
        [APIDescriptionEN("Check Whether string is null or empty")]
        [APIExampleCode(@"
Debug.Log(string.Empty.IsNullOrEmpty());
// true
        ")]
#endif
        public static bool IsNullOrEmpty(this string selfStr)
        {
            return string.IsNullOrEmpty(selfStr);
        }


#if UNITY_EDITOR
        // v1 No.19
        [MethodAPI]
        [APIDescriptionCN("检测是否为非空且非Empty")]
        [APIDescriptionEN("Checks both not null and non-empty")]
        [APIExampleCode(@"
Debug.Log(""Hello"".IsNotNullAndEmpty());
// true
        ")]
#endif
        public static bool IsNotNullAndEmpty(this string selfStr)
        {
            return !string.IsNullOrEmpty(selfStr);
        }

#if UNITY_EDITOR
        // v1 No.20
        [MethodAPI]
        [APIDescriptionCN("去掉两端空格后，检测是否为空或 Empty")]
        [APIDescriptionEN("Check if it is Empty or Empty after removing whitespace from both sides")]
        [APIExampleCode(@"
Debug.Log(""   "".IsTrimNullOrEmpty());
// true
        ")]
#endif
        public static bool IsTrimNullOrEmpty(this string selfStr)
        {
            return selfStr == null || string.IsNullOrEmpty(selfStr.Trim());
        }

        /// <summary>
        /// Check Whether string trim is null or empty
        /// </summary>
        /// <param name="selfStr"></param>
        /// <returns></returns>
#if UNITY_EDITOR
        // v1 No.21
        [MethodAPI]
        [APIDescriptionCN("去掉两端空格后，检测是否为非 null 且非 Empty")]
        [APIDescriptionEN("After removing whitespace from both sides, check whether it is non-null and non-empty")]
        [APIExampleCode(@"
Debug.Log(""  123  "".IsTrimNotNullAndEmpty());
// true
        ")]
#endif
        public static bool IsTrimNotNullAndEmpty(this string selfStr)
        {
            return selfStr != null && !string.IsNullOrEmpty(selfStr.Trim());
        }


        /// <summary>
        /// 缓存
        /// </summary>
        private static readonly char[] mCachedSplitCharArray = { '.' };

#if UNITY_EDITOR
        // v1 No.22
        [MethodAPI]
        [APIDescriptionCN("字符串分割")]
        [APIDescriptionEN("String splitting")]
        [APIExampleCode(@"
""1.2.3.4.5"".Split('.').ForEach(str=>Debug.Log(str));
// 1 2 3 4 5
        ")]
#endif
        public static string[] Split(this string selfStr, char splitSymbol)
        {
            mCachedSplitCharArray[0] = splitSymbol;
            return selfStr.Split(mCachedSplitCharArray);
        }

#if UNITY_EDITOR
        // v1 No.23
        [MethodAPI]
        [APIDescriptionCN("格式化字符串填充参数")]
        [APIDescriptionEN("The format string populates the parameters")]
        [APIExampleCode(@"

var newStr = ""{0},{1}"".FillFormat(1,2);
Debug.Log(newStr);
// 1,2
        ")]
#endif
        public static string FillFormat(this string selfStr, params object[] args)
        {
            return string.Format(selfStr, args);
        }

#if UNITY_EDITOR
        // v1 No.24
        [MethodAPI]
        [APIDescriptionCN("返回包含此字符串的 StringBuilder")]
        [APIDescriptionEN("Returns a StringBuilder containing this string")]
        [APIExampleCode(@"
var builder = ""Hello"".Builder();
builder.Append("" QF"");
Debug.Log(builder.ToString());
// Hello QF
        ")]
#endif
        public static StringBuilder Builder(this string selfStr)
        {
            return new StringBuilder(selfStr);
        }

#if UNITY_EDITOR
        // v1 No.25
        [MethodAPI]
        [APIDescriptionCN("StringBuilder 添加前缀")]
        [APIDescriptionEN("StringBuilder insert prefix string")]
        [APIExampleCode(@"
var builder = ""I'm liangxie"".Builder().AddPrefix(""Hi!"") ;
Debug.Log(builder.ToString());
// Hi!I'm liangxie
        ")]
#endif
        public static StringBuilder AddPrefix(this StringBuilder self, string prefixString)
        {
            self.Insert(0, prefixString);
            return self;
        }


#if UNITY_EDITOR
        // v1 No.26
        [MethodAPI]
        [APIDescriptionCN("字符串解析成 Int")]
        [APIDescriptionEN("parse string to int")]
        [APIExampleCode(@"
var number = ""123456"".ToInt();
Debug.Log(number);
// 123456
// notice unsafe
// 不安全
        ")]
#endif
        public static int ToInt(this string selfStr, int defaulValue = 0)
        {
            var retValue = defaulValue;
            return int.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }

#if UNITY_EDITOR
        // v1 No.27
        [MethodAPI]
        [APIDescriptionCN("字符串解析成 Int")]
        [APIDescriptionEN("parse string to int")]
        [APIExampleCode(@"
DateTime.Now.ToString().ToDataTime();
        ")]
#endif
        public static DateTime ToDateTime(this string selfStr, DateTime defaultValue = default(DateTime))
        {
            return DateTime.TryParse(selfStr, out var retValue) ? retValue : defaultValue;
        }


#if UNITY_EDITOR
        // v1 No.28
        [MethodAPI]
        [APIDescriptionCN("字符串解析成 float")]
        [APIDescriptionEN("parse string to float")]
        [APIExampleCode(@"
var number = ""123456f"".ToInt();
Debug.Log(number);
// 123456
// notice unsafe
// 不安全
        ")]
#endif
        public static float ToFloat(this string selfStr, float defaultValue = 0)
        {
            return float.TryParse(selfStr, out var retValue) ? retValue : defaultValue;
        }

#if UNITY_EDITOR
        // v1 No.29
        [MethodAPI]
        [APIDescriptionCN("是否存在中文字符")]
        [APIDescriptionEN("check string contains chinese or not")]
        [APIExampleCode(@"
Debug.Log(""你好"".HasChinese());
// true
")]
#endif
        public static bool HasChinese(this string input)
        {
            return Regex.IsMatch(input, @"[\u4e00-\u9fa5]");
        }

#if UNITY_EDITOR
        // v1 No.30
        [MethodAPI]
        [APIDescriptionCN("是否存在空格")]
        [APIDescriptionEN("check string contains space or not")]
        [APIExampleCode(@"
Debug.Log(""你好 "".HasSpace());
// true
")]
#endif
        public static bool HasSpace(this string input)
        {
            return input.Contains(" ");
        }

#if UNITY_EDITOR
        // v1 No.31
        [MethodAPI]
        [APIDescriptionCN("remove string")]
        [APIDescriptionEN("check string contains space or not")]
        [APIExampleCode(@"
Debug.Log(""Hello World "".RemoveString(""Hello"","" ""));
// World
")]
#endif
        public static string RemoveString(this string str, params string[] targets)
        {
            return targets.Aggregate(str, (current, t) => current.Replace(t, string.Empty));
        }
        
#if UNITY_EDITOR
        // v1.0.39
        [MethodAPI]
        [APIDescriptionCN("join string")]
        [APIDescriptionEN("join string")]
        [APIExampleCode(@"
Debug.Log(new List<string>() { ""1"",""2"",""3""}.StringJoin("",""));
// 1,2,3
")]
#endif
        public static string StringJoin(this IEnumerable<string> self, string separator)
        {
                return string.Join(separator, self);
        }
    }
}