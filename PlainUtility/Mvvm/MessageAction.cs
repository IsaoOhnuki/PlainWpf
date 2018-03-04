using Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mvvm
{
    public class DialogBoxAction : IViewAction
    {
        private static void ShowMessage(DialogBoxMessage msg)
        {
            var result = MessageBox.Show(msg.Message, "確認", msg.Button);
            msg.Result = result;
        }

        public void Register(FrameworkElement recipient)
        {
            Messenger.Default.Register<DialogBoxMessage>(recipient, ShowMessage);
        }
    }
}
