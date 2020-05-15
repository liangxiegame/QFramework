using UnityEngine;

namespace QFramework
{
    public class TMathUtils
    {
        public const float EPSILON = 0.00001f;

        public static bool IsZero(float v, float e = EPSILON)
        {
            return Mathf.Abs(v) < EPSILON;
        }
        public static Vector3 Vector3ZeroY(Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }
        public static Vector3 GetDirection2D(Vector3 to, Vector3 from)
        {
            Vector3 dir = to - from;
            dir.y = 0;
            return dir.normalized;
        }
        public static float GetDistance2D(Vector3 to, Vector3 from)
        {
            Vector3 dir = to - from;
            dir.y = 0;
            return dir.magnitude;
        }
    }
}
