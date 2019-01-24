using UnityEngine;

namespace QFramework
{
    public static class MathHelper
    {
        public static T RandomValueFrom<T>(params T[] values)
        {
            return values[Random.Range(0, values.Length)];
        }

        /// <summary>
        /// percent probability
        /// </summary>
        /// <param name="percent"> 0 ~ 100 </param>
        /// <returns></returns>
        public static bool PercentProbability(int percent)
        {
            return Random.Range(0, 1000) * 0.001f < 50 * 0.01f;
        }
    }
}