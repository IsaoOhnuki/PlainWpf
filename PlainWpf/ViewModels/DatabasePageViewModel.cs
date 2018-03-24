using Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseUtility;

namespace PlainWpf.ViewModels
{
    public class DatabasePageViewModel
    {
        private DelegateCommand showDatabaseDialogCommand;
        public DelegateCommand ShowDatabaseDialogCommand { get; set; }

        public DatabasePageViewModel()
        {
            ShowDatabaseDialogCommand = new DelegateCommand(() => {
                string connection = "";
                var ret = SQLServerConfigurationDialog.ShowDialog(ref connection, DialogInitializeType.ServerAndDatabaseSelect, ServerConnectionAuthenticate.Windows);
            });
        }
    }
}
