namespace BindingsRx.Converters
{
    public class TextToIntConverter : IConverter<string, int>, IConverter<int, string>
    {
        public string From(int value)
        { return value.ToString(); }

        public int From(string value)
        {
            int output;
            int.TryParse(value, out output);
            return output;            
        }
    }
}