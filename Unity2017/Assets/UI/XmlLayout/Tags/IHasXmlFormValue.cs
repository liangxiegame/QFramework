using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Xml.Tags
{
    interface IHasXmlFormValue
    {
        string GetValue(XmlElement element);        
    }
}
