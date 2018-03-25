using Mvvm;
using MvvmOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PlainWpf.ViewModels
{
    // https://qiita.com/takutoy/items/d45aa736ced25a8158b3 C# Taskの待ちかた集
    // http://nineworks2.blog.fc2.com/blog-entry-4.html asyncとIProgressを使ってプログレスバーを操作する
    public class ThreadPageViewModel : BindableBase
    {
        private string loopCount;
        public string LoopCount
        {
            get { return loopCount; }
            set
            {
                loopCount = value;
                OnPropertyChanged();
            }
        }
        private string result;
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand RunCommand { get; set; }

        public ThreadPageViewModel()
        {
            RunCommand = new DelegateCommand(() => {
                if (long.TryParse(LoopCount, out long loop))
                {
                    Result = "Task開始";
                    if (loop < 0)
                        loop *= -1;
                    var result = MessengerService.SendMessage(typeof(ThreadPageViewModel), new ProgressDialogRequestMessage
                    {
                        Title = "0123456",
                        Content = "0123456",
                        WorkAction = x =>
                        {
                            for (long l = 0; l < loop; ++l)
                            {
                                if (x.IsCancellationRequested)
                                    break;
                            }
                        },
                        WorkCompleted = x =>
                        {
                            Result = x == ProgressDialogResult.Completed ? "Taskは完了しました" : "Taskはキャンセルしました";
                        }
                    });
                }
                else
                {
                    Result = "ループ回数が不正です";
                }
            });
        }
    }
}
