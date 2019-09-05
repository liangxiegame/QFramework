using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Common;
using QF.GraphDesigner.Systems.GraphUI;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class DebugPanelUISystem : DiagramPlugin, IOverlayDrawer, IQueryDesignerWindowOverlayContent
    {
        private IPlatformDrawer _platformDrawer;

        public IPlatformDrawer PlatformDrawer
        {
            get { return _platformDrawer ?? (_platformDrawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
            set { _platformDrawer = value; }
        }

        public void QueryDesignerWindowOverlayContent(List<DesignerWindowOverlayContent> content)
        {
            if (EditorApplication.isPaused)
            {
                content.Add(new DesignerWindowOverlayContent()
                {
                    Drawer = this
                });
            }
            
        }

        public void Draw(Rect bouds)
        {

            //PlatformDrawer.DrawStretchBox(bouds, CachedStyles.BreadcrumbBoxStyle, 11);
           

            bouds = bouds.Pad(15,0,0,0);

            var stepButtonRect = new Rect().Align(bouds).WithSize(75, 65);
            var continueButtonRect = stepButtonRect.RightOf(stepButtonRect);

            var stepIconRect = new Rect().WithSize(31, 31).Align(stepButtonRect).AlignVerticallyByCenter(stepButtonRect).Translate(0,7);
            var continueIconRect = new Rect().WithSize(31, 31).Align(continueButtonRect).AlignVerticallyByCenter(continueButtonRect).Translate(0, 7);

            var stepTitleRectr = new Rect().WithSize(75, 20).InnerAlignWithBottomCenter(stepButtonRect).Translate(0,-5);
            var continueTitleRectr = new Rect().WithSize(75, 20).InnerAlignWithBottomCenter(continueButtonRect).Translate(0, -5);


            PlatformDrawer.SetTooltipForRect(stepButtonRect,"Step to the next action");
            PlatformDrawer.DoButton(stepButtonRect,"", CachedStyles.WizardActionButtonStyle, DoStep);

            PlatformDrawer.SetTooltipForRect(continueButtonRect, "Continue normal execution");
            PlatformDrawer.DoButton(continueButtonRect, "", CachedStyles.WizardActionButtonStyle, DoContinue);

            PlatformDrawer.DrawImage(stepIconRect,"StepIcon",true);
            PlatformDrawer.DrawImage(continueIconRect, "PlayIcon", true);

            PlatformDrawer.DrawLabel(stepTitleRectr,"Step",CachedStyles.BreadcrumbTitleStyle,DrawingAlignment.MiddleCenter);
            PlatformDrawer.DrawLabel(continueTitleRectr, "Continue", CachedStyles.BreadcrumbTitleStyle, DrawingAlignment.MiddleCenter);

        }

        public void DoContinue()
        {
            
            Execute(new ContinueCommand());
        }

        public void DoStep()
        {
            Execute(new StepCommand());
        }


        public Rect CalculateBounds(Rect diagramRect)
        {   
            return new Rect().WithSize(75*2+30, 65).InnerAlignWithBottomLeft(diagramRect).Translate(0,-35);
        }

    }
}
