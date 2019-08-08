using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class GenericNodeGenerator<TData> : CodeGenerator where TData : GenericNode
    {
        private CodeTypeDeclaration _decleration;


        public TData Data
        {
            get { return ObjectData as TData; }
            set { ObjectData = value; }
        }

        //public override string Filename
        //{
        //    get
        //    {

        //        if (IsDesignerFile)
        //        {
        //            if (GeneratorConfig.DesignerFilename == null)
        //            {
        //                return base.Filename;
        //            }
        //            return GeneratorConfig.DesignerFilename.GetValue(Data);
        //        }
        //        else
        //        {
        //            if (GeneratorConfig.Filename == null)
        //            {
        //                return base.Filename;
        //            }
        //            return GeneratorConfig.Filename.GetValue(Data);
        //        }
                
        //    }
        //    set { base.Filename = value; }
            
        //}

        public virtual string NameAsClass
        {
            get { return GeneratorConfig.ClassName.GetValue(Data); }
        }

        public virtual string NameAsDesignerClass
        {
            get { return NameAsClass + "Base"; }
        }

        public override Type GeneratorFor
        {
            get { return typeof(TData); }
            set
            {
                
            }
        }

        public override void Initialize(CodeFileGenerator codeFileGenerator)
        {
            var nodeConfig = InvertApplication.Container.GetNodeConfig<TData>();
            if (!nodeConfig.TypeGeneratorConfigs.ContainsKey(this.GetType())) return;

            GeneratorConfig = nodeConfig.TypeGeneratorConfigs[this.GetType()] as NodeGeneratorConfig<TData>;
            if (GeneratorConfig == null) return;
            if (GeneratorConfig.Condition != null && !GeneratorConfig.Condition(Data)) return;
            base.Initialize(codeFileGenerator);
         

            Decleration = new CodeTypeDeclaration(IsDesignerFile ? NameAsDesignerClass : NameAsClass)
            {
                IsPartial = true
            };
            Compose();
        }

        public NodeGeneratorConfig<TData> GeneratorConfig { get; set; }

        protected virtual void Compose()
        {
            if (GeneratorConfig.Namespaces != null)
            {
                foreach (var item in GeneratorConfig.Namespaces.GetValue(Data))
                {
                    TryAddNamespace(item);
                }
            }
            if (IsDesignerFile)
            {
                if (GeneratorConfig.DesignerDeclaration != null)
                {
                    Decleration = GeneratorConfig.DesignerDeclaration.GetValue(Data);
                }
                if (GeneratorConfig.BaseType != null)
                {
                    Decleration.BaseTypes.Add(GeneratorConfig.BaseType.GetValue(Data));
                }
                
                InitializeDesignerFile();
               
            }
            else
            {
                if (GeneratorConfig.Declaration != null)
                {
                    Decleration = GeneratorConfig.Declaration.GetValue(Data);
                }
                Decleration.BaseTypes.Add(NameAsDesignerClass);
                
                InitializeEditableFile();
            }
            Namespace.Types.Add(Decleration);
        }

        protected virtual void InitializeEditableFile()
        {
            
        }

        protected virtual void InitializeDesignerFile()
        {
            
        }

        public CodeTypeDeclaration Decleration
        {
            get
            {
               
                return _decleration;
            }
            set { _decleration = value; }
        }

        public void AddMembers<TFor>(Func<TData, IEnumerable<TFor>> selector, IMemberGenerator generator)
        {
            foreach (var item in selector(Data))
            {
                var result = generator.Create(Decleration, item, IsDesignerFile);
                if (result != null)
                Decleration.Members.Add(result);
            }
        }

        public void AddMember<TFor>(Func<TData, TFor> selector, IMemberGenerator generator)
        {
            var result = generator.Create(Decleration, selector(Data), IsDesignerFile);
            if (result != null)
            Decleration.Members.Add(result);
        }
        public void AddMember(IMemberGenerator generator)
        {
            var result = generator.Create(Decleration, Data, IsDesignerFile);
            if (result != null)
                Decleration.Members.Add(result);
        }
        public void AddMethod(CodeMemberMethod method)
        {   
            if (method != null)
                Decleration.Members.Add(method);
        }
    }
}