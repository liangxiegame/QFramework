using System;
using System.IO;

namespace QF.GraphDesigner
{
    public abstract class OutputGenerator
    {
        public virtual string Filename
        {
            get;
            set;
        }

        public virtual Type GeneratorFor { get; set; }

        public object ObjectData { get; set; }
       
        public string AssetDirectory { get; set; }
        
#if UNITY_EDITOR
        public string UnityPath
        {
            get
            {
                return FullPathName.Replace(UnityEngine.Application.dataPath.ToLower().Replace("\\", "/"),"");
            }
        }
#endif

        public string FullPathName
        {
            get { return Path.Combine(AssetDirectory, Filename).Replace("\\", "/"); }
        }

        public string RelativeFullPathName
        {
            get { return Path.Combine(AssetDirectory, Filename).Replace("\\", "/").Substring(7); }
        }

        public virtual void Initialize(CodeFileGenerator codeFileGenerator)
        {
            
        }

    
        //public bool IsEnabled(IProjectRepository project)
        //{

        //    var customAttribute = this.GetType().GetCustomAttributes(typeof(ShowInSettings), true).OfType<ShowInSettings>().FirstOrDefault();
        //    if (customAttribute == null) return true;

        //    return project.GetSetting(customAttribute.Group, true);
        //}

        public virtual bool DoesTypeExist(FileInfo fileInfo)
        {
            return false;
        }
        public virtual bool IsValid()
        {
           
            return true;
        }
        public bool AlwaysRegenerate { get; set; }
    }
}