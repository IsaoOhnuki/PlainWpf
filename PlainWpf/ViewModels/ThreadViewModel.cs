using Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainWpf.ViewModels
{
    public class ThreadViewModel
    {
        public DelegateCommand RunCommand { get; set; }

        public ThreadViewModel()
        {
            RunCommand = new DelegateCommand(() => {
            });
        }
    }
}
