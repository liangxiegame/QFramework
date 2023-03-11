/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    [System.Serializable]
    public class LanguageDefine
    {
        public Language Language;
    }

    /// <summary>
    ///   <para>The language the user's operating system is running in. Returned by Application.systemLanguage.</para>
    /// </summary>
    public enum Language 
    {
        /// <summary>
        ///   <para>Afrikaans.</para>
        /// </summary>
        Afrikaans = 0,

        /// <summary>
        ///   <para>Arabic.</para>
        /// </summary>
        Arabic = 1,

        /// <summary>
        ///   <para>Basque.</para>
        /// </summary>
        Basque = 2,

        /// <summary>
        ///   <para>Belarusian.</para>
        /// </summary>
        Belarusian = 3,

        /// <summary>
        ///   <para>Bulgarian.</para>
        /// </summary>
        Bulgarian = 4,

        /// <summary>
        ///   <para>Catalan.</para>
        /// </summary>
        Catalan = 5,

        /// <summary>
        ///   <para>Chinese.</para>
        /// </summary>
        Chinese = 6,

        /// <summary>
        ///   <para>Czech.</para>
        /// </summary>
        Czech = 7,

        /// <summary>
        ///   <para>Danish.</para>
        /// </summary>
        Danish = 8,

        /// <summary>
        ///   <para>Dutch.</para>
        /// </summary>
        Dutch = 9,

        /// <summary>
        ///   <para>English.</para>
        /// </summary>
        English = 10, // 0x0000000A

        /// <summary>
        ///   <para>Estonian.</para>
        /// </summary>
        Estonian = 11, // 0x0000000B

        /// <summary>
        ///   <para>Faroese.</para>
        /// </summary>
        Faroese = 12, // 0x0000000C

        /// <summary>
        ///   <para>Finnish.</para>
        /// </summary>
        Finnish = 13, // 0x0000000D

        /// <summary>
        ///   <para>French.</para>
        /// </summary>
        French = 14, // 0x0000000E

        /// <summary>
        ///   <para>German.</para>
        /// </summary>
        German = 15, // 0x0000000F

        /// <summary>
        ///   <para>Greek.</para>
        /// </summary>
        Greek = 16, // 0x00000010

        /// <summary>
        ///   <para>Hebrew.</para>
        /// </summary>
        Hebrew = 17, // 0x00000011

        // [Obsolete("Use SystemLanguage.Hungarian instead (UnityUpgradable) -> Hungarian", true)]
        Hugarian = 18, // 0x00000012

        /// <summary>
        ///   <para>Hungarian.</para>
        /// </summary>
        Hungarian = 18, // 0x00000012

        /// <summary>
        ///   <para>Icelandic.</para>
        /// </summary>
        Icelandic = 19, // 0x00000013

        /// <summary>
        ///   <para>Indonesian.</para>
        /// </summary>
        Indonesian = 20, // 0x00000014

        /// <summary>
        ///   <para>Italian.</para>
        /// </summary>
        Italian = 21, // 0x00000015

        /// <summary>
        ///   <para>Japanese.</para>
        /// </summary>
        Japanese = 22, // 0x00000016

        /// <summary>
        ///   <para>Korean.</para>
        /// </summary>
        Korean = 23, // 0x00000017

        /// <summary>
        ///   <para>Latvian.</para>
        /// </summary>
        Latvian = 24, // 0x00000018

        /// <summary>
        ///   <para>Lithuanian.</para>
        /// </summary>
        Lithuanian = 25, // 0x00000019

        /// <summary>
        ///   <para>Norwegian.</para>
        /// </summary>
        Norwegian = 26, // 0x0000001A

        /// <summary>
        ///   <para>Polish.</para>
        /// </summary>
        Polish = 27, // 0x0000001B

        /// <summary>
        ///   <para>Portuguese.</para>
        /// </summary>
        Portuguese = 28, // 0x0000001C

        /// <summary>
        ///   <para>Romanian.</para>
        /// </summary>
        Romanian = 29, // 0x0000001D

        /// <summary>
        ///   <para>Russian.</para>
        /// </summary>
        Russian = 30, // 0x0000001E

        /// <summary>
        ///   <para>Serbo-Croatian.</para>
        /// </summary>
        SerboCroatian = 31, // 0x0000001F

        /// <summary>
        ///   <para>Slovak.</para>
        /// </summary>
        Slovak = 32, // 0x00000020

        /// <summary>
        ///   <para>Slovenian.</para>
        /// </summary>
        Slovenian = 33, // 0x00000021

        /// <summary>
        ///   <para>Spanish.</para>
        /// </summary>
        Spanish = 34, // 0x00000022

        /// <summary>
        ///   <para>Swedish.</para>
        /// </summary>
        Swedish = 35, // 0x00000023

        /// <summary>
        ///   <para>Thai.</para>
        /// </summary>
        Thai = 36, // 0x00000024

        /// <summary>
        ///   <para>Turkish.</para>
        /// </summary>
        Turkish = 37, // 0x00000025

        /// <summary>
        ///   <para>Ukrainian.</para>
        /// </summary>
        Ukrainian = 38, // 0x00000026

        /// <summary>
        ///   <para>Vietnamese.</para>
        /// </summary>
        Vietnamese = 39, // 0x00000027

        /// <summary>
        ///   <para>ChineseSimplified.</para>
        /// </summary>
        ChineseSimplified = 40, // 0x00000028

        /// <summary>
        ///   <para>ChineseTraditional.</para>
        /// </summary>
        ChineseTraditional = 41, // 0x00000029

        /// <summary>
        ///   <para>Unknown.</para>
        /// </summary>
        Unknown = 42, // 0x0000002A
    }
}