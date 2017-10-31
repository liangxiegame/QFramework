/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

/// <summary>
/// 代码生成工具
/// </summary>

namespace QFramework 
{
	using System.CodeDom;
	using System.IO;
	using System.CodeDom.Compiler;
	using Microsoft.CSharp;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// 访问权限定义
	/// </summary>
	public enum QAccessLimit 
	{
		Public,
		Private,
		Internal,
	}

	/// <summary>
	/// 编译类型
	/// </summary>
	public enum QCompileType 
	{
		Const,
		Static,
		Member,
	}

	/// <summary>
	/// 类型
	/// </summary>
	public enum QTypeDefine 
	{
		String,
		Int,
		UInt,
		UShort,
		Short,
		Char,
		Float,
		Double,
	}

	/// <summary>
	/// 变量定义
	/// </summary>
	public class QVariable 
	{
		/// <summary>
		/// 描述
		/// </summary>
		public string Comment;
		/// <summary>
		/// 访问权限
		/// </summary>
		public QAccessLimit AccessLimit;

		/// <summary>
		/// 类型
		/// </summary>
		public CodeTypeReference Type;

		/// <summary>
		/// 编译类型
		/// </summary>
		public QCompileType CompileType = QCompileType.Member;

		/// <summary>
		/// 变量名
		/// </summary>
		public string Name;

		/// <summary>
		/// 值
		/// </summary>
		public string Value;

		public QVariable() 
		{
		}

		public QVariable(QAccessLimit accessLimit,QCompileType compileType,QTypeDefine type,string name,string value,string comment = null) 
		{
			AccessLimit = accessLimit;
			CompileType = compileType;
			Name = name;
			Value = value;
			Type = QTypeUtil.GetCodeType(type);
			CompileType = compileType;
			Comment = comment;
		}
	}

	public class QTypeUtil 
	{
		public static CodeTypeReference GetCodeType(QTypeDefine type) 
		{
			CodeTypeReference retType  = null;
			switch (type) {
				case QTypeDefine.Char:
					retType = new CodeTypeReference (typeof(System.Char));
					break;
				case QTypeDefine.Double:
					retType = new CodeTypeReference (typeof(System.Double));
					break;
				case QTypeDefine.Float:
					retType = new CodeTypeReference (typeof(System.Decimal));
					break;
				case QTypeDefine.Int:
					retType = new CodeTypeReference (typeof(System.Int32));
					break;
				case QTypeDefine.UInt:
					retType = new CodeTypeReference (typeof(System.UInt32));
					break;
				case QTypeDefine.Short:
					retType = new CodeTypeReference (typeof(System.Int16));
					break;
				case QTypeDefine.UShort:
					retType = new CodeTypeReference (typeof(System.UInt16));
					break;
				case QTypeDefine.String:
					retType = new CodeTypeReference (typeof(System.String));
					break;
			}
			return retType;
		}
	}

	/// <summary>
	/// 属性定义只支持Get
	/// </summary>
	public class QProperty 
	{
		/// <summary>
		/// 描述
		/// </summary>
		public string Comment;
		/// <summary>
		/// 访问权限
		/// </summary>
		public QAccessLimit AccessLimit;

		/// <summary>
		/// 类型
		/// </summary>
		public CodeTypeReference Type;

		/// <summary>
		/// 编译类型
		/// </summary>
		public QCompileType CompileType = QCompileType.Member;

		/// <summary>
		/// 变量名
		/// </summary>
		public string Name;

		/// <summary>
		/// 代码
		/// </summary>
		public string GetReturnCode;

		public QProperty() {}

		public QProperty(QAccessLimit accessLimit,QCompileType compileType,QTypeDefine type,string name,string getReturnCode,string comment = null) 
		{
			AccessLimit = accessLimit;
			Type = QTypeUtil.GetCodeType (type);
			CompileType = compileType;
			Name = name;
			GetReturnCode = getReturnCode;
			Comment = comment;
		}

	}

	/// <summary>
	/// 命名空间
	/// </summary>
	public class QNamespaceDefine 
	{
		/// <summary>
		/// 注释
		/// </summary>
		public string Comment;

		/// <summary>
		/// 名字
		/// </summary>
		public string Name;

		/// <summary>
		/// 包含的类
		/// </summary>
		public List<QClassDefine> Classes = new List<QClassDefine> ();

		/// <summary>
		/// 生成的文件名
		/// </summary>
		public string FileName;


		/// <summary>
		/// 生成的路径
		/// </summary>
		public string GenerateDir = "";
	}

	/// <summary>
	/// 类定义
	/// </summary>
	public class QClassDefine 
	{
		/// <summary>
		/// 注释
		/// </summary>
		public string Comment;

		/// <summary>
		/// 类名字
		/// </summary>
		public string Name;

		/// <summary>
		/// 变量
		/// </summary>
		public List<QVariable> Variables = new List<QVariable>();

		/// <summary>
		/// 属性定义 
		/// </summary>
		public List<QProperty> Properties = new List<QProperty> ();
	}

	public class QCodeGenerator 
	{
		public static void Generate(QNamespaceDefine nameSpace) 
		{
			IOUtils.CreateDirIfNotExists (nameSpace.GenerateDir);

			var compileUnit = new CodeCompileUnit ();
			var codeNameSpace = new CodeNamespace (nameSpace.Name);
			compileUnit.Namespaces.Add (codeNameSpace);

			foreach (var classDefine in nameSpace.Classes) 
			{
				var codeType = new CodeTypeDeclaration (classDefine.Name);
				codeNameSpace.Types.Add (codeType);

				AddDocumentComment (codeType.Comments, classDefine.Comment);

				foreach (var variable in classDefine.Variables) 
				{
					AddVariable (codeType, variable);
				}

				foreach (var property in classDefine.Properties) 
				{
					AddProperty (codeType, property);
				}

			}
			var provider = new CSharpCodeProvider ();
			var options = new CodeGeneratorOptions ();
			options.BlankLinesBetweenMembers = false;
//			options.BracingStyle = "Block";
			options.BracingStyle = "C";
			StreamWriter writer = new StreamWriter(File.Open (Path.GetFullPath (nameSpace.GenerateDir + Path.DirectorySeparatorChar + nameSpace.FileName), FileMode.Create));

			provider.GenerateCodeFromCompileUnit (compileUnit, writer, options);
			writer.Close ();
			AssetDatabase.Refresh ();
		}
			
		/// <summary>
		/// 添加注释
		/// </summary>
		static void AddDocumentComment(CodeCommentStatementCollection comments,string commentContent) 
		{
			if (!string.IsNullOrEmpty (commentContent)) 
			{
				comments.Add (new CodeCommentStatement (new CodeComment ("<summary>", true)));
				comments.Add (new CodeCommentStatement (new CodeComment (commentContent, true)));
				comments.Add (new CodeCommentStatement (new CodeComment ("</summary>", true)));
			}
		}

		/// <summary>
		/// 添加变量
		/// </summary>
		static void AddVariable(CodeTypeDeclaration codeType,QVariable variable) 
		{
			CodeMemberField nameField = new CodeMemberField ();

			AddDocumentComment (nameField.Comments, variable.Comment);

			switch (variable.AccessLimit) 
			{
				case QAccessLimit.Public:
					nameField.Attributes = MemberAttributes.Public;
					break;
				case QAccessLimit.Private:
					nameField.Attributes = MemberAttributes.Private;
					break;
			}

			switch (variable.CompileType) 
			{
				case QCompileType.Const:
					nameField.Attributes |= MemberAttributes.Const;
					break;
				case QCompileType.Static:
					nameField.Attributes |= MemberAttributes.Static;
					break;
			}

			nameField.Name = variable.Name;
			nameField.Type = variable.Type;
			nameField.InitExpression = new CodePrimitiveExpression (variable.Value);

			codeType.Members.Add (nameField);
		}

		/// <summary>
		/// get Property
		/// https://msdn.microsoft.com/zh-cn/library/system.codedom.codememberproperty?cs-save-lang=1&cs-lang=csharp#code-snippet-1
		/// </summary>
		static void AddProperty(CodeTypeDeclaration codeType,QProperty property) 
		{
			CodeMemberProperty getProperty = new CodeMemberProperty();

			AddDocumentComment (getProperty.Comments, property.Comment);

			switch (property.AccessLimit) 
			{
				case QAccessLimit.Public:
					getProperty.Attributes = MemberAttributes.Public;
					break;
				case QAccessLimit.Private:
					getProperty.Attributes = MemberAttributes.Private;
					break;
			}

			switch (property.CompileType) 
			{
				case QCompileType.Const:
					getProperty.Attributes |= MemberAttributes.Const;
					break;
				case QCompileType.Static:
					getProperty.Attributes |= MemberAttributes.Static;
					break;
			}

			getProperty.Name = property.Name;
			getProperty.Type = property.Type;
			getProperty.GetStatements.Add( new CodeMethodReturnStatement( new CodeFieldReferenceExpression(null,property.GetReturnCode) ) );
		
			codeType.Members.Add (getProperty);
		}
	}
}