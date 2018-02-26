using Utilitys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvvm;

namespace PlainWpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private bool appEnabled;
        public bool AppEnabled
        {
            get { return appEnabled; }
            set { SetProperty(ref appEnabled, value); }
        }

        public MainWindowViewModel()
        {
            Title = "012";
            MavigateAnimation = new DelegateCommand<bool>(animation => {
                AppEnabled = !animation;
            });
        }

        public DelegateCommand<bool> MavigateAnimation { get; private set; }
    }
}
