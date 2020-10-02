using System;

namespace QFramework.CodeGen
{
    [Flags]
    public enum TemplateLocation
    {
        DesignerFile = 0,
        EditableFile = 1,
        Both = 2
    }
}