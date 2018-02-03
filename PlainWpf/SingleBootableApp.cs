using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlainWpf
{
    /// <summary>
    /// 2重起動防止アプリケーションクラス
    /// http://kisuke0303.sakura.ne.jp/blog/?p=182 WPF で二重起動を防止する
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

        #region 2重起動防止コード
        /// <summary>
        /// コンストラクタでアプリケーション名を習得
        /// </summary>
        private string appName;
        /// <summary>
        /// 起動フラグ用ミューテックス
        /// アプリケーション名使用
        /// </summary>
        private System.Threading.Mutex mutex;
        /// <summary>
        /// スタートアップ時のミューテックスのチェックと拾得
        /// </summary>
        private void DoubleBootCheckAndRegist()
        {
            // ミューテックスの所有権を要求
            if (!mutex.WaitOne(0, false))
            {
                // 既に起動しているため終了させる
                MessageBox.Show(appName + " は既に起動しています。", "二重起動防止", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                mutex.Close();
                mutex = null;
                this.Shutdown();
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
        #endregion
    }
}
