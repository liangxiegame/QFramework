using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace UI.Xml.CustomAttributes
{    
    public abstract class SizeAttribute: CustomXmlAttribute
    {
        public override bool KeepOriginalTag
        {
            get
            {
                return true;
            }
        }               

        protected Vector2 ApplyAlignment(Vector2 vector, RectAlignment alignment = RectAlignment.MiddleCenter)
        {
           switch (alignment)
            {
                case RectAlignment.MiddleCenter:
                    return new Vector2((1 - vector.x) / 2, (1 - vector.y) / 2);

                case RectAlignment.MiddleLeft:
                    return new Vector2(0, (1 - vector.y) / 2);

                case RectAlignment.MiddleRight:
                    return new Vector2(1 - vector.x, (1 - vector.y) / 2);

                case RectAlignment.UpperCenter:
                    return new Vector2((1 - vector.x) / 2, 1 - vector.y);

                case RectAlignment.LowerCenter:
                    return new Vector2((1 - vector.x) / 2, 0f);

                case RectAlignment.UpperLeft:
                    return new Vector2(0, 1 - vector.y);

                case RectAlignment.UpperRight:
                    return new Vector2(1 - vector.x, 1 - vector.y);

                case RectAlignment.LowerLeft:
                    return new Vector2(0, 0);

                case RectAlignment.LowerRight:
                    return new Vector2(1 - vector.x, 0);
            }

            return vector;
        }

        protected RectAlignmentStruct GetAlignmentStruct(   float width, 
                                                            float height,
                                                            Vector2 position,
                                                            RectAlignment alignment = RectAlignment.MiddleCenter)
        {
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Vector2 anchorMin = new Vector2(0.5f, 0.5f);
            Vector2 anchorMax = new Vector2(0.5f, 0.5f);
            var halfWidth = width / 2f;
            var halfHeight = height / 2f;

            switch (alignment)
            {
                case RectAlignment.LowerCenter:
                    pivot = new Vector2(0.5f, 0);
                    anchorMin = new Vector2(0.5f, 0);
                    anchorMax = new Vector2(0.5f, 0);
                    position = new Vector2(0, halfHeight);
                    break;
                case RectAlignment.LowerLeft:
                    pivot = new Vector2(0, 0);
                    anchorMin = new Vector2(0, 0);
                    anchorMax = new Vector2(0, 0);
                    position = new Vector2(halfWidth, halfHeight);
                    break;
                case RectAlignment.LowerRight:
                    pivot = new Vector2(1, 0);
                    anchorMin = new Vector2(1, 0);
                    anchorMax = new Vector2(1, 0);
                    position = new Vector2(-halfWidth, halfHeight);
                    break;

                case RectAlignment.MiddleCenter:
                    break;

                case RectAlignment.MiddleLeft:
                    pivot = new Vector2(0, 0.5f);
                    anchorMin = new Vector2(0, 0.5f);
                    anchorMax = new Vector2(0, 0.5f);
                    position = new Vector2(halfWidth, 0);
                    break;
                case RectAlignment.MiddleRight:
                    pivot = new Vector2(1, 0.5f);
                    anchorMin = new Vector2(1, 0.5f);
                    anchorMax = new Vector2(1, 0.5f);
                    position = new Vector2(-halfWidth, 0);
                    break;

                case RectAlignment.UpperCenter:
                    pivot = new Vector2(0.5f, 1);
                    anchorMin = new Vector2(0.5f, 1);
                    anchorMax = new Vector2(0.5f, 1);
                    position = new Vector2(0, -halfHeight);
                    break;

                case RectAlignment.UpperLeft:
                    pivot = new Vector2(0, 1);
                    anchorMin = new Vector2(0, 1);
                    anchorMax = new Vector2(0, 1);
                    position = new Vector2(halfWidth, -halfHeight);
                    break;
                case RectAlignment.UpperRight:
                    pivot = new Vector2(1, 1);
                    anchorMin = new Vector2(1, 1);
                    anchorMax = new Vector2(1, 1);
                    position = new Vector2(-halfWidth, -halfHeight);
                    break;
            }

            return new RectAlignmentStruct
            {
                Pivot = pivot,
                AnchorMin = anchorMin,
                AnchorMax = anchorMax,
                Position = position
            };
        }

        protected RectAlignment GetRectAlignment(string alignment)
        {
            RectAlignment rectAlignment = RectAlignment.MiddleCenter;            
            if (Enum.GetNames(typeof(RectAlignment)).Contains(alignment, StringComparer.OrdinalIgnoreCase))
            {
                rectAlignment = (RectAlignment)Enum.Parse(typeof(RectAlignment), alignment, true);
            }

            return rectAlignment;
        }

        public override eAttributeGroup AttributeGroup
        {
            get
            {
                return eAttributeGroup.RectPosition;
            }
        }
    }            

    public struct RectAlignmentStruct
    {
        public Vector2 Pivot;
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Position;
    }
}
