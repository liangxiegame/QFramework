using System;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Interface for proxy objects (type unsafe version)
	/// </summary>
	public interface IProxyFactory
	{
		/// <summary>
		/// Takes an instance of a target object and returns a proxy object wrapping it
		/// </summary>
		object CreateProxyObject(object o);
		/// <summary>
		/// Gets the proxied type
		/// </summary>
		Type TargetType { get; }
		/// <summary>
		/// Gets the proxy type
		/// </summary>
		Type ProxyType { get; }
	}

	/// <summary>
	/// Interface for proxy objects (type safe version)
	/// </summary>
	/// <typeparam name="TProxy">The type of the proxy.</typeparam>
	/// <typeparam name="TTarget">The type of the target.</typeparam>
	public interface IProxyFactory<TProxy, TTarget> : IProxyFactory
		where TProxy : class
		where TTarget : class
	{
		/// <summary>
		/// Takes an instance of a target object and returns a proxy object wrapping it
		/// </summary>
		TProxy CreateProxyObject(TTarget target);
	}

}
