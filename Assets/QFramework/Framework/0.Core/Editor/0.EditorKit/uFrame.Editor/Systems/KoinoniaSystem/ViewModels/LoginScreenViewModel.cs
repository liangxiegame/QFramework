using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QFramework.GraphDesigner;
using QFramework.GraphDesigner.Unity.KoinoniaSystem.Commands;
using QFramework.GraphDesigner.Unity.WindowsPlugin;

namespace QFramework.GraphDesigner.Unity.KoinoniaSystem.ViewModels
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
