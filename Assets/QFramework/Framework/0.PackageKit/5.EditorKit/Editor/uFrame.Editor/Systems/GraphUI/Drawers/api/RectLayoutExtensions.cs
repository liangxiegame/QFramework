using System.Linq;
using UnityEngine;

namespace QF.GraphDesigner
{
    public static class RectLayoutExtensions
    {
        public static Rect Below(this Rect source, Rect belowSource)
        {
            return new Rect(source.x, belowSource.y + belowSource.height, source.width, source.height);
        }
   
        
        public static Rect RightOf(this Rect source, Rect target)
        {
            return new Rect(target.x + target.width, source.y, source.width, source.height);
        }

        public static Rect WithSize(this Rect source, float width, float height)
        {
            return new Rect(source.x, source.y, width, height);
        }

        public static Rect WithWidth(this Rect source, float width)
        {
            return new Rect(source.x, source.y, width, source.height);
        }

        public static Rect WithHeight(this Rect source, float height)
        {
            return new Rect(source.x, source.y, source.width, height);
        }

        public static Rect Pad(this Rect source, float left, float top, float right, float bottom)
        {
            return new Rect(source.x + left, source.y + top, source.width-right, source.height-bottom);
        }

        public static Rect PadSides(this Rect source, float padding)
        {
            return new Rect(source.x + padding, source.y + padding, source.width - padding*2, source.height - padding*2);
        }

        public static Rect AlignTopRight(this Rect source, Rect target)
        {
            return new Rect(target.xMax - source.width, target.y, source.width, source.height);
        }

        public static Rect AlignHorisonallyByCenter(this Rect source, Rect target)
        {
            var y = target.y + (target.height - source.height)/2;

            return new Rect(source.x, y, source.width, source.height);
        }

        public static Rect AlignVerticallyByCenter(this Rect source, Rect target)
        {
            var x = target.x + (target.width- source.width)/2;
            return new Rect(x, source.y, source.width, source.height);
        }

        public static Rect Translate(this Rect source, float x, float y)
        {
            return new Rect(source.x + x, source.y + y, source.width, source.height);
        }

        public static Rect WithOrigin(this Rect source, float x, float y)
        {
            return new Rect(x, y, source.width, source.height);
        }
   
        public static Rect Align(this Rect source, Rect target)
        {
            return new Rect(target.x, target.y, source.width, source.height);
        }

        public static Rect AlignAndScale(this Rect source, Rect target)
        {
            return new Rect(target.x, target.y, target.width, target.height);
        }

        public static Rect AlignHorisontally(this Rect source, Rect target)
        {
            return new Rect(source.x, target.y, source.width, source.height);
        }
        
        public static Rect AlignVertically(this Rect source, Rect target)
        {
            return new Rect(target.x, source.y, source.width, source.height);
        }

        public static Rect CenterInsideOf(this Rect source, Rect target)
        {
            var y = target.y + (target.height - source.height) / 2;
            var x = target.x + (target.width - source.width) / 2;
            return new Rect(x, y, source.width, source.height);      
        }

        public static Rect LeftHalf(this Rect source)
        {
            return new Rect(source.x, source.y, source.width/2, source.height);
        }

        public static Rect RightHalf(this Rect source)
        {
            return new Rect(source.x+source.width/2, source.y, source.width/2, source.height);
        }

        public static Rect TopHalf(this Rect source)
        {
            return new Rect(source.x,source.y,source.width,source.height/2);
        }

        public static Rect BottomHalf(this Rect source)
        {
            return new Rect(source.x, source.y + source.height/2, source.width, source.height/2);
        }
        public static Rect Clip(this Rect source, Rect target)
        {
            var x = source.x;
            if (source.x < target.x) x = target.x;
            if (source.x > target.xMax) x = target.xMax;

            var y = source.y;
            if (source.y < target.y) y = target.y;
            if (source.y > target.yMax) y = target.yMax;

            var width = source.width;
            if (x + source.width > target.xMax) width = target.xMax - source.x;

            var height = source.height;
            if (y + source.height > target.yMax) height = target.yMax - source.y;

            return new Rect(x,y,width,height);
        }

        public static Rect InnerAlignWithBottomRight(this Rect source, Rect target)
        {
            return new Rect(target.xMax - source.width,target.yMax - source.height, source.width, source.height);
        }            
        
        public static Rect InnerAlignWithCenterRight(this Rect source, Rect target)
        {
            return source.InnerAlignWithBottomRight(target).AlignHorisonallyByCenter(target);
        }    
        
        public static Rect InnerAlignWithCenterLeft(this Rect source, Rect target)
        {
            return source.InnerAlignWithBottomLeft(target).AlignHorisonallyByCenter(target);
        }    
        
        public static Rect InnerAlignWithBottomLeft(this Rect source, Rect target)
        {
            return new Rect(target.x, target.yMax - source.height, source.width, source.height);
        }    
        
        public static Rect InnerAlignWithUpperRight(this Rect source, Rect target)
        {
            return new Rect(target.xMax - source.width, target.y, source.width, source.height);
        }

        public static Rect InnerAlignWithBottomCenter(this Rect source, Rect target)
        {
            var rect = source.AlignVerticallyByCenter(target);
            rect.y = target.yMax - rect.height;
            return rect;
        }

        public static Rect LeftOf(this Rect source, Rect target)
        {
            return new Rect(target.x-source.width,source.y,source.width,source.height);
        }

        public static Rect Above(this Rect source, Rect target)
        {
            return new Rect(source.x,target.y- source.height,source.width,source.height);
        }
        
        public static Rect AboveAll(this Rect source, Rect target, int i)
        {
            return new Rect(source.x,target.y- source.height*i,source.width,source.height);
        }

        public static Rect Cover(this Rect source, params Rect[] targets)
        {
            var x = targets.Min(t => t.x);
            var y = targets.Min(t => t.y);
            var width = targets.Max(t => t.xMax - x);
            var height= targets.Max(t => t.yMax - y);
            return new Rect(x, y, width, height);

   

        }

        public static Rect StretchedVerticallyAlong(this Rect source, Rect target)
        {
            return new Rect(source.x, source.y, source.width, target.yMax - source.y);
        }

        public static Rect AddHeight(this Rect source, int height)
        {
            return new Rect(source.x, source.y, source.width, source.height + height);
        }      
        public static Rect AddWidth(this Rect source, int width)
        {
            return new Rect(source.x, source.y, source.width + width, source.height);
        }


    }
}