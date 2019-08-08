using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Common;
using UnityEngine;

namespace QF.GraphDesigner.Unity.Schemas
{
    class UnityBreadcrumbsStyleSchema : BreadcrumbsStyleSchema
    {
        protected override object ConstructIcon(string name, Color tint = default(Color))
        {
            Texture2D texture = ElementDesignerStyles.GetSkinTexture(name);

            if (tint != default(Color))
            {
                texture = texture.Tint(tint);
            }

            return texture;
        }
    }
}
