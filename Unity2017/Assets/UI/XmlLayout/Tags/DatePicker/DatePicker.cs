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
    public class DatePickerInlineTagHandler : DatePickerTagHandler
    {
        public override string prefabPath
        {
            get
            {
                return "Prefabs/DatePicker - Inline";
            }
        }
    }

    public class DatePickerTagHandler : ElementTagHandler
    {
        protected DatePicker DatePicker
        {
            get
            {
                return primaryComponent as DatePicker;
            }
        }

        public override MonoBehaviour primaryComponent
        {
            get
            {
                return currentInstanceTransform.GetComponent<DatePicker>();
            }
        }

        public override string prefabPath
        {
            get
            {
                return "Prefabs/DatePicker - Popup";
            }
        }

        public override bool isCustomElement
        {
            get
            {
                return true;
            }
        }        

        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"selectedDate", "xs:string"},
                    {"visibleDate", "xs:string"},
                    {"visibleDateDefaultBehaviour", "UseStoredValue,UseTodaysDate"}
                };
            }
        }

        public override string elementChildType
        {
            get
            {
                return "datePickerGroup";
            }
        }

        private static DatePickerTagHandler _previousTagHandler { get; set; }
        public static DatePickerTagHandler CurrentTagHandler { get; private set; }

        public override void Open(AttributeDictionary attributes)
        {
            base.Open(attributes);

            _previousTagHandler = CurrentTagHandler;
            CurrentTagHandler = this;
        }

        public override void Close()
        {
            base.Close();

            var _datePicker = (DatePicker)primaryComponent;
            _datePicker.UpdateDisplay();

            DatePickerTimer.DelayedCall(0.1f, () => { (_datePicker).UpdateDisplay(); }, _datePicker);

            CurrentTagHandler = _previousTagHandler;
        }
    }
}
#endif