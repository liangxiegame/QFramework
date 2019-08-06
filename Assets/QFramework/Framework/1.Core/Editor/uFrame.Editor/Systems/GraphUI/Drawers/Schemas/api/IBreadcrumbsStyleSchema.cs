using JetBrains.Annotations;
using UnityEngine;

namespace QF.GraphDesigner
{
    public interface IBreadcrumbsStyleSchema
    {

        object GetIcon(string name, Color tint = default(Color));

    }
}