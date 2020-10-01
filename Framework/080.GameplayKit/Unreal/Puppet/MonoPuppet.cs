using UnityEngine;

namespace QFramework
{
    public class MonoPuppet : MonoBehaviour, IPuppet
    {
        public Controller Controller { get; set; }
    }
}