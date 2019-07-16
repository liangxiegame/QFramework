using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class ZenjectStateMachineBehaviourAutoInjecter : MonoBehaviour
    {
        DiContainer _container;
        Animator _animator;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
            _animator = GetComponent<Animator>();
            Assert.IsNotNull(_animator);
        }

        // The unity docs (https://unity3d.com/learn/tutorials/modules/beginner/5-pre-order-beta/state-machine-behaviours)
        // mention that StateMachineBehaviour's should only be retrieved in the Start method
        // which is why we do it here
        public void Start()
        {
            // Animator can be null when users create GameObjects directly so in that case
            // Just don't bother attempting to inject the behaviour classes
            if (_animator != null)
            {
                var behaviours = _animator.GetBehaviours<StateMachineBehaviour>();

                if (behaviours != null)
                {
                    foreach (var behaviour in behaviours)
                    {
                        _container.Inject(behaviour);
                    }
                }
            }
        }
    }
}
