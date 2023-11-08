using System;

namespace MoonSharp.Interpreter.Tree
{
	class Token
	{
		public readonly int SourceId;
		public readonly int FromCol, ToCol, FromLine, ToLine, PrevCol, PrevLine;
		public readonly TokenType Type;

		public string Text { get; set; }

		public Token(TokenType type, int sourceId, int fromLine, int fromCol, int toLine, int toCol, int prevLine, int prevCol)
		{
			Type = type;

			SourceId = sourceId;
			FromLine = fromLine;
			FromCol = fromCol;
			ToCol = toCol;
			ToLine = toLine;
			PrevCol = prevCol;
			PrevLine = prevLine;
		}


		public override string ToString()
		{
			string tokenTypeString = (Type.ToString() + "                                                      ").Substring(0, 16);

			string location = string.Format("{0}:{1}-{2}:{3}", FromLine, FromCol, ToLine, ToCol);

			location = (location + "                                                      ").Substring(0, 10);

			return string.Format("{0}  - {1} - '{2}'", tokenTypeString, location, this.Text ?? "");
		}

		public static TokenType? GetReservedTokenType(string reservedWord)
		{
			switch (reservedWord)
			{
				case "and":
					return TokenType.And;
				case "break":
					return TokenType.Break;
				case "do":
					return TokenType.Do;
				case "else":
					return TokenType.Else;
				case "elseif":
					return TokenType.ElseIf;
				case "end":
					return TokenType.End;
				case "false":
					return TokenType.False;
				case "for":
					return TokenType.For;
				case "function":
					return TokenType.Function;
				case "goto":
					return TokenType.Goto;
				case "if":
					return TokenType.If;
				case "in":
					return TokenType.In;
				case "local":
					return TokenType.Local;
				case "nil":
					return TokenType.Nil;
				case "not":
					return TokenType.Not;
				case "or":
					return TokenType.Or;
				case "repeat":
					return TokenType.Repeat;
				case "return":
					return TokenType.Return;
				case "then":
					return TokenType.Then;
				case "true":
					return TokenType.True;
				case "until":
					return TokenType.Until;
				case "while":
					return TokenType.While;
				default:
					return null;
			}
		}

		public double GetNumberValue()
		{
			if (this.Type == TokenType.Number)
				return LexerUtils.ParseNumber(this);
			else if (this.Type == TokenType.Number_Hex)
				return LexerUtils.ParseHexInteger(this);
			else if (this.Type == TokenType.Number_HexFloat)
				return LexerUtils.ParseHexFloat(this);
			else
				throw new NotSupportedException("GetNumberValue is supported only on numeric tokens");
		}


		public bool IsEndOfBlock()
		{
			switch (Type)
			{
				case TokenType.Else:
				case TokenType.ElseIf:
				case TokenType.End:
				case TokenType.Until:
				case TokenType.Eof:
					return true;
				default:
					return false;
			}
		}

		public bool IsUnaryOperator()
		{
			return Type == TokenType.Op_MinusOrSub || Type == TokenType.Not || Type == TokenType.Op_Len;
		}

		public bool IsBinaryOperator()
		{
			switch (Type)
			{
				case TokenType.And:
				case TokenType.Or:
				case TokenType.Op_Equal:
				case TokenType.Op_LessThan:
				case TokenType.Op_LessThanEqual:
				case TokenType.Op_GreaterThanEqual:
				case TokenType.Op_GreaterThan:
				case TokenType.Op_NotEqual:
				case TokenType.Op_Concat:
				case TokenType.Op_Pwr:
				case TokenType.Op_Mod:
				case TokenType.Op_Div:
				case TokenType.Op_Mul:
				case TokenType.Op_MinusOrSub:
				case TokenType.Op_Add:
					return true;
				default:
					return false;
			}
		}


		internal Debugging.SourceRef GetSourceRef(bool isStepStop = true)
		{
			return new Debugging.SourceRef(this.SourceId, this.FromCol, this.ToCol, this.FromLine, this.ToLine, isStepStop);
		}

		internal Debugging.SourceRef GetSourceRef(Token to, bool isStepStop = true)
		{
			return new Debugging.SourceRef(this.SourceId, this.FromCol, to.ToCol, this.FromLine, to.ToLine, isStepStop);
		}

		internal Debugging.SourceRef GetSourceRefUpTo(Token to, bool isStepStop = true)
		{
			return new Debugging.SourceRef(this.SourceId, this.FromCol, to.PrevCol, this.FromLine, to.PrevLine, isStepStop);
		}
	}
}
