using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.GraphDesigner.Unity.KoinoniaSystem.Commands;
using QF.GraphDesigner.Unity.WindowsPlugin;
using QF.GraphDesigner;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.ViewModels
{
    public class LoginScreenViewModel : WindowViewModel
    {

        public string Username { get; set; }
        public string Password { get; set; }

    

        public void Login()
        {
            InvertApplication.ExecuteInBackground(new LoginCommand()
            {
                Password = Password,
                Username = Username
            });
        }

    }
}
