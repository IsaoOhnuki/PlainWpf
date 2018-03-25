using Mvvm;
using MvvmOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainWpf.ViewModels
{
    // https://qiita.com/takutoy/items/d45aa736ced25a8158b3 C# Taskの待ちかた集
    // http://nineworks2.blog.fc2.com/blog-entry-4.html asyncとIProgressを使ってプログレスバーを操作する
    public class ThreadPageViewModel : BindableBase
    {
        public string Result { get; set; }

        public DelegateCommand RunCommand { get; set; }

        public ThreadPageViewModel()
        {
            RunCommand = new DelegateCommand(() => {
                var result = MessengerService.SendMessage(typeof(ThreadPageViewModel), new ProgressDialogRequestMessage { Title = "0123456", Content = "0123456" });
            });
        }
    }
}
