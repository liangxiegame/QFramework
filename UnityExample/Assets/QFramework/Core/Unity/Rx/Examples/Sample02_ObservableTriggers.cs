using UnityEngine;
 // Triggers Namepsace

namespace QFramework.Examples
{
    public class Sample02_ObservableTriggers : MonoBehaviour
    {
        void Start()
        {
            // Get the plain object
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // Add ObservableXxxTrigger for handle MonoBehaviour's event as Observable
            cube.AddComponent<ObservableUpdateTrigger>()
                .UpdateAsObservable()
                .SampleFrame(30)
                .Subscribe(x => Debug.Log("cube"), () => Debug.Log("destroy"));

            // destroy after 3 second:)
            GameObject.Destroy(cube, 3f);
        }
    }
}