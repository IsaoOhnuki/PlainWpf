using Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvvm;

namespace PlainWpf.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Mvvm.BindableBase" />
    public class SubPage3ViewModel : BindableBase
    {
        public SubPage3ViewModel()
        {
            //var act = new Func<>
        }

        private DateTime now = DateTime.Now;
        public DateTime Now
        {
            get { return now; }
            set { SetProperty(ref now, value); }
        }

        private DelegateCommand nowCommand;
        public DelegateCommand NowCommand { get { return nowCommand ?? (nowCommand = new DelegateCommand(NowCommandAction)); } }

        public void NowCommandAction()
        {
            Now = DateTime.Now;
        }
    }
}
