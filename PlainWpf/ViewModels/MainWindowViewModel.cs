using Utilitys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public DelegateCommand SubPage1ShowCommand { get; set; }

        public MainWindowViewModel()
        {
            Title = "012";
            SubPage1ShowCommand = new DelegateCommand(() => {
                
            });
        }
    }
}
