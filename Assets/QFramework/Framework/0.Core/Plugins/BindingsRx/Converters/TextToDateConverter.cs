using System;

namespace BindingsRx.Converters
{
    public class TextToDateConverter : IConverter<string, DateTime>, IConverter<DateTime, string>
    {
        private string _dateFormat { get; set; }

        public TextToDateConverter(string dateFormat = "d")
        {
            _dateFormat = dateFormat;
        }

        public string From(DateTime value)
        { return value.ToString(_dateFormat); }

        public DateTime From(string value)
        {
            DateTime output;
            DateTime.TryParse(value, out output);
            return output;
        }
    }
}