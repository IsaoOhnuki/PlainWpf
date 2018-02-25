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
using Utilitys;

namespace PlainWpf
{
    /// <summary>
    /// WPFアプリケーション基底クラス<br/>
    /// スタートアップクラスAppの基底クラスApplicationと差し替えて使う、xamlの基底クラスも差し替える<br/>
    /// 2重起動防止機能<br/>
    /// <see cref="OnConstructor"/>をオーバーライドしてアプリケーションの開始処理を行える<br/>
    /// <see cref="OnDestructor"/>をオーバーライドしてアプリケーションの終了処理を行える<br/>
    /// <see cref="OnUnhandledException"/>をオーバーライドしてキャッチされなかった例外処理を行える<br/>
    /// </summary>
    /// <remarks><a href="http://kisuke0303.sakura.ne.jp/blog/?p=182">WPF で二重起動を防止する</a><br/>
    /// <a href="http://www.geocities.jp/litorud/WPF.html#9">二重起動を防止し、起動中のウィンドウをアクティブにする</a></remarks>
    public class SingleBootableApp : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleBootableApp"/> class.
        /// </summary>
        public SingleBootableApp()
        {
            appName = this.GetType().Assembly.GetName().ToString();
            mutex = new System.Threading.Mutex(false, appName);
        }

        /// <summary>
        /// 二重起動防止の処理を行い、<see cref="OnConstructor()"/>を呼び出す<br/>
        ///   <see cref="E:System.Windows.Application.Startup" /> イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している <see cref="T:System.Windows.StartupEventArgs" />。</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += SingleBootableApp_DispatcherUnhandledException;

            OnConstructor();
        }

        /// <summary>
        /// キャッチされなかった例外のハンドラ、<see cref="OnUnhandledException"/>をコールして終了コードを拾得する
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Threading.DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void SingleBootableApp_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            int errorCode = -1;
            OnUnhandledException(e.Exception, ref errorCode);

            e.Handled = true;
            Shutdown(errorCode);
        }

        /// <summary>
        /// Called when [unhandled exception].
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="errorCode">The error code.</param>
        public virtual void OnUnhandledException(Exception e, ref int errorCode)
        {
            Logger.Write(e, "application exception tarminate handle.");
        }

        /// <summary>
        /// Called when [constructor].
        /// </summary>
        public virtual void OnConstructor()
        {
            Logger.Write(LogType.Information, "application start.");

            DoubleBootCheckAndRegist();
        }

        private bool destract = false;
        /// <summary>
        /// Called when [destructor].
        /// </summary>
        public virtual void OnDestructor()
        {
            Logger.Write(LogType.Information, "application tarminate.");

            DoubleBootRelease();

            destract = true;
        }

        /// <summary>
        ///   <see cref="E:System.Windows.Application.Exit" /> イベントを発生させます。
        /// </summary>
        /// <param name="e">イベント データを格納している <see cref="T:System.Windows.ExitEventArgs" />。</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (!destract)
                OnDestructor();

            Logger.Write(LogType.Information, "application exit : code " + e.ApplicationExitCode.ToString() + ".");
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

                Logger.Write(LogType.Information, "application starting duplication.");

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
