using System;

namespace QF.GraphDesigner
{
    public class RegisteredConnection
    {
        public Type TOutputType { get; set; }
        public Type TInputType { get; set; }

        public virtual bool CanConnect(IConnectable output, IConnectable input)
        {
            if (CanConnect(output.GetType(), input.GetType()))
            {
                if (output.CanOutputTo(input) && input.CanInputFrom(output))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CanConnect(Type output, Type input)
        {
            if (TOutputType.IsAssignableFrom(output))
            {
                if (TInputType.IsAssignableFrom(input))
                {
                    return true;
                }
            }
            return false;
        }
    }
}