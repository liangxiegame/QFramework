using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public abstract class ActionKitVisualEvent : MonoBehaviour
    {
        [HideInInspector]
        public List<ActionKitVisualAction> Acitons;

        private SequenceNode mSequenceNode;
        public void Execute()
        {
            if (Acitons != null && Acitons.Count != 0)
            {
                if (mSequenceNode == null)
                {
                    mSequenceNode = new SequenceNode();

                    foreach (var actionKitVisualAction in Acitons)
                    {
                        mSequenceNode.Append(actionKitVisualAction);
                    }
                }

                this.ExecuteNode(mSequenceNode);
            }
        }
    }
}