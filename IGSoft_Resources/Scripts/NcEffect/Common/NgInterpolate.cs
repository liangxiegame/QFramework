using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Interpolation utility functions: easing, bezier, and catmull-rom.
 * Consider using Unity's Animation curve editor and AnimationCurve class
 * before scripting the desired behaviour using this utility.
 *
 * Interpolation functionality available at different levels of abstraction.
 * Low level access via individual easing functions (ex. EaseInOutCirc),
 * Bezier(), and CatmullRom(). High level access using sequence generators,
 * NewEase(), NewBezier(), and NewCatmullRom().
 *
 * Sequence generators are typically used as follows:
 *
 * IEnumerable<Vector3> sequence = NcInterpolate.New[Ease|Bezier|CatmulRom](configuration);
 * foreach (Vector3 newPoint in sequence) {
 *   transform.position = newPoint;
 *   yield return WaitForSeconds(1.0f);
 * }
 *
 * Or:
 *
 * IEnumerator<Vector3> sequence = NcInterpolate.New[Ease|Bezier|CatmulRom](configuration).GetEnumerator();
 * function Update() {
 *   if (sequence.MoveNext()) {
 *     transform.position = sequence.Current;
 *   }
 * }
 *
 * The low level functions work similarly to Unity's built in Lerp and it is
 * up to you to track and pass in elapsedTime and duration on every call. The
 * functions take this form (or the logical equivalent for Bezier() and CatmullRom()).
 *
 * transform.position = ease(start, distance, elapsedTime, duration);
 *
 * For convenience in configuration you can use the Ease(EaseType) function to
 * look up a concrete easing function:
 * 
 *  [SerializeField]
 *  NcInterpolate.EaseType easeType; // set using Unity's property inspector
 *  NcInterpolate.Function ease; // easing of a particular EaseType
 * function Awake() {
 *   ease = NcInterpolate.Ease(easeType);
 * }
 *
 * @author Fernando Zapata (fernando@cpudreams.com)
 * @Traduzione Andrea85cs (andrea85cs@dynematica.it)
 */
 
public class NgInterpolate
{
    /**
 * Different methods of easing interpolation.
 */
    public enum EaseType {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc
    }
 
    /**
    * Sequence of eleapsedTimes until elapsedTime is >= duration.
    *
    * Note: elapsedTimes are calculated using the value of Time.deltatTime each
    * time a value is requested.
    */
    static Vector3 Identity(Vector3 v) {
        return v;
    }
 
    static Vector3 TransformDotPosition(Transform t) {
        return t.position;
    }
 
 
    static IEnumerable<float> NewTimer(float duration) {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration) {
            yield return elapsedTime;
            elapsedTime += Time.deltaTime;
            // make sure last value is never skipped
            if (elapsedTime >= duration) {
                yield return elapsedTime;
            }
        }
    }
 
    public delegate Vector3 ToVector3<T>(T v);
    public delegate float Function(float a, float b, float c, float d);
 
    /**
     * Generates sequence of integers from start to end (inclusive) one step
     * at a time.
     */
    static IEnumerable<float> NewCounter(int start, int end, int step) {
        for (int i = start; i <= end; i += step) {
            yield return i;
        }
    }
 
    /**
     * Returns sequence generator from start to end over duration using the
     * given easing function. The sequence is generated as it is accessed
     * using the Time.deltaTime to calculate the portion of duration that has
     * elapsed.
     */
    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float duration) {
        IEnumerable<float> timer = NgInterpolate.NewTimer(duration);
        return NewEase(ease, start, end, duration, timer);
    }
 
    /**
     * Instead of easing based on time, generate n interpolated points (slices)
     * between the start and end positions.
     */
    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, int slices) {
        IEnumerable<float> counter = NgInterpolate.NewCounter(0, slices + 1, 1);
        return NewEase(ease, start, end, slices + 1, counter);
    }
 
 
 
    /**
     * Generic easing sequence generator used to implement the time and
     * slice variants. Normally you would not use this function directly.
     */
    static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float total, IEnumerable<float> driver) {
        Vector3 distance = end - start;
        foreach (float i in driver) {
            yield return Ease(ease, start, distance, i, total);
        }
    }
 
    /**
     * Vector3 interpolation using given easing method. Easing is done independently
     * on all three vector axis.
     */
    static Vector3 Ease(Function ease, Vector3 start, Vector3 distance, float elapsedTime, float duration) {
        start.x = ease(start.x, distance.x, elapsedTime, duration);
        start.y = ease(start.y, distance.y, elapsedTime, duration);
        start.z = ease(start.z, distance.z, elapsedTime, duration);
        return start;
    }
 
    /**
     * Returns the static method that implements the given easing type for scalars.
     * Use this method to easily switch between easing interpolation types.
     *
     * All easing methods clamp elapsedTime so that it is always <= duration.
     *
     * var ease = NcInterpolate.Ease(EaseType.EaseInQuad);
     * i = ease(start, distance, elapsedTime, duration);
     */
    public static Function Ease(EaseType type) {
        // Source Flash easing functions:
        // http://gizma.com/easing/
        // http://www.robertpenner.com/easing/easing_demo.html
        //
        // Changed to use more friendly variable names, that follow my Lerp
        // conventions:
        // start = b (start value)
        // distance = c (change in value)
        // elapsedTime = t (current time)
        // duration = d (time duration)
 
        Function f = null;
        switch (type) {
            case EaseType.Linear: f = NgInterpolate.Linear; break;
            case EaseType.EaseInQuad: f = NgInterpolate.EaseInQuad; break;
            case EaseType.EaseOutQuad: f = NgInterpolate.EaseOutQuad; break;
            case EaseType.EaseInOutQuad: f = NgInterpolate.EaseInOutQuad; break;
            case EaseType.EaseInCubic: f = NgInterpolate.EaseInCubic; break;
            case EaseType.EaseOutCubic: f = NgInterpolate.EaseOutCubic; break;
            case EaseType.EaseInOutCubic: f = NgInterpolate.EaseInOutCubic; break;
            case EaseType.EaseInQuart: f = NgInterpolate.EaseInQuart; break;
            case EaseType.EaseOutQuart: f = NgInterpolate.EaseOutQuart; break;
            case EaseType.EaseInOutQuart: f = NgInterpolate.EaseInOutQuart; break;
            case EaseType.EaseInQuint: f = NgInterpolate.EaseInQuint; break;
            case EaseType.EaseOutQuint: f = NgInterpolate.EaseOutQuint; break;
            case EaseType.EaseInOutQuint: f = NgInterpolate.EaseInOutQuint; break;
            case EaseType.EaseInSine: f = NgInterpolate.EaseInSine; break;
            case EaseType.EaseOutSine: f = NgInterpolate.EaseOutSine; break;
            case EaseType.EaseInOutSine: f = NgInterpolate.EaseInOutSine; break;
            case EaseType.EaseInExpo: f = NgInterpolate.EaseInExpo; break;
            case EaseType.EaseOutExpo: f = NgInterpolate.EaseOutExpo; break;
            case EaseType.EaseInOutExpo: f = NgInterpolate.EaseInOutExpo; break;
            case EaseType.EaseInCirc: f = NgInterpolate.EaseInCirc; break;
            case EaseType.EaseOutCirc: f = NgInterpolate.EaseOutCirc; break;
            case EaseType.EaseInOutCirc: f = NgInterpolate.EaseInOutCirc; break;
        }
        return f;
    }
 
    /**
     * Returns sequence generator from the first node to the last node over
     * duration time using the points in-between the first and last node
     * as control points of a bezier curve used to generate the interpolated points
     * in the sequence. If there are no control points (ie. only two nodes, first
     * and last) then this behaves exactly the same as NewEase(). In other words
     * a zero-degree bezier spline curve is just the easing method. The sequence
     * is generated as it is accessed using the Time.deltaTime to calculate the
     * portion of duration that has elapsed.
     */
    public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, float duration) {
        IEnumerable<float> timer = NgInterpolate.NewTimer(duration);
        return NewBezier<Transform>(ease, nodes, TransformDotPosition, duration, timer);
    }
 
    /**
     * Instead of interpolating based on time, generate n interpolated points
     * (slices) between the first and last node.
     */
    public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, int slices) {
        IEnumerable<float> counter = NewCounter(0, slices + 1, 1);
        return NewBezier<Transform>(ease, nodes, TransformDotPosition, slices + 1, counter);
    }
 
    /**
     * A Vector3[] variation of the Transform[] NewBezier() function.
     * Same functionality but using Vector3s to define bezier curve.
     */
    public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, float duration) {
        IEnumerable<float> timer = NewTimer(duration);
        return NewBezier<Vector3>(ease, points, Identity, duration, timer);
    }
 
    /**
     * A Vector3[] variation of the Transform[] NewBezier() function.
     * Same functionality but using Vector3s to define bezier curve.
     */
    public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, int slices) {
        IEnumerable<float> counter = NewCounter(0, slices + 1, 1);
        return NewBezier<Vector3>(ease, points, Identity, slices + 1, counter);
    }
 
    /**
     * Generic bezier spline sequence generator used to implement the time and
     * slice variants. Normally you would not use this function directly.
     */
    static IEnumerable<Vector3> NewBezier<T>(Function ease, IList nodes, ToVector3<T> toVector3, float maxStep, IEnumerable<float> steps) {
        // need at least two nodes to spline between
        if (nodes.Count >= 2) {
            // copy nodes array since Bezier is destructive
            Vector3[] points = new Vector3[nodes.Count];
 
            foreach (float step in steps) {
                // re-initialize copy before each destructive call to Bezier
                for (int i = 0; i < nodes.Count; i++) {
                    points[i] = toVector3((T)nodes[i]);
                }
                yield return Bezier(ease, points, step, maxStep);
                // make sure last value is always generated
            }
        }
    }
 
    /**
     * A Vector3 n-degree bezier spline.
     *
     * WARNING: The points array is modified by Bezier. See NewBezier() for a
     * safe and user friendly alternative.
     *
     * You can pass zero control points, just the start and end points, for just
     * plain easing. In other words a zero-degree bezier spline curve is just the
     * easing method.
     *
     * @param points start point, n control points, end point
     */
    static Vector3 Bezier(Function ease, Vector3[] points, float elapsedTime, float duration) {
        // Reference: http://ibiblio.org/e-notes/Splines/Bezier.htm
        // NcInterpolate the n starting points to generate the next j = (n - 1) points,
        // then interpolate those n - 1 points to generate the next n - 2 points,
        // continue this until we have generated the last point (n - (n - 1)), j = 1.
        // We store the next set of output points in the same array as the
        // input points used to generate them. This works because we store the
        // result in the slot of the input point that is no longer used for this
        // iteration.
        for (int j = points.Length - 1; j > 0; j--) {
            for (int i = 0; i < j; i++) {
                points[i].x = ease(points[i].x, points[i + 1].x - points[i].x, elapsedTime, duration);
                points[i].y = ease(points[i].y, points[i + 1].y - points[i].y, elapsedTime, duration);
                points[i].z = ease(points[i].z, points[i + 1].z - points[i].z, elapsedTime, duration);
            }
        }
        return points[0];
    }
 
    /**
     * Returns sequence generator from the first node, through each control point,
     * and to the last node. N points are generated between each node (slices)
     * using Catmull-Rom.
     */
    public static IEnumerable<Vector3> NewCatmullRom(Transform[] nodes, int slices, bool loop) {
        return NewCatmullRom<Transform>(nodes, TransformDotPosition, slices, loop);
    }
 
    /**
     * A Vector3[] variation of the Transform[] NewCatmullRom() function.
     * Same functionality but using Vector3s to define curve.
     */
    public static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, int slices, bool loop) {
        return NewCatmullRom<Vector3>(points, Identity, slices, loop);
    }
 
    /**
     * Generic catmull-rom spline sequence generator used to implement the
     * Vector3[] and Transform[] variants. Normally you would not use this
     * function directly.
     */
    static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, ToVector3<T> toVector3, int slices, bool loop) {
        // need at least two nodes to spline between
        if (nodes.Count >= 2) {
 
            // yield the first point explicitly, if looping the first point
            // will be generated again in the step for loop when interpolating
            // from last point back to the first point
            yield return toVector3((T)nodes[0]);
 
            int last = nodes.Count - 1;
            for (int current = 0; loop || current < last; current++) {
                // wrap around when looping
                if (loop && current > last) {
                    current = 0;
                }
                // handle edge cases for looping and non-looping scenarios
                // when looping we wrap around, when not looping use start for previous
                // and end for next when you at the ends of the nodes array
                int previous = (current == 0) ? ((loop) ? last : current) : current - 1;
                int start = current;
                int end = (current == last) ? ((loop) ? 0 : current) : current + 1;
                int next = (end == last) ? ((loop) ? 0 : end) : end + 1;
 
                // adding one guarantees yielding at least the end point
                int stepCount = slices + 1;
                for (int step = 1; step <= stepCount; step++) {
                    yield return CatmullRom(toVector3((T)nodes[previous]),
                                     toVector3((T)nodes[start]),
                                     toVector3((T)nodes[end]),
                                     toVector3((T)nodes[next]),
                                     step, stepCount);
                }
            }
        }
    }
 
    /**
     * A Vector3 Catmull-Rom spline. Catmull-Rom splines are similar to bezier
     * splines but have the useful property that the generated curve will go
     * through each of the control points.
     *
     * NOTE: The NewCatmullRom() functions are an easier to use alternative to this
     * raw Catmull-Rom implementation.
     *
     * @param previous the point just before the start point or the start point
     *                 itself if no previous point is available
     * @param start generated when elapsedTime == 0
     * @param end generated when elapsedTime >= duration
     * @param next the point just after the end point or the end point itself if no
     *             next point is available
     */
    static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, 
                                float elapsedTime, float duration) {
        // References used:
        // p.266 GemsV1
        //
        // tension is often set to 0.5 but you can use any reasonable value:
        // http://www.cs.cmu.edu/~462/projects/assn2/assn2/catmullRom.pdf
        //
        // bias and tension controls:
        // http://local.wasp.uwa.edu.au/~pbourke/miscellaneous/interpolation/
 
        float percentComplete = elapsedTime / duration;
        float percentCompleteSquared = percentComplete * percentComplete;
        float percentCompleteCubed = percentCompleteSquared * percentComplete;
 
        return previous * (-0.5f * percentCompleteCubed +
                                   percentCompleteSquared -
                            0.5f * percentComplete) +
                start   * ( 1.5f * percentCompleteCubed +
                           -2.5f * percentCompleteSquared + 1.0f) +
                end     * (-1.5f * percentCompleteCubed +
                            2.0f * percentCompleteSquared +
                            0.5f * percentComplete) +
                next    * ( 0.5f * percentCompleteCubed -
                            0.5f * percentCompleteSquared);
    }
 
 
 
 
    /**
     * Linear interpolation (same as Mathf.Lerp)
     */
    static float Linear(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * (elapsedTime / duration) + start;
    }
 
    /**
     * quadratic easing in - accelerating from zero velocity
     */
    static float EaseInQuad(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime + start;
    }
 
    /**
     * quadratic easing out - decelerating to zero velocity
     */
    static float EaseOutQuad(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return -distance * elapsedTime * (elapsedTime - 2) + start;
    }
 
    /**
     * quadratic easing in/out - acceleration until halfway, then deceleration
     */
    static float EaseInOutQuad(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime + start;
        elapsedTime--;
        return -distance / 2 * (elapsedTime * (elapsedTime - 2) - 1) + start;
    }
 
    /**
     * cubic easing in - accelerating from zero velocity
     */
    static float EaseInCubic(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime + start;
    }
 
    /**
     * cubic easing out - decelerating to zero velocity
     */
    static float EaseOutCubic(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return distance * (elapsedTime * elapsedTime * elapsedTime + 1) + start;
    }
 
    /**
     * cubic easing in/out - acceleration until halfway, then deceleration
     */
    static float EaseInOutCubic(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return distance / 2 * (elapsedTime * elapsedTime * elapsedTime + 2) + start;
    }
 
    /**
     * quartic easing in - accelerating from zero velocity
     */
    static float EaseInQuart(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
    }
 
    /**
     * quartic easing out - decelerating to zero velocity
     */
    static float EaseOutQuart(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1) + start;
    }
 
    /**
     * quartic easing in/out - acceleration until halfway, then deceleration
     */
    static float EaseInOutQuart(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return -distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2) + start;
    }
 
 
    /**
     * quintic easing in - accelerating from zero velocity
     */
    static float EaseInQuint(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
    }
 
    /**
     * quintic easing out - decelerating to zero velocity
     */
    static float EaseOutQuint(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1) + start;
    }
 
    /**
     * quintic easing in/out - acceleration until halfway, then deceleration
     */
    static float EaseInOutQuint(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2f);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2) + start;
    }
 
    /**
     * sinusoidal easing in - accelerating from zero velocity
     */
    static float EaseInSine(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return -distance * Mathf.Cos(elapsedTime / duration * (Mathf.PI / 2)) + distance + start;
    }
 
    /**
     * sinusoidal easing out - decelerating to zero velocity
     */
    static float EaseOutSine(float start, float distance, float elapsedTime, float duration) {
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * Mathf.Sin(elapsedTime / duration * (Mathf.PI / 2)) + start;
    }
 
    /**
     * sinusoidal easing in/out - accelerating until halfway, then decelerating
     */
    static float EaseInOutSine(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return -distance / 2 * (Mathf.Cos(Mathf.PI * elapsedTime / duration) - 1) + start;
    }
 
    /**
     * exponential easing in - accelerating from zero velocity
     */
    static float EaseInExpo(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * Mathf.Pow(2, 10 * (elapsedTime / duration - 1)) + start;
    }
 
    /**
     * exponential easing out - decelerating to zero velocity
     */
    static float EaseOutExpo(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * (-Mathf.Pow(2, -10 * elapsedTime / duration) + 1) + start;
    }
 
    /**
     * exponential easing in/out - accelerating until halfway, then decelerating
     */
    static float EaseInOutExpo(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 *  Mathf.Pow(2, 10 * (elapsedTime - 1)) + start;
        elapsedTime--;
        return distance / 2 * (-Mathf.Pow(2, -10 * elapsedTime) + 2) + start;
    }
 
    /**
     * circular easing in - accelerating from zero velocity
     */
    static float EaseInCirc(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return -distance * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start;
    }
 
    /**
     * circular easing out - decelerating to zero velocity
     */
    static float EaseOutCirc(float start, float distance, float elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return distance * Mathf.Sqrt(1 - elapsedTime * elapsedTime) + start;
    }
 
    /**
     * circular easing in/out - acceleration until halfway, then deceleration
     */
    static float EaseInOutCirc(float start, float distance, float
                         elapsedTime, float duration) {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return -distance / 2 * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start;
        elapsedTime -= 2;
        return distance / 2 * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) + 1) + start;
    }
}


// -------------------------------------------------------------------------------------------------------------
// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.
/*
var start = 0.0;
var distance = 3.0;
var duration = 2.0;
private var elapsedTime = 0.0;
 
function Update() {
    transform.position.y = NcInterpolate.EaseOutSine(start, distance,
                                                   elapsedTime, duration);
    elapsedTime += Time.deltaTime;
}

using UnityEngine;
using System.Collections;

public class Sample : MonoBehaviour
{
	Transform[] path; // path's control points
	bool  loop;
	// number of nodes to generate between path nodes, to smooth out the path
	int betweenNodeCount;
	private IEnumerator nodes;
	 
	void  Awake()
	{
	  nodes = (IEnumerator)NcInterpolate.NewCatmullRom(path, betweenNodeCount, loop);
	}
	 
	void  Update()
	{
	  if (nodes.MoveNext())
		transform.position = (Vector3)nodes.Current;
	}
	 
	// optional, use gizmos to draw the path in the editor
	void  OnDrawGizmos()
	{
	  if (path != null && 2 <= path.Length)
	  {
		// draw control points
		for (int i= 0; i < path.Length; i++)
		  Gizmos.DrawWireSphere(path[i].position, 0.15f);
	 
		// draw spline curve using line segments
		IEnumerator sequence		= (IEnumerator)NcInterpolate.NewCatmullRom(path, betweenNodeCount, loop);
		Vector3		firstPoint		= path[0].position;
		Vector3		segmentStart	= firstPoint;
		sequence.MoveNext(); // skip the first point
		// use "for in" syntax instead of sequence.MoveNext() when convenient
		foreach (Vector3 segmentEnd in sequence)
		{
		  Gizmos.DrawLine(segmentStart, segmentEnd);
		  segmentStart = segmentEnd;
		  // prevent infinite loop, when attribute loop == true
		  if (segmentStart == firstPoint)
			  break;
		}
	  }
	}
}
*/
