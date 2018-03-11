using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Controls
{
    public class NoControlboxWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_STYLE = -16;
        const int WS_SYSMENU = 0x80000;

        public bool ControlboxEnabled { get; set; } = false;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (!ControlboxEnabled)
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;
                int style = GetWindowLong(handle, GWL_STYLE);
                style = style & (~WS_SYSMENU);
                SetWindowLong(handle, GWL_STYLE, style);
            }
        }
    }
}
