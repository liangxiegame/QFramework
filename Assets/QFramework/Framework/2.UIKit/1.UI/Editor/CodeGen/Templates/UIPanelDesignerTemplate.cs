/****************************************************************************
 * Copyright (c) 2019.1 liangxie
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

using System.CodeDom;
using QFramework.GraphDesigner;

namespace QFramework
{
    [TemplateClass(TemplateLocation.DesignerFile)]
    [RequiresNamespace("UnityEngine")]
    [RequiresNamespace("UnityEngine.UI")]
    [AsPartial]
    public class UIPanelDesignerTemplate : IClassTemplate<PanelCodeData>,ITemplateCustomFilename
    {
        public string OutputPath  { get; private set; }
        public bool   CanGenerate
        {
            get { return true; }
        }

        public void TemplateSetup()
        {
            Ctx.CurrentDeclaration.BaseTypes.Clear();
            Ctx.CurrentDeclaration.Name = Ctx.Data.PanelName;

            var dataTypeName = Ctx.Data.PanelName + "Data";

            var nameField = new CodeMemberField(typeof(string), "NAME")
            {
                Attributes = MemberAttributes.Const | MemberAttributes.Public,
                InitExpression = Ctx.Data.PanelName.ToCodeSnippetExpression()
            };
            Ctx.CurrentDeclaration.Members.Add(nameField);

            Ctx.Data.MarkedObjInfos.ForEach(info =>
            {
                var field = Ctx.CurrentDeclaration._public_(info.MarkObj.ComponentName, info.Name);

                if (info.MarkObj.Comment.IsNotNullAndEmpty())
                {
                    field.Comments.Add(new CodeCommentStatement(info.MarkObj.Comment));
                }

                field.CustomAttributes.Add(new CodeAttributeDeclaration("SerializeField".ToCodeReference()));
            });

            var data = Ctx.CurrentDeclaration._private_(dataTypeName, "mPrivateData");
            data.InitExpression = new CodeSnippetExpression("null");
        }

        [GenerateMethod, AsOverride]
        protected void ClearUIComponents()
        {
            Ctx.Data.MarkedObjInfos.ForEach(info => { Ctx._("{0} = null", info.Name); });
            Ctx._("mData = null");
        }

        [GenerateProperty]
        public string mData
        {
            get
            {
                var dataTypeName = Ctx.Data.PanelName + "Data";
                Ctx._("return mPrivateData ?? (mPrivateData = new {0}())",dataTypeName);
                Ctx.CurrentProperty.Type = dataTypeName.ToCodeReference();
                return dataTypeName;
            }
            set
            {
                Ctx._("mUIData = value");
                Ctx._("mPrivateData = value");
            }
        }
        public TemplateContext<PanelCodeData> Ctx { get; set; }
        
        public string Filename { get; private set; }
    }
}