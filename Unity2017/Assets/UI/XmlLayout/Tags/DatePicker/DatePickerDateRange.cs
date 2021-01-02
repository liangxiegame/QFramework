// In order to use this, you need to a) have DatePicker installed within the project, and b) make sure DATEPICKER_PRESENT is defined
// You can do this by
// a) defining it here: #define DATEPICKER_PRESENT (not reccommended as it will be overwritten if you update XmlLayout)
// b) defining it in your "Player Settings" -> "Scripting Define Symbols"
// c) Adding a file called "smcs.rsp" to your PROJECT_DIR/Assets/ folder, and putting the following text in it "-define: DATEPICKER_PRESENT" (you will need to restart Visual Studio after doing so)
#if DATEPICKER_PRESENT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UI.Dates;

namespace UI.Xml.Tags
{
    public class DatePickerDateRangeTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                return currentInstanceTransform.GetComponent<DatePicker_DateRange>();
            }
        }

        public override string prefabPath
        {
            get
            {
                return "Prefabs/DatePicker - Date Range";
            }
        }

        public override bool isCustomElement
        {
            get
            {
                return true;
            }
        }

        public override bool ParseChildElements(System.Xml.XmlNode xmlNode)
        {
            return true;
        }

        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"fromDate", "xs:string"},
                    {"toDate", "xs:string"}
                };
            }
        }
    }
}
#endif