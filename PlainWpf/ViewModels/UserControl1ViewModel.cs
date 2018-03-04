using Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainWpf.ViewModels
{
    class UserControl1ViewModel : BindableBase, IMessageRecipient
    {
        private IMessage message;
        public IMessage Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public bool Closer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    public class UserControl1Message : IMessage
    {
        public string Title { get; set; }
        public object Content { get; set; }
    }
}
