using Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainWpf.ViewModels
{
    public class ViewBoxTestViewModel
    {
        public DelegateCommand ShowWindowCommand { get; private set; }
        public DelegateCommand ShowOkDialogCommand { get; private set; }
        public DelegateCommand ShowOkCancelDialogCommand { get; private set; }
        public DelegateCommand ShowYesNoCancelDialogCommand { get; private set; }
        public DelegateCommand OpenFileDialogCommand { get; private set; }
        public DelegateCommand SaveFileDialogCommand { get; private set; }

        public ViewBoxTestViewModel()
        {
            ShowOkDialogCommand = new DelegateCommand(() => {
                var result = MessengerService.SendMessage(typeof(ViewBoxTestViewModel), new OkDialogRequestMessage { Title = "0123456", Content = "0123456" });
            });
            ShowOkCancelDialogCommand = new DelegateCommand(() => {
                var result = MessengerService.SendMessage(typeof(ViewBoxTestViewModel), new OkCancelDialogRequestMessage { Title = "0123456", Content = "0123456" });
            });
            ShowYesNoCancelDialogCommand = new DelegateCommand(() => {
                var result = MessengerService.SendMessage(typeof(ViewBoxTestViewModel), new YesNoCancelDialogRequestMessage { Title = "0123456", Content = "0123456" });
            });
            ShowWindowCommand = new DelegateCommand(() => {
                var result = MessengerService.SendMessage(typeof(ViewBoxTestViewModel), new UserControl1Message { Title = "0123456", Content = "0123456" });
            });
            OpenFileDialogCommand = new DelegateCommand(() => {
                var result = MessengerService.SendMessage(typeof(ViewBoxTestViewModel), new OpenFileDialogMessage { Title = "0123456", Content = "0123456" });
            });
            SaveFileDialogCommand = new DelegateCommand(() => {
                var result = MessengerService.SendMessage(typeof(ViewBoxTestViewModel), new SaveFileDialogMessage { Title = "0123456", Content = "0123456" });
            });
        }
    }
}
