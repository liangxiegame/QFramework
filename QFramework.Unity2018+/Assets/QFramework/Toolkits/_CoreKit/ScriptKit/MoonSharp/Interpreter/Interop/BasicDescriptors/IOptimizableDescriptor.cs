
namespace MoonSharp.Interpreter.Interop.BasicDescriptors
{
	/// <summary>
	/// Interface for descriptors of any kind which support optimizations of their implementation according to InteropAccessMode
	/// modes. This should seldom - if ever - be implemented in user code.
	/// </summary>
	public interface IOptimizableDescriptor
	{
		/// <summary>
		/// Called by standard descriptors when background optimization or preoptimization needs to be performed.
		/// </summary>
		void Optimize();
	}
}
