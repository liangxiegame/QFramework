namespace BindingsRx.Converters
{
    public class TextToDoubleConverter : IConverter<string, double>, IConverter<double, string>
    {
        public string From(double value)
        { return value.ToString(); }

        public double From(string value)
        {
            double output;
            double.TryParse(value, out output);
            return output;
        }
    }
}