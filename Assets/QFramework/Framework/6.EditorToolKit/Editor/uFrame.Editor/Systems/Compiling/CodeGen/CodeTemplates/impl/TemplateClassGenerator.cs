using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using QF.GraphDesigner;
using Invert.Data;

namespace QF.GraphDesigner
{
    public class TemplateClassGenerator<TData, TTemplateType> : CodeGenerator, ITemplateClassGenerator
        where TData : class, IDataRecord
        where TTemplateType : class, IClassTemplate<TData>, new()
    {
        private CodeTypeDeclaration _decleration;
        private TemplateContext<TData> _templateContext;
        private TTemplateType _templateClass;
        private TemplateClass _templateAttribute;
        private KeyValuePair<MethodInfo, GenerateConstructor>[] _templateConstructors;
        private KeyValuePair<MethodInfo, GenerateMethod>[] _templateMethods;
        private KeyValuePair<PropertyInfo, GenerateProperty>[] _templateProperties;
        private List<TemplateMemberResult> _results;
        public Predicate<IDiagramNodeItem> ItemFilter { get; set; }

        public Type TemplateType
        {
            get { return typeof(TTemplateType); }
        }

       
        public override string Filename
        {
            get
            {
                var template = new TTemplateType { Ctx = new TemplateContext<TData>(TemplateType) { DataObject = Data,IsDesignerFile = IsDesignerFile} };
                var customFilenameTemplate = template as ITemplateCustomFilename;
                if (customFilenameTemplate != null)
                {
                    return customFilenameTemplate.Filename;
                }
                if (IsDesignerFile)
                {
                    
                    return template.OutputPath + ".designer.cs";
                }

                return Path.Combine(template.OutputPath, ClassName(Data as IDiagramNodeItem) + ".cs");
            }
            set { base.Filename = value; }
        }

        public TTemplateType TemplateClass
        {
            get
            {
                if (_templateClass == null)
                {
                    _templateClass = new TTemplateType();
                    _templateClass.Ctx = TemplateContext;

                }
                return _templateClass;
            }
            set { _templateClass = value; }
        }

        public TemplateContext<TData> TemplateContext
        {
            get
            {
                if (_templateContext == null)
                {
                    _templateContext = CreateTemplateContext();
                }
                return _templateContext;
            }
            set { _templateContext = value; }
        }

        protected virtual TemplateContext<TData> CreateTemplateContext()
        {
            var context = new TemplateContext<TData>(TemplateType);
            context.Generator = this;
            context.DataObject = Data;
            context.Namespace = Namespace;
            context.CurrentDeclaration = Decleration;
            context.IsDesignerFile = IsDesignerFile;
            context.ItemFilter = ItemFilter;
            return context;
        }

        public virtual bool IsDesignerFileOnly
        {
            get { return false; }
        }

        public TData Data
        {
            get { return ObjectData as TData; }
            set { ObjectData = value; }
        }

        public override bool IsValid()
        {
            
            var template = new TTemplateType {Ctx = new TemplateContext<TData>(TemplateType) {DataObject = Data}};
            if (template is IOnDemandTemplate) return false;
            return template.CanGenerate;
        }

        public CodeTypeDeclaration Decleration
        {
            get
            {
                return _decleration;
            }
            set { _decleration = value; }
        }

        public virtual string ClassNameFormat
        {
            get { return Attribute.ClassNameFormat; }
        }

        public virtual string ClassName(IDiagramNodeItem node)
        {
            if (node == null) return string.Empty;
            if (!string.IsNullOrEmpty(ClassNameFormat))
            {
                return Regex.Replace(string.Format(ClassNameFormat, node.Name), @"[^a-zA-Z0-9_\.]+", "");
            }

            var typeNode = node as IClassTypeNode;
            if (typeNode != null)
            {
                return Regex.Replace(typeNode.ClassName, @"[^a-zA-Z0-9_\.]+", "");
            }
            return Regex.Replace(node.Name, @"[^a-zA-Z0-9_\.]+", "");
        }

        public virtual string ClassNameBase(IDiagramNodeItem node)
        {
            if (node == null) return string.Empty;
            if (IsDesignerFileOnly)
                return ClassName(node);

            return ClassName(node) + "Base";
        }

        public TemplateClass Attribute
        {
            get
            {
                return _templateAttribute ?? (_templateAttribute = typeof(TTemplateType).GetCustomAttributes(typeof(TemplateClass), true)
                .OfType<TemplateClass>()
                .FirstOrDefault());
            }
        }

        public override Type GeneratorFor
        {
            get { return typeof(TData); }
            set
            {

            }
        }

        public TemplateContext Context
        {
            get { return TemplateContext; }
        }

        public string[] FilterToMembers { get; set; }

        public override void Initialize(CodeFileGenerator codeFileGenerator)
        {
            base.Initialize(codeFileGenerator);
            //if (!string.IsNullOrEmpty(TemplateType.Namespace))
            //    TryAddNamespace(TemplateType.Namespace);
            Decleration = TemplateType.ToClassDecleration();

            var inheritable = Data as GenericInheritableNode;
            if (!Attribute.AutoInherit)
            {
                inheritable = null;
            }
            if (IsDesignerFile && Attribute.Location != TemplateLocation.DesignerFile)
            {
                Decleration.Name = ClassNameBase(Data as IDiagramNodeItem);
                if (inheritable != null && inheritable.BaseNode != null)
                {

                    Decleration.BaseTypes.Clear();
                    Decleration.BaseTypes.Add(ClassName(inheritable.BaseNode));
                }

            }
            else 
            {
                Decleration.Name = ClassName(Data as IDiagramNodeItem);
                if (Attribute.Location != TemplateLocation.DesignerFile)
                {
                    Decleration.BaseTypes.Clear();
                    Decleration.BaseTypes.Add(ClassNameBase(Data as IDiagramNodeItem));
                }

            }

            Namespace.Types.Add(Decleration);

            ProcessTemplate();
            return; // Skip the stuff below for now

            if (IsDesignerFile)
            {
                // base.Initialize(fileGenerator);

                if (IsDesignerFile)
                {
                    InitializeDesignerFile();
                }
                else
                {
                    InitializeEditableFile();
                }
            }

        }

        public IClassTemplate Template
        {
            get
            {
                return TemplateClass;
            }
        }


        protected virtual void InitializeEditableFile()
        {

        }

        protected virtual void InitializeDesignerFile()
        {

        }
        
        public void ProcessTemplate()
        {
            // Initialize the template
            TemplateContext.Iterators.Clear();
            TemplateClass.TemplateSetup();

            foreach (
                var item in
                    TemplateClass.GetType()
                        .GetCustomAttributes(typeof (TemplateAttribute), true)
                        .OfType<TemplateAttribute>().OrderBy(p=>p.Priority))
            {
                item.Modify(TemplateClass,null,TemplateContext);
            }

            var initializeMethods = TemplateClass.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(p=>p.IsDefined(typeof(TemplateSetup),true)).ToArray();
            foreach (var item in initializeMethods)
            {
                item.Invoke(TemplateClass, null);
            }

            InvertApplication.SignalEvent<ICodeTemplateEvents>(_ => _.TemplateGenerating(TemplateClass, TemplateContext));
          

            foreach (var templateProperty in TemplateProperties)
            {
                if (FilterToMembers != null && !FilterToMembers.Contains(templateProperty.Key.Name)) continue;
                foreach (var item in TemplateContext.RenderTemplateProperty(TemplateClass, templateProperty))
                {
                    Results.Add(new TemplateMemberResult(this,templateProperty.Key,templateProperty.Value,item, Decleration));
                }
            }

            foreach (var templateMethod in TemplateMethods)
            {
                if (FilterToMembers != null && !FilterToMembers.Contains(templateMethod.Key.Name)) continue;
                foreach (var item in TemplateContext.RenderTemplateMethod(TemplateClass, templateMethod))
                {
                    Results.Add(new TemplateMemberResult(this, templateMethod.Key, templateMethod.Value, item, Decleration));
                }
            }

            foreach (var templateConstructor in TemplateConstructors)
            {
                if (FilterToMembers != null && !FilterToMembers.Contains(templateConstructor.Key.Name)) continue;
                foreach (var item in TemplateContext.RenderTemplateConstructor(TemplateClass, templateConstructor))
                {
                    Results.Add(new TemplateMemberResult(this, templateConstructor.Key, templateConstructor.Value, item, Decleration));
                }
            }
	        var postMethods = TemplateClass.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(p => p.IsDefined(typeof(TemplateComplete), true)).ToArray();
            foreach (var item in postMethods)
            {
                item.Invoke(TemplateClass, null);
            }
        
            var list = new List<CodeNamespaceImport>();
            foreach (var item in TemplateContext.Namespace.Imports)
            {
                list.Add((CodeNamespaceImport)item);
            }
            TemplateContext.Namespace.Imports.Clear();
            foreach (var item in list.OrderBy(p=>p.Namespace))
            {
                TemplateContext.Namespace.Imports.Add(item);
            }
          
        }

        public List<TemplateMemberResult> Results
        {
            get { return _results ?? (_results = new List<TemplateMemberResult>()); }
            set { _results = value; }
        }

        public void Initialize(CodeFileGenerator codeFileGenerator, Predicate<IDiagramNodeItem> itemFilter = null)
        {
            
        }

        public KeyValuePair<MethodInfo, GenerateConstructor>[] TemplateConstructors
        {
            get { return _templateConstructors ?? (_templateConstructors = TemplateType.GetMethodsWithAttribute<GenerateConstructor>(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).ToArray()); }
            set { _templateConstructors = value; }
        }

        public KeyValuePair<MethodInfo, GenerateMethod>[] TemplateMethods
        {
            get { return _templateMethods ?? (_templateMethods = TemplateType.GetMethodsWithAttribute<GenerateMethod>(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).ToArray()); }
            set { _templateMethods = value; }
        }

        public KeyValuePair<PropertyInfo, GenerateProperty>[] TemplateProperties
        {
            get { return _templateProperties ?? (_templateProperties = TemplateType.GetPropertiesWithAttributeByType<GenerateProperty>(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).ToArray()); }
            set { _templateProperties = value; }
        }
    }

    public class TemplateContext
    {

        public string ProcessType(Type t)
        {
            var genericParameter = t.GetGenericParameter();
            if (genericParameter != null)
            {
                var gt = ProcessType(genericParameter);
                if (gt == null) return null;
                return string.Format("{0}<{1}>", t.Name.Replace("`1",""), gt);
            }
            if (typeof(_TEMPLATETYPE_).IsAssignableFrom(t))
            {
                var type = Activator.CreateInstance(t) as _TEMPLATETYPE_;
                return type.TheType(this);
            }

            return null;

        }
        public object GetTemplateProperty(object templateInstance, string propertyName)
        {
            var property = templateInstance.GetType().GetProperty(propertyName);
            if (property == null)
            {
                property = (Item ?? DataObject).GetType()
                    .GetProperty(propertyName);

                if (property == null)
                {
                    throw new TemplateException(string.Format("Template Property Not Found {0}", propertyName));
                }

                return property.GetValue(Item ?? DataObject, null);
            }
            else
            {
                return property.GetValue(templateInstance, null);
            }
        }

        // TODO Remove this and add the Generator collections to here
        public ITemplateClassGenerator Generator { get; set; }

        protected List<TemplateMemberResult> Results
        {
            get { return Generator.Results; }
        }


        private Stack<CodeStatementCollection> _contextStatements;
        private CodeStatementCollection _currentStatements;
        private IDiagramNodeItem _item;
        public bool IsDesignerFile { get; set; }
        public IDataRecord DataObject { get; set; }

        public IDiagramNodeItem NodeItem
        {
            get { return DataObject as IDiagramNodeItem; }
        }
        public IDiagramNodeItem Item
        {
            get { return _item ?? NodeItem; }
            set { _item = value; }
        }

        public ITypedItem TypedItem
        {
            get { return Item as ITypedItem; }
        }

        public Predicate<IDiagramNodeItem> ItemFilter { get; set; }

        public CodeTypeMember CurrentMember { get; set; }

        public CodeMemberEvent CurrentEvent
        {
            get { return CurrentMember as CodeMemberEvent; }
        }

        public CodeMemberProperty CurrentProperty
        {
            get { return CurrentMember as CodeMemberProperty; }
        }

        public CodeMemberMethod CurrentMethod
        {
            get { return CurrentMember as CodeMemberMethod; }
        }
        public GenerateMethod CurrentMethodAttribute
        {
            get { return CurrentAttribute as GenerateMethod; }
        }
        public GenerateProperty CurrentPropertyAttribute
        {
            get { return CurrentAttribute as GenerateProperty; }
        }
        public GenerateConstructor CurrentConstructorAttribute
        {
            get { return CurrentAttribute as GenerateConstructor; }
        }
        public GenerateMember CurrentAttribute { get; set; }
        public CodeTypeDeclaration CurrentDeclaration { get; set; }
        public CodeNamespace Namespace { get; set; }
        public void TryAddNamespace(string ns)
        {
            if (Namespace == null) return;
            if (string.IsNullOrEmpty(ns) || string.IsNullOrEmpty(ns.Trim())) return;
            foreach (CodeNamespaceImport n in Namespace.Imports)
            {
                if (n.Namespace == ns)
                    return;
            }
            Namespace.Imports.Add(new CodeNamespaceImport(ns));
        }

        public CodeStatementCollection CurrentStatements
        {
            get
            {
                if (ContextStatements.Count < 1)
                    return null;
                return ContextStatements.Peek();
            }
            //set
            //{
            //    _currentStatements = value;
                
            //}
        }

        public void _(string formatString, params object[] args)
        {
            if (CurrentStatements == null)
            {
                CurrentDeclaration.Members.Add(new CodeSnippetTypeMember(string.Format(formatString, args)));
                return;
            }
            CurrentStatements._(formatString, args);
        }
        public void _comment(string formatString, params object[] args)
        {
            CurrentStatements.Add(new CodeCommentStatement(string.Format(formatString, args)));
        }
        public CodeConditionStatement _if(string formatString, params object[] args)
        {
            return CurrentStatements._if(formatString, args);
        }

        public void AddInterface(object type, params object[] args)
        {
            CurrentDeclaration.BaseTypes.Add(type.ToCodeReference(args));
        }
        public void SetBaseType(object type, params object[] args)
        {
            CurrentDeclaration.BaseTypes.Clear();
            CurrentDeclaration.BaseTypes.Add(type.ToCodeReference(args));
        }
        public void SetBaseTypeArgument(object type, params object[] args)
        {
            CurrentDeclaration.BaseTypes[0].TypeArguments.Clear();
            CurrentDeclaration.BaseTypes[0].TypeArguments.Add(type.ToCodeReference(args));
        }
        public void SetTypeArgument(object type, params object[] args)
        {


            if (CurrentProperty != null)
            {
                CurrentProperty.SetTypeArgument(type, args);
            }

            else if (CurrentMethod != null)
            {
                CurrentMethod.ReturnType.TypeArguments.Clear();
                CurrentMethod.ReturnType.TypeArguments.Add(type.ToCodeReference(args));
            }
        }
        public void SetType(object type, params object[] args)
        {
            if (CurrentProperty != null)
            {
                CurrentProperty.Type = type.ToCodeReference(args);
            }

            else if (CurrentMethod != null)
            {
                CurrentMethod.ReturnType = type.ToCodeReference(args);
            }
            else
            {

            }


        }

        public void LazyGet(string fieldName, string createExpression, params object[] args)
        {
            _if("{0}==null", fieldName)
                .TrueStatements._("{0} = {1}", fieldName, string.Format(createExpression, args).ToString());
            _("return {0}", fieldName);
        }

        protected Stack<CodeStatementCollection> ContextStatements
        {
            get { return _contextStatements ?? (_contextStatements = new Stack<CodeStatementCollection>()); }
            set { _contextStatements = value; }
        }

        public void PushStatements(CodeStatementCollection codeStatementCollection)
        {
            ContextStatements.Push(codeStatementCollection);
        }

        public void PopStatements()
        {
            ContextStatements.Pop();
        }



        public virtual void AddMemberIterator(string name, Func<object, IEnumerable> func)
        {
        
        }
    }
}