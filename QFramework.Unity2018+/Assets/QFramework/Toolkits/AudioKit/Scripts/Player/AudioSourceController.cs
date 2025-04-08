using UnityEngine;

namespace QFramework
{
    public class AudioSourceController : IPoolable,IPoolType
    {
        public AudioSource AudioSource;

        public static AudioSourceController Allocate(GameObject root,string name)
        {
            var controller = SafeObjectPool<AudioSourceController>.Instance.Allocate();

            if (!controller.AudioSource)
            {
                controller.AudioSource = new GameObject(name)
                    .AddComponent<AudioSource>();
            }

            controller.AudioSource
                .Parent(root)
                .Name(name)
                .Show();

            return controller;
        }

        public void OnRecycled()
        {
            AudioSource
                .Parent(AudioManager.Instance)
                .Hide();
        }

        public bool IsRecycled { get; set; }
        
        public void Recycle2Cache()
        {
            SafeObjectPool<AudioSourceController>.Instance.Recycle(this);
        }

        public void Update(GameObject root, string name)
        {
            AudioSource
                .Parent(root)
                .Name(name)
                .Show();
        }
    }
}