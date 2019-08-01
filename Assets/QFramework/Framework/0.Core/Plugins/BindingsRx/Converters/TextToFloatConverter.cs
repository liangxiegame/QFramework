namespace BindingsRx.Converters
{
    public class TextToFloatConverter : IConverter<string, float>, IConverter<float, string>
    {
        public string From(float value)
        { return value.ToString(); }

        public float From(string value)
        {
            float output;
            float.TryParse(value, out output);
            return output;
        }
    }
}