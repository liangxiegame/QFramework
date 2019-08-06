using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class TemplateContext<TData> : TemplateContext
    {
        private Dictionary<string, Func<TData, IEnumerable>> _iterators;
        private Dictionary<string, Func<TData, bool>> _conditions;

        public Type TemplateType { get; set; }

        public TemplateContext(Type templateType)
        {
            TemplateType = templateType;
        }

        public CodeAttributeDeclaration AddAttribute(object type, params string[] parameters)
        {
            var attribute = new CodeAttributeDeclaration(type.ToCodeReference(),
                parameters.Select(p => new CodeAttributeArgument(new CodeSnippetExpression(p))).ToArray());
            if (CurrentMember == null)
            {
                if (CurrentConstructor == null)
                {
                    CurrentDeclaration.CustomAttributes.Add(attribute);
                }
                else
                {
                    CurrentConstructor.CustomAttributes.Add(attribute);
                }

            }
            else
            {
                CurrentMember.CustomAttributes.Add(attribute);
            }

            return attribute;

        }

        public TData Data
        {
            get { return (TData)DataObject; }
        }

        public IEnumerable CurrentIterator { get; set; }

        public Action<TemplateContext> CurrentIteratorAction { get; set; }
        public Func<TemplateContext> CurrentIteratorCreateContext { get; set; }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        public Dictionary<string, Func<TData, IEnumerable>> Iterators
        {
            get { return _iterators ?? (_iterators = new Dictionary<string, Func<TData, IEnumerable>>()); }
            set { _iterators = value; }
        }

        public CodeConstructor CurrentConstructor { get; set; }

        public Dictionary<string, Func<TData, bool>> Conditions
        {
            get { return _conditions ?? (_conditions = new Dictionary<string, Func<TData, bool>>()); }
            set { _conditions = value; }
        }

        public void AddIterator(string memberName, Func<TData, IEnumerable> iterator)
        {
            Iterators.Add(memberName, iterator);
        }
        public void AddCondition(string memberName, Func<TData, bool> condition)
        {

            AddIterator(memberName, _ => condition(_) ? Enumerable.Repeat(_, 1) : Enumerable.Empty<TData>());
        }

        public T ItemAs<T>()
        {
            return (T)Item;
        }

        public IEnumerable<CodeConstructor> RenderTemplateConstructor(object instance, string methodName)
        {
            return RenderTemplateConstructor(instance, instance.GetType().GetMethod(methodName));
        }

        public IEnumerable<CodeConstructor> RenderTemplateConstructor(object instance, MethodInfo info)
        {
            return RenderTemplateConstructor(instance, new KeyValuePair<MethodInfo, GenerateConstructor>(info, info.GetCustomAttributes(typeof(GenerateConstructor), true).OfType<GenerateConstructor>().FirstOrDefault()));
        }
        public IEnumerable<CodeConstructor> RenderTemplateConstructor(object instance, KeyValuePair<MethodInfo, GenerateConstructor> templateConstructor)
        {
            CurrentAttribute = templateConstructor.Value;
            var attributes = templateConstructor.Key.GetCustomAttributes(typeof(TemplateAttribute), true).OfType<TemplateAttribute>().OrderBy(p => p.Priority).ToArray();

            bool success = true;
            foreach (var attribute in attributes)
            {
                if (!attribute.CanGenerate(instance, templateConstructor.Key, this))
                {
                    success = false;
                }
            }
            if (!success)
            {
                yield break;
            }


            // Default to designer file only
            if (!attributes.OfType<Inside>().Any())
            {
                if (!IsDesignerFile) yield break;
            }

            if (Iterators.ContainsKey(templateConstructor.Key.Name))
            {
                var iterator = Iterators[templateConstructor.Key.Name];
                var items = iterator(Data).OfType<IDiagramNodeItem>().ToArray();

                foreach (var item in items)
                {
                    if (ItemFilter != null && !ItemFilter(item))
                        continue;
                    Item = item;
                    yield return RenderConstructor(instance, templateConstructor, item);
                }
            }
            else
            {
                Item = Data as IDiagramNodeItem;
                if (ItemFilter != null && !ItemFilter(Item))
                    yield break;
                yield return RenderConstructor(instance, templateConstructor, Data as IDiagramNodeItem);
            }
        }
        

        public IEnumerable<CodeMemberProperty> RenderTemplateProperty(object instance, string propertyName)
        {
            return RenderTemplateProperty(instance, instance.GetType().GetProperty(propertyName));
        }

        public IEnumerable<CodeMemberProperty> RenderTemplateProperty(object instance, PropertyInfo info)
        {
            return RenderTemplateProperty(instance, new KeyValuePair<PropertyInfo, GenerateProperty>(info, info.GetCustomAttributes(typeof(GenerateProperty), true).OfType<GenerateProperty>().FirstOrDefault()));
        }
        public IEnumerable<CodeMemberProperty> RenderTemplateProperty(object instance, KeyValuePair<PropertyInfo, GenerateProperty> templateProperty)
        {
            CurrentAttribute = templateProperty.Value;
            var attributes = templateProperty.Key.GetCustomAttributes(typeof (TemplateAttribute), true).OfType<TemplateAttribute>().OrderBy(p=>p.Priority).ToArray();

            bool success = true;
            foreach (var attribute in attributes)
            {
                if (!attribute.CanGenerate(instance, templateProperty.Key, this))
                {
                    success = false;
                }
            }
            if (!success)
            {
                yield break;
            }

            // Default to designer file only
            if (!attributes.OfType<Inside>().Any())
            {
                if (!IsDesignerFile) yield break;
            }

            if (Iterators.ContainsKey(templateProperty.Key.Name))
            {


                var iterator = Iterators[templateProperty.Key.Name];
                var items = iterator(Data).OfType<IDiagramNodeItem>().ToArray();

                foreach (var item in items)
                {
                    if (ItemFilter != null && !ItemFilter(item))
                        continue;
                    Item = item;
                  
                    var domObject = RenderProperty(instance, templateProperty);
                    foreach (var attribute in attributes)
                    {
                        attribute.Modify(instance,templateProperty.Key,this);
                    }
                    CurrentDeclaration.Members.Add(domObject);
                    yield return domObject;
                    InvertApplication.SignalEvent<ICodeTemplateEvents>(_ => _.PropertyAdded(instance, this, domObject));
                }
                Item = null;
            }
            else
            {
                Item = Data as IDiagramNodeItem;
                if (ItemFilter != null && !ItemFilter(Item))
                    yield break;
                var domObject = RenderProperty(instance, templateProperty);
                foreach (var attribute in attributes)
                {
                    attribute.Modify(instance, templateProperty.Key, this);
                }
                CurrentDeclaration.Members.Add(domObject);
                yield return domObject;
                InvertApplication.SignalEvent<ICodeTemplateEvents>(_=>_.PropertyAdded(instance, this, domObject));
                Item = null;
            }
        }
        public IEnumerable<CodeMemberMethod> RenderTemplateMethod(object instance, string methodName)
        {
            return RenderTemplateMethod(instance, instance.GetType().GetMethod(methodName));
        }

        public IEnumerable<CodeMemberMethod> RenderTemplateMethod(object instance, MethodInfo info)
        {
            return RenderTemplateMethod(instance, new KeyValuePair<MethodInfo, GenerateMethod>(info, info.GetCustomAttributes(typeof(GenerateMethod), true).OfType<GenerateMethod>().FirstOrDefault()));
        }
        
        public IEnumerable<CodeMemberMethod> RenderTemplateMethod(object instance, KeyValuePair<MethodInfo, GenerateMethod> templateMethod)
        {
            CurrentAttribute = templateMethod.Value;
            var attributes = templateMethod.Key.GetCustomAttributes(typeof(TemplateAttribute), true).OfType<TemplateAttribute>().OrderBy(p => p.Priority).ToArray();

            bool success = true;
            foreach (var attribute in attributes)
            {
                if (!attribute.CanGenerate(instance, templateMethod.Key, this))
                {
                    success = false;
                }
            }
            if (!success)
            {
                yield break;
            }

            // Default to designer file only
            if (!attributes.OfType<Inside>().Any())
            {
                if (!IsDesignerFile) yield break;
            }
            // if (templateMethod.Value.Location == TemplateLocation.DesignerFile &&
            //     templateMethod.Value.Location != TemplateLocation.Both && !IsDesignerFile) yield break;
            // if (templateMethod.Value.Location == TemplateLocation.EditableFile &&
            //     templateMethod.Value.Location != TemplateLocation.Both && IsDesignerFile) yield break;

            // var forEachAttribute =
            //templateMethod.Key.GetCustomAttributes(typeof(TemplateForEach), true).FirstOrDefault() as
            //    TemplateForEach;

            // var iteratorName = templateMethod.Key.Name;
            // if (forEachAttribute != null)
            // {
            //     iteratorName = forEachAttribute.IteratorProperty;
            //     AddIterator(templateMethod.Key.Name,
            //         delegate(TData arg1)
            //         {
            //             return CreateIterator(instance, iteratorName, arg1);
            //         });
            // }

            if (Iterators.ContainsKey(templateMethod.Key.Name))
            {
                var iterator = Iterators[templateMethod.Key.Name];
                var items = iterator(Data).OfType<IDiagramNodeItem>().ToArray();

                foreach (var item in items)
                {
                    Item = item;
                    if (ItemFilter != null && !ItemFilter(item))
                        continue;
                    var result = RenderMethod(instance, templateMethod, item);
                    foreach (var attribute in attributes)
                    {
                        attribute.Modify(instance, templateMethod.Key, this);
                    }
                    yield return result;

                }
                Item = null;
            }
            else
            {
                Item = Data as IDiagramNodeItem;
                if (ItemFilter != null && !ItemFilter(Item))
                    yield break;
                var result = RenderMethod(instance, templateMethod, Data as IDiagramNodeItem);
                foreach (var attribute in attributes)
                {
                    attribute.Modify(instance, templateMethod.Key, this);
                }
                yield return result;
                Item = null;
            }
        }

        private static IEnumerable CreateIterator(object instance, string iteratorName, TData arg1)
        {
            var property = instance.GetType().GetProperty(iteratorName);
            if (property == null && arg1 != null)
            {
                property = arg1.GetType().GetProperty(iteratorName);
                return property.GetValue(arg1, null) as IEnumerable;
            }
            if (property != null)
            {
                return property.GetValue(instance, null) as IEnumerable;
            }

            throw new Exception(string.Format("ForEach on property '{0}' could not be found on the template, or the node.",
                iteratorName));
        }

        protected CodeConstructor RenderConstructor(object instance, KeyValuePair<MethodInfo, GenerateConstructor> templateMethod, IDiagramNodeItem data)
        {
            var info = templateMethod.Key;
            var dom = templateMethod.Key.ToCodeConstructor();
            CurrentAttribute = templateMethod.Value;
            CurrentConstructor = dom;
            PushStatements(dom.Statements);
            //            CurrentStatements = dom.Statements;
            var args = new List<object>();
            var parameters = info.GetParameters();
            foreach (var arg in parameters)
            {
                args.Add(GetDefault(arg.ParameterType));
            }
            foreach (var item in templateMethod.Value.BaseCallArgs)
            {
                dom.BaseConstructorArgs.Add(new CodeSnippetExpression(item));
            }


            info.Invoke(instance, args.ToArray());
            PopStatements();
            CurrentDeclaration.Members.Add(dom);
            InvertApplication.SignalEvent<ICodeTemplateEvents>(_ => _.ConstructorAdded(instance, this, dom));
            return dom;
        }
        protected CodeMemberProperty RenderProperty(object instance, KeyValuePair<PropertyInfo, GenerateProperty> templateProperty)
        {
            var domObject = TemplateType.PropertyFromTypeProperty(templateProperty.Key.Name);
            CurrentMember = domObject;
            CurrentAttribute = templateProperty.Value;
            //if (templateProperty.Value.AutoFill != AutoFillType.None)
            //{
            //    domObject.Name = string.Format(templateProperty.Value.NameFormat, this.Item.Name.Clean());
            //}

            //if (templateProperty.Value.AutoFill == AutoFillType.NameAndType ||
            //    templateProperty.Value.AutoFill == AutoFillType.NameAndTypeWithBackingField)
            //{
            //    var typedItem = Item as ITypedItem;
            //    if (typedItem != null)
            //    {
            //        if (domObject.Type.TypeArguments.Count > 0)
            //        {
            //            domObject.Type.TypeArguments.Clear();
            //            domObject.Type.TypeArguments.Add(typedItem.RelatedTypeName);
            //        }
            //        else
            //        {
            //            domObject.Type = new CodeTypeReference(typedItem.RelatedTypeName);
            //        }
            //    }
            //}
            PushStatements(domObject.GetStatements);
            templateProperty.Key.GetValue(instance, null);
            PopStatements();
            if (templateProperty.Key.CanWrite)
            {
                PushStatements(domObject.SetStatements);
                //CurrentStatements = domObject.SetStatements;
                templateProperty.Key.SetValue(instance,
                    GetDefault(templateProperty.Key.PropertyType),
                    null);
                PopStatements();
            }

            if (!IsDesignerFile && domObject.Attributes != MemberAttributes.Final && templateProperty.Value.Location == TemplateLocation.Both)
            {
                domObject.Attributes |= MemberAttributes.Override;
            }
            
            return domObject;
        }
        protected CodeMemberMethod RenderMethod(object instance, KeyValuePair<MethodInfo, GenerateMethod> templateMethod, IDiagramNodeItem data)
        {
            MethodInfo info;
            var dom = TemplateType.MethodFromTypeMethod(templateMethod.Key.Name, out info, false);
            
            CurrentMember = dom;
            CurrentAttribute = templateMethod.Value;
            PushStatements(dom.Statements);
            
            var args = new List<object>();
            var parameters = info.GetParameters();
            foreach (var arg in parameters)
            {
                args.Add(GetDefault(arg.ParameterType));
            }

            CurrentDeclaration.Members.Add(dom);

            var result = info.Invoke(instance, args.ToArray());
            var a = result as IEnumerable;
            if (a != null)
            {
                var dummyIteraters = a.Cast<object>().ToArray();
                foreach (var item in dummyIteraters)
                {

                }
            }

            PopStatements();

            //var isOverried = false;
            //if (!IsDesignerFile && dom.Attributes != MemberAttributes.Final && templateMethod.Value.Location == TemplateLocation.Both)
            //{
            //    dom.Attributes |= MemberAttributes.Override;
            //    isOverried = true;
            //}
            //if ((info.IsVirtual && !IsDesignerFile) || (info.IsOverride() && !info.GetBaseDefinition().IsAbstract && IsDesignerFile))
            //{
            //    if (templateMethod.Value.CallBase)
            //    {
            //        //if (!info.IsOverride() || !info.GetBaseDefinition().IsAbstract && IsDesignerFile)
            //        //{ 
            //        dom.invoke_base(true);
            //        //}

            //    }
            //}
            InvertApplication.SignalEvent<ICodeTemplateEvents>(_ => _.MethodAdded(instance, this, dom));
            return dom;

        }

        public void AddMemberOutput(IDiagramNodeItem data, TemplateMemberResult templateMemberResult)
        {
            if (ItemFilter != null && !ItemFilter(data)) 
                return;
            Results.Add(templateMemberResult);
        }

        public override void AddMemberIterator(string name, Func<object, IEnumerable> func)
        {
            base.AddMemberIterator(name, func);
            AddIterator(name, _=> { return func(_); });
        }
    }
}