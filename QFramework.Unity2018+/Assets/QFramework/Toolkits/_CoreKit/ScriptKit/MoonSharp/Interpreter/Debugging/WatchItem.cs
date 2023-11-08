
namespace MoonSharp.Interpreter.Debugging
{
	/// <summary>
	/// A watch item for the debugger to consume.
	/// Most properties make or not sense depending on the WatchType.
	/// </summary>
	public class WatchItem
	{
		/// <summary>
		/// Gets or sets the address of the item
		/// </summary>
		public int Address { get; set; }
		/// <summary>
		/// Gets or sets the base pointer (base value of v-stack at entering the function).
		/// Valid only for call-stack items
		/// </summary>
		public int BasePtr { get; set; }
		/// <summary>
		/// Gets or sets the return address.
		/// Valid only for call-stack items
		/// </summary>
		public int RetAddress { get; set; }
		/// <summary>
		/// Gets or sets the name of the item
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Gets or sets the value of the item
		/// </summary>
		public DynValue Value { get; set; }
		/// <summary>
		/// Gets or sets the symbol reference of the item 
		/// </summary>
		public SymbolRef LValue { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this instance is generating an error.
		/// </summary>
		public bool IsError { get; set; }
		/// <summary>
		/// Gets or sets the source location this item refers to.
		/// </summary>
		public SourceRef Location { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
				Address, BasePtr, RetAddress, Name ?? "(null)",
				Value != null ? Value.ToString() : "(null)",
				LValue != null ? LValue.ToString() : "(null)");
		}

	}
}
