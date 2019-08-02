using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{

    public class ProblemUISystem : DiagramPlugin
        , IDrawProblem
        , IShowExceptionDetails
    {

        private IPlatformDrawer _platformDrawer;

        public IPlatformDrawer PlatformDrawer
        {
            get { return _platformDrawer ?? (_platformDrawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
            set { _platformDrawer = value; }
        }

        public void DrawProblemInspector(Rect bounds, Problem problem)
        {
            var headerRect = bounds.WithHeight(30);
            PlatformDrawer.DrawLabel(headerRect, problem.Exception.Message, CachedStyles.WizardActionTitleStyle, DrawingAlignment.MiddleCenter);

            var stackFrames = problem.StackTrace.GetFrames();
            if (stackFrames == null) return;

            var lastLineRect = headerRect;
            
            foreach (var frame in stackFrames)
            {
                var lineRect = bounds.WithHeight(15).Below(lastLineRect);

                PlatformDrawer.DrawLabel(lineRect ,frame.GetMethod().DeclaringType.Name+"."+frame.GetMethod().Name + " at " +frame.GetFileName(), CachedStyles.ListItemTitleStyle, DrawingAlignment.MiddleCenter);


                lastLineRect = lineRect;
            }

        }

        public void ShowExceptionDetails(Problem exception)
        {
            var window = EditorWindow.GetWindow<ProblemWindow>();
            window.Problem = exception;
            window.minSize = new Vector2(300,500);
         
            window.Show();
            window.Repaint();
            window.Focus();
        }
    }
}

