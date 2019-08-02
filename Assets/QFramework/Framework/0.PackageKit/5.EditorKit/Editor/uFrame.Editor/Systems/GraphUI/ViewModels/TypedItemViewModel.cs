using System.Collections.Generic;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public abstract class TypedItemViewModel : ItemViewModel<ITypedItem>
    {
        public static Dictionary<string, string> TypeNameAliases = new Dictionary<string, string>()
    {
        {"Int32","int"},
        {"Boolean","bool"},
        {"Char","char"},
        {"Decimal","decimal"},
        {"Double","double"},
        {"Single","float"},
        {"String","string"},
        {"System.Int32","int"},
        {"System.Boolean","bool"},
        {"System.Char","char"},
        {"System.Decimal","decimal"},
        {"System.Double","double"},
        {"System.Single","float"},
        {"System.String","string"},
    };
        public static string TypeAlias(string typeName)
        {
            if (typeName == null)
            {
                return " ";
            }
            if (TypeNameAliases.ContainsKey(typeName))
            {
                return TypeNameAliases[typeName];
            }
            return typeName;
        }

        protected TypedItemViewModel(ITypedItem viewModelItem, DiagramNodeViewModel nodeViewModel)
            : base(nodeViewModel)
        {
            DataObject = viewModelItem;
        }

        public override bool IsEditable
        {
            get { return !Data.Precompiled; }
            set { base.IsEditable = value; }
        }

        public IMemberInfo MemberInfo
        {
            get { return Data as IMemberInfo; }
        }
        public virtual string RelatedType
        {
            get
            {
                if (MemberInfo != null)
                {
                    var typeName = TypeAlias(MemberInfo.MemberType.TypeName);
                    if (string.IsNullOrEmpty(typeName))
                    {
                        return "[void]";
                    }
                    return typeName;
                }
               
                return Data.RelatedTypeName;//ElementDataBase.TypeAlias(Data.RelatedType);
            }
            set
            {
                Data.RelatedType = value;
            }
        }

        public virtual string TypeLabel
        {
            get { return RelatedType; }
        }

        public void ShowSelectionListWindow()
        {
            EndEditing();
            InvertApplication.Execute(new SelectTypeCommand()
            {
                PrimitiveOnly = false,
                AllowNone = false,
                IncludePrimitives = true,
                Item = this.DataObject as ITypedItem,
            });
            // TODO 2.0 Typed Item Selection Window
            // This was in the drawer re-implement

            //if (!this.ItemViewModel.Enabled) return;
            //if (TypedItemViewModel.Data.Precompiled) return;
            //var commandName = ViewModelObject.DataObject.GetType().Name.Replace("Data", "") + "TypeSelection";

            //var command = InvertGraphEditor.Container.Resolve<IEditorCommand>(commandName);
        }
    }
}