using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

/// <summary>
/// 1.泛型
/// 2.反射
/// 3.抽象类
/// 4.命名空间
/// </summary>
namespace QFramework {
	public abstract class QSingleton<T> where T : QSingleton<T>
	{
		protected static T instance = null;

		protected QSingleton()
		{
		}

		public static T Instance()
		{
			if (instance == null)
			{
				// 先获取所有非public的构造方法
				ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
				// 从ctors中获取无参的构造方法
				ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
				if (ctor == null)
					throw new Exception("Non-public ctor() not found!");
				// 调用构造方法
				instance = ctor.Invoke(null) as T;
			}

			return instance;
		}
	}
}