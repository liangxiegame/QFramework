using System.Text;

namespace MoonSharp.Interpreter.Tree
{
	class Lexer
	{
		Token m_Current = null;
		string m_Code;
		int m_PrevLineTo = 0;
		int m_PrevColTo = 1;
		int m_Cursor = 0;
		int m_Line = 1;
		int m_Col = 0;
		int m_SourceId;
		bool m_AutoSkipComments = false;

		public Lexer(int sourceID, string scriptContent, bool autoSkipComments)
		{
			m_Code = scriptContent;
			m_SourceId = sourceID;

			// remove unicode BOM if any
			if (m_Code.Length > 0 && m_Code[0] == 0xFEFF)
				m_Code = m_Code.Substring(1);

			m_AutoSkipComments = autoSkipComments;
		}

		public Token Current
		{
			get
			{
				if (m_Current == null)
					Next();

				return m_Current;
			}
		}

		private Token FetchNewToken()
		{
			while (true)
			{
				Token T = ReadToken();

				//System.Diagnostics.Debug.WriteLine("LEXER : " + T.ToString());

				if ((T.Type != TokenType.Comment && T.Type != TokenType.HashBang) || (!m_AutoSkipComments))
					return T;
			}
		}

		public void Next()
		{
			m_Current = FetchNewToken();
		}

		public Token PeekNext()
		{
			int snapshot = m_Cursor;
			Token current = m_Current;
			int line = m_Line;
			int col = m_Col;

			Next();
			Token t = Current;

			m_Cursor = snapshot;
			m_Current = current;
			m_Line = line;
			m_Col = col;

			return t;
		}


		private void CursorNext()
		{
			if (CursorNotEof())
			{
				if (CursorChar() == '\n')
				{
					m_Col = 0;
					m_Line += 1;
				}
				else
				{
					m_Col += 1;
				}

				m_Cursor += 1;
			}
		}

		private char CursorChar()
		{
			if (m_Cursor < m_Code.Length)
				return m_Code[m_Cursor];
			else
				return '\0'; //  sentinel
		}

		private char CursorCharNext()
		{
			CursorNext();
			return CursorChar();
		}

		private bool CursorMatches(string pattern)
		{
			for (int i = 0; i < pattern.Length; i++)
			{
				int j = m_Cursor + i;

				if (j >= m_Code.Length)
					return false;
				if (m_Code[j] != pattern[i])
					return false;
			}
			return true;
		}

		private bool CursorNotEof()
		{
			return m_Cursor < m_Code.Length;
		}

		private bool IsWhiteSpace(char c)
		{
			return char.IsWhiteSpace(c);
		}

		private void SkipWhiteSpace()
		{
			for (; CursorNotEof() && IsWhiteSpace(CursorChar()); CursorNext())
			{
			}
		}


		private Token ReadToken()
		{
			SkipWhiteSpace();

			int fromLine = m_Line;
			int fromCol = m_Col;

			if (!CursorNotEof())
				return CreateToken(TokenType.Eof, fromLine, fromCol, "<eof>");

			char c = CursorChar();

			switch (c)
			{
				case '|':
					CursorCharNext();
					return CreateToken(TokenType.Lambda, fromLine, fromCol, "|");
				case ';':
					CursorCharNext();
					return CreateToken(TokenType.SemiColon, fromLine, fromCol, ";");
				case '=':
					return PotentiallyDoubleCharOperator('=', TokenType.Op_Assignment, TokenType.Op_Equal, fromLine, fromCol);
				case '<':
					return PotentiallyDoubleCharOperator('=', TokenType.Op_LessThan, TokenType.Op_LessThanEqual, fromLine, fromCol);
				case '>':
					return PotentiallyDoubleCharOperator('=', TokenType.Op_GreaterThan, TokenType.Op_GreaterThanEqual, fromLine, fromCol);
				case '~':
				case '!':
					if (CursorCharNext() != '=')
						throw new SyntaxErrorException(CreateToken(TokenType.Invalid, fromLine, fromCol), "unexpected symbol near '{0}'", c);

					CursorCharNext();
					return CreateToken(TokenType.Op_NotEqual, fromLine, fromCol, "~=");
				case '.':
					{
						char next = CursorCharNext();
						if (next == '.')
							return PotentiallyDoubleCharOperator('.', TokenType.Op_Concat, TokenType.VarArgs, fromLine, fromCol);
						else if (LexerUtils.CharIsDigit(next))
							return ReadNumberToken(fromLine, fromCol, true);
						else
							return CreateToken(TokenType.Dot, fromLine, fromCol, ".");
					}
				case '+':
					return CreateSingleCharToken(TokenType.Op_Add, fromLine, fromCol);
				case '-':
					{
						char next = CursorCharNext();
						if (next == '-')
						{
							return ReadComment(fromLine, fromCol);
						}
						else
						{
							return CreateToken(TokenType.Op_MinusOrSub, fromLine, fromCol, "-");
						}
					}
				case '*':
					return CreateSingleCharToken(TokenType.Op_Mul, fromLine, fromCol);
				case '/':
					return CreateSingleCharToken(TokenType.Op_Div, fromLine, fromCol);
				case '%':
					return CreateSingleCharToken(TokenType.Op_Mod, fromLine, fromCol);
				case '^':
					return CreateSingleCharToken(TokenType.Op_Pwr, fromLine, fromCol);
				case '$':
					return PotentiallyDoubleCharOperator('{', TokenType.Op_Dollar, TokenType.Brk_Open_Curly_Shared, fromLine, fromCol);
				case '#':
					if (m_Cursor == 0 && m_Code.Length > 1 && m_Code[1] == '!')
						return ReadHashBang(fromLine, fromCol);

					return CreateSingleCharToken(TokenType.Op_Len, fromLine, fromCol);
				case '[':
					{
						char next = CursorCharNext();
						if (next == '=' || next == '[')
						{
							string str = ReadLongString(fromLine, fromCol, null, "string");
							return CreateToken(TokenType.String_Long, fromLine, fromCol, str);
						}
						return CreateToken(TokenType.Brk_Open_Square, fromLine, fromCol, "[");
					}
				case ']':
					return CreateSingleCharToken(TokenType.Brk_Close_Square, fromLine, fromCol);
				case '(':
					return CreateSingleCharToken(TokenType.Brk_Open_Round, fromLine, fromCol);
				case ')':
					return CreateSingleCharToken(TokenType.Brk_Close_Round, fromLine, fromCol);
				case '{':
					return CreateSingleCharToken(TokenType.Brk_Open_Curly, fromLine, fromCol);
				case '}':
					return CreateSingleCharToken(TokenType.Brk_Close_Curly, fromLine, fromCol);
				case ',':
					return CreateSingleCharToken(TokenType.Comma, fromLine, fromCol);
				case ':':
					return PotentiallyDoubleCharOperator(':', TokenType.Colon, TokenType.DoubleColon, fromLine, fromCol);
				case '"':
				case '\'':
					return ReadSimpleStringToken(fromLine, fromCol);
				case '\0':
					throw new SyntaxErrorException(CreateToken(TokenType.Invalid, fromLine, fromCol), "unexpected symbol near '{0}'", CursorChar())
					{
						IsPrematureStreamTermination = true
					};
				default:
					{
						if (char.IsLetter(c) || c == '_')
						{
							string name = ReadNameToken();
							return CreateNameToken(name, fromLine, fromCol);
						}
						else if (LexerUtils.CharIsDigit(c))
						{
							return ReadNumberToken(fromLine, fromCol, false);
						}
					}

					throw new SyntaxErrorException(CreateToken(TokenType.Invalid, fromLine, fromCol), "unexpected symbol near '{0}'", CursorChar());
			}
		}

		private string ReadLongString(int fromLine, int fromCol, string startpattern, string subtypeforerrors)
		{
			// here we are at the first '=' or second '['
			StringBuilder text = new StringBuilder(1024);
			string end_pattern = "]";

			if (startpattern == null)
			{
				for (char c = CursorChar(); ; c = CursorCharNext())
				{
					if (c == '\0' || !CursorNotEof())
					{
						throw new SyntaxErrorException(
							CreateToken(TokenType.Invalid, fromLine, fromCol),
							"unfinished long {0} near '<eof>'", subtypeforerrors) { IsPrematureStreamTermination = true };
					}
					else if (c == '=')
					{
						end_pattern += "=";
					}
					else if (c == '[')
					{
						end_pattern += "]";
						break;
					}
					else
					{
						throw new SyntaxErrorException(
							CreateToken(TokenType.Invalid, fromLine, fromCol),
							"invalid long {0} delimiter near '{1}'", subtypeforerrors, c) { IsPrematureStreamTermination = true };
					}
				}
			}
			else
			{
				end_pattern = startpattern.Replace('[', ']');
			}


			for (char c = CursorCharNext(); ; c = CursorCharNext())
			{
				if (c == '\r') // XXI century and we still debate on how a newline is made. throw new DeveloperExtremelyAngryException.
					continue;

				if (c == '\0' || !CursorNotEof())
				{
					throw new SyntaxErrorException(
							CreateToken(TokenType.Invalid, fromLine, fromCol),
							"unfinished long {0} near '{1}'", subtypeforerrors, text.ToString()) { IsPrematureStreamTermination = true };
				}
				else if (c == ']' && CursorMatches(end_pattern))
				{
					for (int i = 0; i < end_pattern.Length; i++)
						CursorCharNext();

					return LexerUtils.AdjustLuaLongString(text.ToString());
				}
				else
				{
					text.Append(c);
				}
			}
		}

		private Token ReadNumberToken(int fromLine, int fromCol, bool leadingDot)
		{
			StringBuilder text = new StringBuilder(32);

			//INT : Digit+
			//HEX : '0' [xX] HexDigit+
			//FLOAT : Digit+ '.' Digit* ExponentPart?
			//		| '.' Digit+ ExponentPart?
			//		| Digit+ ExponentPart
			//HEX_FLOAT : '0' [xX] HexDigit+ '.' HexDigit* HexExponentPart?
			//			| '0' [xX] '.' HexDigit+ HexExponentPart?
			//			| '0' [xX] HexDigit+ HexExponentPart
			//
			// ExponentPart : [eE] [+-]? Digit+
			// HexExponentPart : [pP] [+-]? Digit+

			bool isHex = false;
			bool dotAdded = false;
			bool exponentPart = false;
			bool exponentSignAllowed = false;

			if (leadingDot)
			{
				text.Append("0.");
			}
			else if (CursorChar() == '0')
			{
				text.Append(CursorChar());
				char secondChar = CursorCharNext();

				if (secondChar == 'x' || secondChar == 'X')
				{
					isHex = true;
					text.Append(CursorChar());
					CursorCharNext();
				}
			}

			for (char c = CursorChar(); CursorNotEof(); c = CursorCharNext())
			{
				if (exponentSignAllowed && (c == '+' || c == '-'))
				{
					exponentSignAllowed = false;
					text.Append(c);
				}
				else if (LexerUtils.CharIsDigit(c))
				{
					text.Append(c);
				}
				else if (c == '.' && !dotAdded)
				{
					dotAdded = true;
					text.Append(c);
				}
				else if (LexerUtils.CharIsHexDigit(c) && isHex && !exponentPart)
				{
					text.Append(c);
				}
				else if (c == 'e' || c == 'E' || (isHex && (c == 'p' || c == 'P')))
				{
					text.Append(c);
					exponentPart = true;
					exponentSignAllowed = true;
					dotAdded = true;
				}
				else
				{
					break;
				}
			}

			TokenType numberType = TokenType.Number;

			if (isHex && (dotAdded || exponentPart))
				numberType = TokenType.Number_HexFloat;
			else if (isHex)
				numberType = TokenType.Number_Hex;

			string tokenStr = text.ToString();
			return CreateToken(numberType, fromLine, fromCol, tokenStr);
		}

		private Token CreateSingleCharToken(TokenType tokenType, int fromLine, int fromCol)
		{
			char c = CursorChar();
			CursorCharNext();
			return CreateToken(tokenType, fromLine, fromCol, c.ToString());
		}

		private Token ReadHashBang(int fromLine, int fromCol)
		{
			StringBuilder text = new StringBuilder(32);

			for (char c = CursorChar(); CursorNotEof(); c = CursorCharNext())
			{
				if (c == '\n')
				{
					CursorCharNext();
					return CreateToken(TokenType.HashBang, fromLine, fromCol, text.ToString());
				}
				else if (c != '\r')
				{
					text.Append(c);
				}
			}

			return CreateToken(TokenType.HashBang, fromLine, fromCol, text.ToString());
		}


		private Token ReadComment(int fromLine, int fromCol)
		{
			StringBuilder text = new StringBuilder(32);

			bool extraneousFound = false;

			for (char c = CursorCharNext(); CursorNotEof(); c = CursorCharNext())
			{
				if (c == '[' && !extraneousFound && text.Length > 0)
				{
					text.Append('[');
					//CursorCharNext();
					string comment = ReadLongString(fromLine, fromCol, text.ToString(), "comment");
					return CreateToken(TokenType.Comment, fromLine, fromCol, comment);
				}
				else if (c == '\n')
				{
					extraneousFound = true;
					CursorCharNext();
					return CreateToken(TokenType.Comment, fromLine, fromCol, text.ToString());
				}
				else if (c != '\r')
				{
					if (c != '[' && c != '=')
						extraneousFound = true;

					text.Append(c);
				}
			}

			return CreateToken(TokenType.Comment, fromLine, fromCol, text.ToString());
		}

		private Token ReadSimpleStringToken(int fromLine, int fromCol)
		{
			StringBuilder text = new StringBuilder(32);
			char separator = CursorChar();

			for (char c = CursorCharNext(); CursorNotEof(); c = CursorCharNext())
			{
			redo_Loop:

				if (c == '\\')
				{
					text.Append(c);
					c = CursorCharNext();
					text.Append(c);

					if (c == '\r')
					{
						c = CursorCharNext();
						if (c == '\n')
							text.Append(c);
						else
							goto redo_Loop;
					}
					else if (c == 'z')
					{
						c = CursorCharNext();

						if (char.IsWhiteSpace(c))
							SkipWhiteSpace();

						c = CursorChar();

						goto redo_Loop;
					}
				}
				else if (c == '\n' || c == '\r')
				{
					throw new SyntaxErrorException(
						CreateToken(TokenType.Invalid, fromLine, fromCol),
						"unfinished string near '{0}'", text.ToString());
				}
				else if (c == separator)
				{
					CursorCharNext();
					Token t = CreateToken(TokenType.String, fromLine, fromCol);
					t.Text = LexerUtils.UnescapeLuaString(t, text.ToString());
					return t;
				}
				else
				{
					text.Append(c);
				}
			}

			throw new SyntaxErrorException(
				CreateToken(TokenType.Invalid, fromLine, fromCol),
				"unfinished string near '{0}'", text.ToString()) { IsPrematureStreamTermination = true };
		}


		private Token PotentiallyDoubleCharOperator(char expectedSecondChar, TokenType singleCharToken, TokenType doubleCharToken, int fromLine, int fromCol)
		{
			string op = CursorChar().ToString();

			CursorCharNext();

			if (CursorChar() == expectedSecondChar)
			{
				CursorCharNext();
				return CreateToken(doubleCharToken, fromLine, fromCol, op + expectedSecondChar);
			}
			else
				return CreateToken(singleCharToken, fromLine, fromCol, op);
		}



		private Token CreateNameToken(string name, int fromLine, int fromCol)
		{
			TokenType? reservedType = Token.GetReservedTokenType(name);

			if (reservedType.HasValue)
			{
				return CreateToken(reservedType.Value, fromLine, fromCol, name);
			}
			else
			{
				return CreateToken(TokenType.Name, fromLine, fromCol, name);
			}
		}


		private Token CreateToken(TokenType tokenType, int fromLine, int fromCol, string text = null)
		{
			Token t = new Token(tokenType, m_SourceId, fromLine, fromCol, m_Line, m_Col, m_PrevLineTo, m_PrevColTo)
			{
				Text = text
			};
			m_PrevLineTo = m_Line;
			m_PrevColTo = m_Col;
			return t;
		}

		private string ReadNameToken()
		{
			StringBuilder name = new StringBuilder(32);

			for (char c = CursorChar(); CursorNotEof(); c = CursorCharNext())
			{
				if (char.IsLetterOrDigit(c) || c == '_')
					name.Append(c);
				else
					break;
			}

			return name.ToString();
		}




	}
}
