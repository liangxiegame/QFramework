using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class InteractiveTutorial 
    {
        public string Name { get; set; }
        private List<ITutorialStep> _steps;
        private bool _lastStepCompleted = true;

        public InteractiveTutorial(string name)
        {
            Name = name;
        }

        public List<ITutorialStep> Steps
        {
            get { return _steps ?? (_steps = new List<ITutorialStep>()); }
            set { _steps = value; }
        }

        public bool LastStepCompleted
        {
            get { return _lastStepCompleted; }
            set { _lastStepCompleted = value; }
        }
    }
}