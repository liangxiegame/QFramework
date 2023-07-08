using System.Linq;

namespace MoonSharp.Interpreter.Interop
{

	/// <summary>
	/// Helps identifying a reflection special name
	/// </summary>
	public enum ReflectionSpecialNameType
	{
		IndexGetter,
		IndexSetter,
		ImplicitCast,
		ExplicitCast,

		OperatorTrue,
		OperatorFalse,

		PropertyGetter,
		PropertySetter,
		AddEvent,
		RemoveEvent,

		OperatorAdd,
		OperatorAnd,
		OperatorOr,
		OperatorDec,
		OperatorDiv,
		OperatorEq,
		OperatorXor,
		OperatorGt,
		OperatorGte,
		OperatorInc,
		OperatorNeq,
		OperatorLt,
		OperatorLte,
		OperatorNot,
		OperatorMod,
		OperatorMul,
		OperatorCompl,
		OperatorSub,
		OperatorNeg,
		OperatorUnaryPlus,
	}

	/// <summary>
	/// Class helping identifying special names found with reflection
	/// </summary>
	public struct ReflectionSpecialName
	{
		public ReflectionSpecialNameType Type { get; private set; }
		public string Argument { get; private set; }

		public ReflectionSpecialName(ReflectionSpecialNameType type, string argument = null)
			: this()
		{
			Type = type;
			Argument = argument;
		}

		public ReflectionSpecialName(string name)
			: this()
		{
			if (name.Contains("."))
			{
				string[] split = name.Split('.');
				name = split[split.Length - 1];
			}

			switch (name)
			{
				case "op_Explicit":
					Type = ReflectionSpecialNameType.ExplicitCast;
					return;
				case "op_Implicit":
					Type = ReflectionSpecialNameType.ImplicitCast;
					return;
				case "set_Item":
					Type = ReflectionSpecialNameType.IndexSetter;
					return;
				case "get_Item":
					Type = ReflectionSpecialNameType.IndexGetter;
					return;
				case "op_Addition":
					Type = ReflectionSpecialNameType.OperatorAdd;
					Argument = "+";
					return;
				case "op_BitwiseAnd":
					Type = ReflectionSpecialNameType.OperatorAnd;
					Argument = "&";
					return;
				case "op_BitwiseOr":
					Type = ReflectionSpecialNameType.OperatorOr;
					Argument = "|";
					return;
				case "op_Decrement":
					Type = ReflectionSpecialNameType.OperatorDec;
					Argument = "--";
					return;
				case "op_Division":
					Type = ReflectionSpecialNameType.OperatorDiv;
					Argument = "/";
					return;
				case "op_Equality":
					Type = ReflectionSpecialNameType.OperatorEq;
					Argument = "==";
					return;
				case "op_ExclusiveOr":
					Type = ReflectionSpecialNameType.OperatorXor;
					Argument = "^";
					return;
				case "op_False":
					Type = ReflectionSpecialNameType.OperatorFalse;
					return;
				case "op_GreaterThan":
					Type = ReflectionSpecialNameType.OperatorGt;
					Argument = ">";
					return;
				case "op_GreaterThanOrEqual":
					Type = ReflectionSpecialNameType.OperatorGte;
					Argument = ">=";
					return;
				case "op_Increment":
					Type = ReflectionSpecialNameType.OperatorInc;
					Argument = "++";
					return;
				case "op_Inequality":
					Type = ReflectionSpecialNameType.OperatorNeq;
					Argument = "!=";
					return;
				case "op_LessThan":
					Type = ReflectionSpecialNameType.OperatorLt;
					Argument = "<";
					return;
				case "op_LessThanOrEqual":
					Type = ReflectionSpecialNameType.OperatorLte;
					Argument = "<=";
					return;
				case "op_LogicalNot":
					Type = ReflectionSpecialNameType.OperatorNot;
					Argument = "!";
					return;
				case "op_Modulus":
					Type = ReflectionSpecialNameType.OperatorMod;
					Argument = "%";
					return;
				case "op_Multiply":
					Type = ReflectionSpecialNameType.OperatorMul;
					Argument = "*";
					return;
				case "op_OnesComplement":
					Type = ReflectionSpecialNameType.OperatorCompl;
					Argument = "~";
					return;
				case "op_Subtraction":
					Type = ReflectionSpecialNameType.OperatorSub;
					Argument = "-";
					return;
				case "op_True":
					Type = ReflectionSpecialNameType.OperatorTrue;
					return;
				case "op_UnaryNegation":
					Type = ReflectionSpecialNameType.OperatorNeg;
					Argument = "-";
					return;
				case "op_UnaryPlus":
					Type = ReflectionSpecialNameType.OperatorUnaryPlus;
					Argument = "+";
					return;
			}

			if (name.StartsWith("get_"))
			{
				Type = ReflectionSpecialNameType.PropertyGetter;
				Argument = name.Substring(4);
			}
			else if (name.StartsWith("set_"))
			{
				Type = ReflectionSpecialNameType.PropertySetter;
				Argument = name.Substring(4);
			}
			else if (name.StartsWith("add_"))
			{
				Type = ReflectionSpecialNameType.AddEvent;
				Argument = name.Substring(4);
			}
			else if (name.StartsWith("remove_"))
			{
				Type = ReflectionSpecialNameType.RemoveEvent;
				Argument = name.Substring(7);
			}
		}
	}
}

