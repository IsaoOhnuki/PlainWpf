using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MvvmHelper
{
    /// <summary>
    /// ページナビゲーションを行うクラス
    /// </summary>
    /// <url name="http://sourcechord.hatenablog.com/entry/2016/02/01/003758">WPFでシンプルな独自ナビゲーション処理のサンプルを書いてみた</url>
    public class NavigationServiceEx : DependencyObject
    {
        /// <summary>
        /// ページナビゲーションを行う領域となるContentControlを保持するAccessor
        /// </summary>
        public ContentControl Content
        {
            get { return (ContentControl)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// ページナビゲーションを行う領域となるContentControlを保持するプロパティ
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
            typeof(ContentControl),
            typeof(NavigationServiceEx),
            new PropertyMetadata(null));

        #region ナビゲーションで利用する各種メソッド
        /// <summary>
        /// view引数で指定されたインスタンスのページへとナビゲーションを行います。
        /// </summary>
        /// <param name="view"></param>
        /// <returns>成功True、失敗False</returns>
        public bool Navigate(FrameworkElement view)
        {
            try
            {
                this.Content.Content = view;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// viewType引数で指定された型のインスタンスを生成し、そのインスタンスのページへとナビゲーションを行います。
        /// </summary>
        /// <param name="viewType">遷移するページのType</param>
        /// <returns>成功True、失敗False</returns>
        public bool Navigate(Type viewType)
        {
            if (viewType == null)
            {
                this.Navigate((FrameworkElement)null);
                return false;
            }
            var view = Activator.CreateInstance(viewType) as FrameworkElement;
            return this.Navigate(view);
        }
        #endregion

        /// <summary>
        /// NavigationCommands.GoToPageコマンドに対する応答処理
        /// </summary>
        /// <param name="sender">未使用</param>
        /// <param name="e">e.Parameterに遷移するページのType</param>
        private void OnGoToPage(object sender, ExecutedRoutedEventArgs e)
        {
            var nextPage = e.Parameter as Type;
            this.Navigate(nextPage);
        }

        #region ページナビゲーションを行う領域となるContentControlを指定するための添付プロパティ
        // この添付プロパティで指定した値は、NavigationServiceEx.Contentプロパティとバインドして同期するようにして扱う。
        public static ContentControl GetTarget(DependencyObject obj)
        {
            return (ContentControl)obj.GetValue(TargetProperty);
        }
        public static void SetTarget(DependencyObject obj, ContentControl value)
        {
            obj.SetValue(TargetProperty, value);
        }
        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty = DependencyProperty.RegisterAttached(
            "Target",
            typeof(ContentControl),
            typeof(NavigationServiceEx),
            new PropertyMetadata(null, OnTargetChanged));

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            var target = e.NewValue as ContentControl;

            if (element != null && target != null)
            {
                // NavigationServiceExのインスタンスを、添付対象のコントロールに付加する。
                var nav = new NavigationServiceEx();
                NavigationServiceEx.SetNavigator(element, nav);

                // ContentプロパティとTargetをバインドしておく。
                BindingOperations.SetBinding(nav, NavigationServiceEx.ContentProperty, new Binding() { Source = target });

                // ナビゲーション用のコマンドバインディング
                element.CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage, nav.OnGoToPage));

                var startup = NavigationServiceEx.GetStartup(element);
                if (startup != null)
                {
                    nav.Navigate(startup);
                }
            }
        }
        #endregion

        #region スタートアップ時に表示するページを指定するための添付プロパティ
        public static Type GetStartup(DependencyObject obj)
        {
            return (Type)obj.GetValue(StartupProperty);
        }
        public static void SetStartup(DependencyObject obj, Type value)
        {
            obj.SetValue(StartupProperty, value);
        }
        // Using a DependencyProperty as the backing store for Startup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartupProperty = DependencyProperty.RegisterAttached(
            "Startup",
            typeof(Type),
            typeof(NavigationServiceEx),
            new PropertyMetadata(null, OnStartupChanged));

        private static void OnStartupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            var startupType = e.NewValue as Type;

            if (element != null && startupType != null)
            {
                var nav = NavigationServiceEx.GetNavigator(element);
                nav?.Navigate(startupType);
            }
        }
        #endregion

        #region ページ遷移時のアニメーション
        /// <summary>
        /// ページ遷移時のアニメーション添付プロパティのGetter
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Storyboard GetStoryboard(DependencyObject obj)
        {
            return (Storyboard)obj.GetValue(StartupProperty);
        }

        /// <summary>
        /// ページ遷移時のアニメーション添付プロパティのSetter
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetStoryboard(DependencyObject obj, Storyboard value)
        {
            obj.SetValue(StartupProperty, value);
        }

        /// <summary>
        /// ページ遷移時のアニメーション添付プロパティ
        /// </summary>
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.RegisterAttached(
            "Storyboard",
            typeof(Storyboard),
            typeof(NavigationServiceEx),
            new PropertyMetadata(null, OnStoryboardChanged));

        /// <summary>
        /// ページ遷移時のアニメーション添付プロパティのハンドラ
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            var storyboard = e.NewValue as Type;

            if (element != null)
            {

            }
        }
        #endregion

        #region 任意のコントロールに対して、NavigationServiceExをアタッチできるようにするための添付プロパティ
        public static NavigationServiceEx GetNavigator(DependencyObject obj)
        {
            return (NavigationServiceEx)obj.GetValue(NavigatorProperty);
        }
        // ↓protectedにして外部からは利用できないように。
        public static void SetNavigator(DependencyObject obj, NavigationServiceEx value)
        {
            obj.SetValue(NavigatorProperty, value);
        }
        // Using a DependencyProperty as the backing store for Navigator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigatorProperty = DependencyProperty.RegisterAttached(
            "Navigator",
            typeof(NavigationServiceEx),
            typeof(NavigationServiceEx),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        #endregion
    }

    public static class NavigationServiceExtensions
    {
        /// <summary>
        /// view引数で指定されたインスタンスのページへとナビゲーションを行います。
        /// </summary>
        /// <param name="element"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static bool Navigate(this FrameworkElement element, FrameworkElement view)
        {
            var navigator = NavigationServiceEx.GetNavigator(element);
            return navigator.Navigate(view);
        }

        /// <summary>
        /// viewType引数で指定された型のインスタンスを生成し、そのインスタンスのページへとナビゲーションを行います。
        /// </summary>
        /// <param name="element"></param>
        /// <param name="viewType"></param>
        /// <returns></returns>
        public static bool Navigate(this FrameworkElement element, Type viewType)
        {
            var navigator = NavigationServiceEx.GetNavigator(element);
            return navigator.Navigate(viewType);
        }
    }
}
