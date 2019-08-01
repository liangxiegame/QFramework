using System;

namespace BindingsRx.Converters
{
    public class TextToTimeSpanConverter : IConverter<string, TimeSpan>, IConverter<TimeSpan, string>
    {
        public string From(TimeSpan value)
        { return value.ToString(); }

        public TimeSpan From(string value)
        {
            TimeSpan output;
            TimeSpan.TryParse(value, out output);
            return output;
        }
    }
}