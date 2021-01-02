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
using System;
using System.Linq;
using UI.Dates;

namespace UI.Xml.Tags
{
    public abstract class DatePickerConfigBase : ElementTagHandler
    {
        public override string prefabPath
        {
            get
            {
                return null;
            }
        }

        public override bool isCustomElement
        {
            get
            {
                return true;
            }
        }

        public override bool renderElement
        {
            get
            {
                return false;
            }
        }

        public override string extension
        {
            get
            {
                return "blank";
            }
        }

        public override string elementGroup
        {
            get
            {
                return "datePickerGroup";
            }
        }        

        protected DatePickerConfig BaseConfigObject
        {
            get
            {
                return ((DatePicker)DatePickerTagHandler.CurrentTagHandler.primaryComponent).Config;
            }
        }

        protected virtual object ConfigObject
        {
            get
            {
                return BaseConfigObject;
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributes)
        {
            if (DatePickerTagHandler.CurrentTagHandler == null) return;

            foreach (var attribute in attributes)
            {
                SetPropertyValue(ConfigObject, attribute.Key, attribute.Value);
            }
        }
    }
    
    [ElementTagHandler("DP_MiscConfig")]
    public class DatePickerMiscConfigTagHandler : DatePickerConfigBase
    {        
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"switchToSelectedMonthWhenDateSelected", "xs:boolean"},
                    {"showDatesInOtherMonths", "xs:boolean"},
                    {"closeWhenDateSelected", "xs:boolean"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Misc;
            }
        }
    }

    [ElementTagHandler("DP_Sizing")]
    public class DatePickerSizingConfigTagHandler : DatePickerConfigBase
    {        
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"overrideTransformHeight", "xs:boolean"},
                    {"preferredHeight", "xs:float"},
                    {"usePreferredWidthInsteadOfInputFieldWidth", "xs:boolean"},
                    {"preferredWidth", "xs:float"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Sizing;
            }
        }
    }

    [ElementTagHandler("DP_Modal")]
    public class DatePickerModalConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"isModal", "xs:boolean"},
                    {"closeWhenModalOverlayClicked", "xs:boolean"},
                    {"screenOverlayColor", "xmlLayout:color"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Modal;
            }
        }
    }

    [ElementTagHandler("DP_DateRange")]
    public class DatePickerDateRangeConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"restrictFromDate", "xs:boolean"},
                    {"restrictToDate", "xs:boolean"},
                    {"fromDate", "xs:string"},
                    {"toDate", "xs:string"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.DateRange;
            }
        }
    }

    [ElementTagHandler("DP_Format")]
    public class DatePickerFormatConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                    
                    {"dateFormat", "xs:string"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Format;
            }
        }
    }

    [ElementTagHandler("DP_Header")]
    public class DatePickerHeaderConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                    
                    {"showHeader", "xs:boolean"},
                    {"backgroundColor", "xmlLayout:color"},
                    {"textColor", "xmlLayout:color"},
                    {"font", "xs:string"},
                    {"showNextAndPreviousMonthButtons", "xs:boolean"},
                    {"showNextAndPreviousYearButtons", "xs:boolean"},
                    {"height", "xs:float"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Header;
            }
        }

        public override string elementChildType
        {
            get
            {
                return "datePickerHeaderConfig";
            }
        }
    }

    [ElementTagHandler("DP_HeaderButton")]
    public class DatePickerHeaderButtonConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                    
                    {"image", "xs:string"},
                    {"colors", "xmlLayout:colorblock"},
                    {"type", "nextMonth,previousMonth,nextYear,previousYear,months,years,all"},
                    {"fadeDuration", "xs:float"},
                    {"colorMultiplier", "xs:float"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {                
                return BaseConfigObject.Header;
            }
        }

        public override string elementGroup
        {
            get
            {
                return "datePickerHeaderConfig";
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributes)
        {
            //base.ApplyAttributes(attributes);
            var type = attributes.GetValue("type");

            if (type != null) type = type.ToLower();

            DatePickerButtonConfig[] buttonsToApplyTo = new DatePickerButtonConfig[] { };
            DatePickerHeaderConfig headerConfig = ConfigObject as DatePickerHeaderConfig;

            if (!string.IsNullOrEmpty(type) && !type.Equals("all", System.StringComparison.OrdinalIgnoreCase))
            {
                switch (type)
                {
                    case "nextmonth":
                        buttonsToApplyTo = new DatePickerButtonConfig[] { headerConfig.NextMonthButton };
                        break;

                    case "previousmonth":
                        buttonsToApplyTo = new DatePickerButtonConfig[] { headerConfig.PreviousMonthButton };
                        break;

                    case "nextyear":
                        buttonsToApplyTo = new DatePickerButtonConfig[] { headerConfig.NextYearButton };
                        break;

                    case "previousyear":
                        buttonsToApplyTo = new DatePickerButtonConfig[] { headerConfig.PreviousYearButton };
                        break;

                    case "months":
                        buttonsToApplyTo = new DatePickerButtonConfig[] { headerConfig.NextMonthButton, headerConfig.PreviousMonthButton };
                        break;

                    case "years":
                        buttonsToApplyTo = new DatePickerButtonConfig[] { headerConfig.NextYearButton, headerConfig.PreviousYearButton };
                        break;
                }                                
            }
            else
            {                
                // this applies to all four buttons
                buttonsToApplyTo = new DatePickerButtonConfig[] { headerConfig.NextMonthButton, headerConfig.PreviousMonthButton, headerConfig.NextYearButton, headerConfig.PreviousYearButton };
            }

            foreach (var button in buttonsToApplyTo)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.Key.Equals("type", System.StringComparison.OrdinalIgnoreCase)) continue;

                    if (attribute.Key.Equals("fadeDuration", System.StringComparison.OrdinalIgnoreCase))
                    {
                        button.Colors.fadeDuration = attribute.Value.ChangeToType<float>();
                        continue;
                    }

                    if (attribute.Key.Equals("colorMultiplier", System.StringComparison.OrdinalIgnoreCase))
                    {
                        button.Colors.colorMultiplier = attribute.Value.ChangeToType<float>();
                        continue;
                    }

                    SetPropertyValue(button, attribute.Key, attribute.Value);                                        
                }
            }
        }
    }

    [ElementTagHandler("DP_WeekDays")]
    public class DatePickerWeekDaysConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                    
                    {"overrideTemplate", "xs:boolean"},
                    {"textColor", "xmlLayout:color"},
                    {"font", "xs:string"},
                    {"backgroundImage", "xs:string"},
                    {"backgroundColor", "xmlLayout:color"}                    
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.WeekDays;
            }
        }        
    }

    [ElementTagHandler("DP_Days")]
    public class DatePickerDaysConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                                        
                    {"backgroundColor", "xmlLayout:color"},
                    {"font", "xs:string"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Days;
            }
        }

        public override string elementChildType
        {
            get
            {
                return "datePickerDayConfig";
            }
        }
    }

    [ElementTagHandler("DP_Day")]
    public class DatePickerDayConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                                        
                    {"type", "all,currentMonth,otherMonths,selectedDay,today"},
                    {"overrideTemplate", "xs:boolean"},
                    {"textColor", "xmlLayout:color"},
                    {"backgroundImage", "xs:string"},
                    {"backgroundColors", "xmlLayout:colorblock"},                    
                    {"backgroundColorFadeDuration", "xs:float"},
                    {"backgroundColorMultiplier", "xs:float"}                                        
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Days;
            }
        }

        public override string elementGroup
        {
            get
            {
                return "datePickerDayConfig";
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributes)
        {            
            var type = attributes.GetValue("type");

            if (type != null) type = type.ToLower();

            DatePickerDayButtonConfig[] buttonsToApplyTo = new DatePickerDayButtonConfig[] { };
            DatePickerDayConfig daysConfig = ConfigObject as DatePickerDayConfig;

            if (!string.IsNullOrEmpty(type) && !type.Equals("all", System.StringComparison.OrdinalIgnoreCase))
            {
                switch (type)
                {
                    case "currentmonth":
                        buttonsToApplyTo = new DatePickerDayButtonConfig[] { daysConfig.CurrentMonth };
                        break;
                    case "othermonths":
                        buttonsToApplyTo = new DatePickerDayButtonConfig[] { daysConfig.OtherMonths };
                        break;
                    case "selectedday":
                        buttonsToApplyTo = new DatePickerDayButtonConfig[] { daysConfig.SelectedDay };
                        break;
                    case "today":
                        buttonsToApplyTo = new DatePickerDayButtonConfig[] { daysConfig.Today };
                        break;
                }
            }
            else
            {
                buttonsToApplyTo = new DatePickerDayButtonConfig[] { daysConfig.CurrentMonth, daysConfig.OtherMonths, daysConfig.SelectedDay, daysConfig.Today };
            }

            foreach (var button in buttonsToApplyTo)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.Key.Equals("type", System.StringComparison.OrdinalIgnoreCase)) continue;

                    if (attribute.Key.Equals("backgroundColorFadeDuration", System.StringComparison.OrdinalIgnoreCase))
                    {
                        button.BackgroundColors.fadeDuration = attribute.Value.ChangeToType<float>();
                        continue;
                    }

                    if (attribute.Key.Equals("backgroundColorMultiplier", System.StringComparison.OrdinalIgnoreCase))
                    {
                        button.BackgroundColors.colorMultiplier = attribute.Value.ChangeToType<float>();
                        continue;
                    }

                    SetPropertyValue(button, attribute.Key, attribute.Value);
                }
            }
        }
    }

    [ElementTagHandler("DP_Animation")]
    public class DatePickerAnimationConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                    
                    {"showAnimation", "none,fade,slide"},
                    {"hideAnimation", "none,fade,slide"},
                    {"monthChangedAnimation", "none,fade,slide"},
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Animation;
            }
        }
    }

    [ElementTagHandler("DP_InputField")]
    public class DatePickerInputFieldConfigTagHandler : DatePickerConfigBase
    {        
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                    
                    {"toggleDisplayWhenInputFieldClicked", "xs:boolean"},
                    {"showToggleButton", "xs:boolean"},
                    {"toggleButtonWidth", "xs:float"},
                    {"datePickerAlignmentRelativeToInputField", "Left,Center,Right"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.InputField;
            }
        }
    }

    [ElementTagHandler("DP_Border")]
    public class DatePickerBorderConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                    
                    {"size", "xmlLayout:rectOffset"},
                    {"color", "xmlLayout:color"}
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Border;
            }
        }
    }

    [ElementTagHandler("DP_Events")]
    public class DatePickerEventsConfigTagHandler : DatePickerConfigBase
    {
        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {                    
                    {"onDaySelected", "xmlLayout:function"},
                    {"onDayMouseOver", "xmlLayout:function"},
                };
            }
        }

        protected override object ConfigObject
        {
            get
            {
                return BaseConfigObject.Events;
            }
        }

        protected override List<string> eventAttributeNames
        {
            get
            {
                return new List<string>() { "ondayselected", "ondaymouseover" };
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributes)
        {
            foreach (var attribute in attributes)
            {
                if (eventAttributeNames.Contains(attribute.Key, StringComparer.OrdinalIgnoreCase))
                {
                    HandleEventAttribute(attribute.Key, attribute.Value);
                }
            }
        }

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {            
            var configObject = ((DatePickerEventConfig)ConfigObject);            
            var layout = currentXmlLayoutInstance;            
            var datePicker = DatePickerTagHandler.CurrentTagHandler.primaryComponent as DatePicker;
            var transform = datePicker.transform as RectTransform;

            var eventData = eventValue.Trim(new Char[] { ')', ';' })
                          .Split(',', '(');

            string value = null;
            if (eventData.Count() > 1)
            {
                value = eventData[1];
            }

            UnityEngine.Events.UnityAction<DateTime> action = (e) =>
            {
                string _value = value;
                var valueLower = value.ToLower();

                if (valueLower == "date" || valueLower == "selecteddate")
                {
                    _value = DatePickerUtilities.ToDateString(e);
                }                

                layout.XmlLayoutController.ReceiveMessage(eventData[0], _value, transform);
            };

            switch (eventName.ToLower())
            {
                case "ondayselected":
                    configObject.OnDaySelected.AddListener(action);
                    break;
                case "ondaymouseover":
                    configObject.OnDayMouseOver.AddListener(action);
                    break;
            }
        }
    }
}
#endif