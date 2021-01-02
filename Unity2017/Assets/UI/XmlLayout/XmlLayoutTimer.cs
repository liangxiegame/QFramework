using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace UI.Xml
{
    internal class DelayedEditorAction
    {
        internal double TimeToExecute;
        internal Action Action;
        internal MonoBehaviour ActionTarget;

        public DelayedEditorAction(double timeToExecute, Action action, MonoBehaviour actionTarget)
        {
            TimeToExecute = timeToExecute;
            Action = action;
            ActionTarget = actionTarget;
        }
    }

    public static class XmlLayoutTimer
    {
#if UNITY_EDITOR
        static List<DelayedEditorAction> delayedEditorActions = new List<DelayedEditorAction>();

        static XmlLayoutTimer()
        {
            //if (!Application.isPlaying) UnityEditor.EditorApplication.update += EditorUpdate;
            UnityEditor.EditorApplication.update += EditorUpdate;
        }
#endif

        static void EditorUpdate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;

            var actionsToExecute = delayedEditorActions.Where(dea => UnityEditor.EditorApplication.timeSinceStartup >= dea.TimeToExecute).ToList();

            if (!actionsToExecute.Any()) return;

            foreach (var actionToExecute in actionsToExecute)
            {
                try
                {
                    if (actionToExecute.ActionTarget != null) // don't execute if the target is gone
                    {
                        actionToExecute.Action.Invoke();
                    }
                }
                finally
                {
                    delayedEditorActions.Remove(actionToExecute);
                }
            }
#endif
        }

        public static void DelayedCall(float delay, Action action, MonoBehaviour actionTarget)
        {
            if (Application.isPlaying)
            {
                if (actionTarget.gameObject.activeInHierarchy) actionTarget.StartCoroutine(_DelayedCall(delay, action));
            }
#if UNITY_EDITOR
            else
            {
                delayedEditorActions.Add(new DelayedEditorAction(UnityEditor.EditorApplication.timeSinceStartup + delay, action, actionTarget));
            }
#endif
        }

        private static IEnumerator _DelayedCall(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);

            action.Invoke();
        }
    }
}
