using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using QF.GraphDesigner;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Commands
{
    public class LoginCommand : IBackgroundCommand
    {
        public string Title
        {
            get { return "Login to Invert Empire"; }
            set
            {
                
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public BackgroundWorker Worker { get; set; }
    }




}
