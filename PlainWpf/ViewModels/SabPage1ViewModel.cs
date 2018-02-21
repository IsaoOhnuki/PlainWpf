using Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainWpf.ViewModels
{
    public class SabPage1ViewModel : BindableBase
    {
        private DelegateCommand<string[]> dragDropCommand;

        public DelegateCommand<string[]> DragDropCommand { get { return dragDropCommand = dragDropCommand ?? new DelegateCommand<string[]>(DragDropHandler); } }

        private void DragDropHandler(string[] dropFiles)
        {

        }
    }
}
