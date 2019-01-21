namespace BindingsRx.Converters
{
    public class DoubleToFloatConverter : IConverter<double, float>, IConverter<float, double>
    {
        public float From(double value)
        { return (float)value; }

        public double From(float value)
        { return value; }
    }
}