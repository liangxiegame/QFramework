using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public abstract class ActionKitVisualEvent : MonoBehaviour
    {
        [HideInInspector]
        public List<ActionKitVisualAction> Acitons;

        public void Execute()
        {
            if (Acitons != null && Acitons.Count != 0)
            {
                var sequenceNode = new SequenceNode();
                
                foreach (var actionKitVisualAction in Acitons)
                {
                    sequenceNode.Append(actionKitVisualAction);
                }
                
                this.ExecuteNode(sequenceNode);
            }
        }
    }
}