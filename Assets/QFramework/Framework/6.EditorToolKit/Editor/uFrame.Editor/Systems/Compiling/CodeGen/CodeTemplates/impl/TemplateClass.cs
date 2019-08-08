using System;

namespace QF.GraphDesigner
{
    public class TemplateClass : Attribute
    {
        private string _classNameFormat = "{0}";
        private TemplateLocation _location = TemplateLocation.Both;
        private bool _autoInherit = true;

        public TemplateLocation Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public string ClassNameFormat
        {
            get { return _classNameFormat; }
            set { _classNameFormat = value; }
        }



        public bool AutoInherit
        {
            get { return _autoInherit; }
            set { _autoInherit = value; }
        }

        public TemplateClass()
        {
        }

        public TemplateClass(TemplateLocation location)
        {
            Location = location;
        }



        public TemplateClass(TemplateLocation location, string classNameFormat)
        {
            ClassNameFormat = classNameFormat;
            Location = location;
        }
    }
}