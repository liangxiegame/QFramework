/****************************************************************************
 * Copyright (c) 2018.3 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 常量 TODO:这个要干掉,最好是自动生成的唯一表示符号
	/// </summary>
	/// <summary>
	/// 关卡需要生成的标记
	/// </summary>
	public class STAGE
	{
		public const int EON = -1; // Empty Or Not
		public const int EM  = 0; // Empty 

		public const int BLOCK_BEGIN = 1;
		public const int BL          = 1; // Block Left
		public const int BM          = 2; // Block Middle
		public const int BR          = 3; // Block Right
		public const int BA          = 4; // Block Air
		public const int BLOCK_END   = 5;

		public const int GOLD_BEGIN = 9;
		public const int G1         = 9; // Gold x1
		public const int G3         = 10; // Gold x3
		public const int G6         = 11; // Gold x6
		public const int GC         = 12; // Gold Circle
		public const int GOLD_END   = 13;

		public const int PROP_BEGIN = 20;
		public const int PA         = 20; // Prop Auto
		public const int PB         = 21; // Prop Big
		public const int PF2        = 22; // Prop Fruit x2
		public const int PG2        = 23; // Prop Gold  x2
		public const int PM         = 24; // Prop Magnetite
		public const int PP         = 25; // Prop Propect
		public const int PROP_END   = 26;
		public const int PTE        = 27; // Prop Time Extra
		public const int PF         = 28; // Prop Find	// 这个不要了

		public const int FRUIT_BEGIN = 30;
		public const int FB          = 30; // Fruit Banana
		public const int FC          = 31; // Fruit Coconut
		public const int FM          = 32; // Fruit Mango
		public const int FPA         = 33; // Fruit Pineapple
		public const int FP          = 34; // Fruit Pitaya
		public const int FRUIT_END   = 35;

		public const int ENEMY_BEGIN = 40;
		public const int ET          = 40; // Enemy Turtle
		public const int EC          = 41; // Enemy Crab
		public const int ENEMY_END   = 42;

		/// <summary>
		/// 前景组合
		/// </summary>
		public const int COM1_BEGIN = 100;

		public const int COM2_BEGIN = 100;

		public const int C1 = 100; // Component1 
		public const int C2 = 101; // Component2
		public const int C3 = 102; // Component3
		public const int C4 = 103; // Component3
		public const int C5 = 104; // Component3
		public const int C6 = 105; // Component3
		public const int C7 = 106; // Component3
		public const int C8 = 107; // Component3
		public const int C9 = 108; // Component3

		public const int COM1_END = 109;
		public const int C10      = 109;
		public const int C11      = 110;
		public const int C12      = 111;
		public const int C13      = 112;
		public const int C14      = 113;
		public const int C15      = 114;
		public const int C16      = 115;
		public const int C17      = 116;
		public const int C18      = 117;
		public const int C19      = 128;
		public const int C20      = 119;
		public const int COM2_END = 120;
	}


	/// <summary>
	/// 音效:1.同时可播放多个的,2.一次只播放一个的 3.背景音乐
	/// </summary>

	public class SOUND
	{

		/// <summary>
		/// 设置
		/// </summary>
		public const int ON = 1; // 声音开启

		public const int OFF = 0; // 声音关闭

		public const int COIN  = 0; // 金币
		public const int JUMP  = 1; // 跳
		public const int DEATH = 2; // 死亡音效
		public const int SHAKE = 3; // 震屏

		public const int BTN   = 4; //按钮点击
		public const int FRUIT = 5; // 吃到水果的音效

		public const int PROP_BIG = 6; // 变大道具的音效

		public const int ENEMY_DEATH1 = 7; // 怪物被踩死或者踢飞
		public const int ENEMY_DEATH2 = 8; // 怪物被踩死或者踢飞

		public const int HERO_HURT = 9; // 英雄被攻击

		public const int FOREST     = 10; // 森林
		public const int FIRE       = 11; // 火焰燃烧
		public const int BOSS_SHAKE = 12; // 敌人出来的时候震动

		public const int COUNT = 13; // 音效的个数,要开辟的数量

	}

	public class EMPTY
	{
		public const int ZERO  = 0;
		public const int ONE   = 1;
		public const int TWO   = 2;
		public const int THREE = 3;
		public const int FOUR  = 4;
		public const int FIVE  = 5;
		public const int SIX   = 6;
	}
}