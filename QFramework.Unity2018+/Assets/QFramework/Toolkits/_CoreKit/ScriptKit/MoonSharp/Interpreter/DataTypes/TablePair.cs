
namespace MoonSharp.Interpreter
{
	/// <summary>
	/// A class representing a key/value pair for Table use
	/// </summary>
	public struct TablePair
	{
		private static TablePair s_NilNode = new TablePair(DynValue.Nil, DynValue.Nil);
		private DynValue mKey, mValue;

		/// <summary>
		/// Gets the key.
		/// </summary>
		public DynValue Key 
		{
			get => mKey;
			private set => mKey = value;
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public DynValue Value
		{
			get => mValue;
			set { if (mKey.IsNotNil()) mValue = value; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="TablePair"/> struct.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="val">The value.</param>
		public TablePair(DynValue key, DynValue val) 
		{
			this.mKey = key;
			this.mValue = val;
		}

		/// <summary>
		/// Gets the nil pair
		/// </summary>
		public static TablePair Nil { get { return s_NilNode; } }
	}
}
