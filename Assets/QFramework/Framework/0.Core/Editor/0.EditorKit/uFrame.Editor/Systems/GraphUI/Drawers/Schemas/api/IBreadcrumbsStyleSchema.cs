using JetBrains.Annotations;
using UnityEngine;

namespace QFramework.GraphDesigner
{
    public interface IBreadcrumbsStyleSchema
    {

        object GetIcon(string name, Color tint = default(Color));

    }
}