using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PlainWpf
{
    /// <summary>
    /// 2重起動防止アプリケーションクラス
    /// http://kisuke0303.sakura.ne.jp/blog/?p=182 WPF で二重起動を防止する
    /// http://www.geocities.jp/litorud/WPF.html#9 二重起動を防止し、起動中のウィンドウをアクティブにする
    /// 
    /// スタートアップクラスAppの基底クラスApplicationと差し替えて使う
    /// xamlの基底クラスも差し替える
    /// </summary>
    public class SingleBootableApp : Application
    {
        public SingleBootableApp()
        {
            appName = this.GetType().Assembly.GetName().ToString();
            mutex = new System.Threading.Mutex(false, appName);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DoubleBootCheckAndRegist();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DoubleBootRelease();
        }

        #region 2重起動防止コード + 起動されているアプリの表示
        /// <summary>
        /// 実際はUUIDなどが良い。
        /// </summary>
        const string applicationId = "00000000-0000-0000-0000-000000000000";
        /// <summary>
        /// コンストラクタでアプリケーション名を習得
        /// </summary>
        private string appName;
        /// <summary>
        /// 起動フラグ用ミューテックス
        /// アプリケーション名使用
        /// </summary>
        private static System.Threading.Mutex mutex;
        /// <summary>
        /// スタートアップ時のミューテックスのチェックと拾得
        /// </summary>
        private void DoubleBootCheckAndRegist()
        {
            // ミューテックスの所有権を要求
            if (!mutex.WaitOne(0, false))
            {
                MessageBox.Show(appName + " は既に起動しています。", "二重起動防止", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                // 起動されているアプリの表示
                ChannelServices.RegisterChannel(new IpcClientChannel(), true);
                ((IpcHandler)Activator.GetObject(typeof(IpcHandler), "ipc://" + applicationId + "/" + appName)).Handle();

                // 既に起動しているため終了させる
                mutex.Close();
                mutex = null;
                this.Shutdown();
            }
            else
            {
                // セマフォの登録
                ChannelServices.RegisterChannel(new IpcServerChannel(applicationId), true);
                RemotingServices.Marshal(new IpcHandler(), appName, typeof(IpcHandler));
            }
        }
        /// <summary>
        /// 終了時のミューテックスの解放
        /// </summary>
        private void DoubleBootRelease()
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Close();
            }
        }

        /// <summary>
        /// IPC用ハンドラ
        /// </summary>
        private class IpcHandler : MarshalByRefObject
        {
            public void Handle()
            {
                Current.Dispatcher.Invoke(() => {
                    Current.MainWindow.Activate();
                    if (Current.MainWindow.WindowState == WindowState.Minimized)
                    {
                        Current.MainWindow.WindowState = WindowState.Normal;
                    }
                });
            }

            public override object InitializeLifetimeService() => null;
        }
        #endregion
    }
}
