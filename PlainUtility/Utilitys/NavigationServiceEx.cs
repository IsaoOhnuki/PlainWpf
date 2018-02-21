using Behaviours;
using Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Utilitys
{
    /// <summary>
    /// ページ遷移時のアニメーションを実行するインターフェース
    /// </summary>
    public interface INavigationStory
    {
        /// <summary>
        /// アニメーション再生の為のイニシャライザ
        /// </summary>
        /// <param name="fromContent">遷移元ページ</param>
        /// <param name="toContent">遷移先ページ</param>
        /// <param name="endAnimation">アニメーション終了時のイベントハンドラ</param>
        /// <returns>アニメーションを再生するコンテント</returns>
        FrameworkElement Initialize(FrameworkElement fromContent, FrameworkElement toContent, Action<FrameworkElement> endAnimation);
        /// <summary>
        /// アニメーション再生
        /// </summary>
        void Animate();
    }
    /// <summary>
    /// シンプルなワイプ系のアニメーション
    /// </summary>
    /// <remarks><a href="http://techoh.net/wpf-control-storyboard-with-code/">2分でできるC#コードからWPFのアニメーションを操る方法</a></remarks>
    public class SimpleWipeNavigationStory : INavigationStory
    {
        /// <summary>
        /// ワイプアニメーションのタイプ定義
        /// </summary>
        public enum WipeNavigationMode
        {
            /// <summary>
            /// 遷移先が中心から広がる
            /// </summary>
            Spread,
            /// <summary>
            /// 遷移先が水平方向に広がる
            /// </summary>
            HorizontalSpread,
            /// <summary>
            /// 遷移先が垂直方向に広がる
            /// </summary>
            VerticalSpread,
            /// <summary>
            /// 遷移元が中心に窄まる
            /// </summary>
            Narrowed,
            /// <summary>
            /// 遷移元が左右から中心に窄まる
            /// </summary>
            HorizontalNarrowed,
            /// <summary>
            /// 遷移元が上下から中心に窄まる
            /// </summary>
            VerticalNarrowed,
        }

        private bool IsSpread { get { return NavigationMode == WipeNavigationMode.Spread || NavigationMode == WipeNavigationMode.VerticalSpread || NavigationMode == WipeNavigationMode.HorizontalSpread; } }
        private bool IsNarrowed { get { return NavigationMode == WipeNavigationMode.Narrowed || NavigationMode == WipeNavigationMode.VerticalNarrowed || NavigationMode == WipeNavigationMode.HorizontalNarrowed; } }
        private bool IsHorizontal { get { return NavigationMode == WipeNavigationMode.HorizontalSpread || NavigationMode == WipeNavigationMode.HorizontalNarrowed; } }
        private bool IsVertical { get { return NavigationMode == WipeNavigationMode.VerticalSpread || NavigationMode == WipeNavigationMode.VerticalNarrowed; } }
        private bool IsFocal { get { return NavigationMode == WipeNavigationMode.Spread || NavigationMode == WipeNavigationMode.Narrowed; } }

        private Canvas storyBoard;
        private Action<FrameworkElement> endAnimation;
        private FrameworkElement fromContent;
        private FrameworkElement toContent;
        private ContentControl toContentControl;
        private ContentControl fromContentControl;

        /// <summary>
        /// 再生するワイプアニメーションのタイプ
        /// </summary>
        public WipeNavigationMode NavigationMode { get; set; }
        /// <summary>
        /// アニメーションの再生時間
        /// </summary>
        public Duration Duration { get; set; } = new Duration(new TimeSpan(0, 0, 0, 0, 300));

        /// <summary>
        /// アニメーション再生の為のイニシャライザ
        /// </summary>
        /// <param name="fromContent">遷移元ページ</param>
        /// <param name="toContent">遷移先ページ</param>
        /// <param name="endAnimation">アニメーション終了時のイベントハンドラ</param>
        /// <returns>アニメーションを再生するコンテント</returns>
        public FrameworkElement Initialize(FrameworkElement fromContent, FrameworkElement toContent, Action<FrameworkElement> endAnimation)
        {
            this.toContent = toContent;
            if (toContent == null)
                return null;
            this.fromContent = fromContent;
            this.endAnimation = endAnimation;

            storyBoard = new Canvas();
            fromContentControl = new ContentControl();
            toContentControl = new ContentControl();
            storyBoard.Children.Add(fromContentControl);
            storyBoard.Children.Add(toContentControl);

            return storyBoard as FrameworkElement;
        }

        /// <summary>
        /// アニメーション再生
        /// </summary>
        public void Animate()
        {
            if (toContent == null)
                return;
            if (fromContent == null)
            {
                endAnimation?.Invoke(toContent);
                return;
            }

            double width = fromContent.ActualWidth;
            double height = fromContent.ActualHeight;
            
            Canvas.SetLeft(fromContentControl, 0);
            fromContentControl.Width = width;
            Canvas.SetTop(fromContentControl, 0);
            fromContentControl.Height = height;

            Canvas.SetLeft(toContentControl, IsFocal ? width / 2 : 0);
            toContentControl.Width = IsFocal ? 0 : width;
            Canvas.SetTop(toContentControl, IsFocal ? height / 2 : 0);
            toContentControl.Height = IsFocal ? 0 : height;

            fromContentControl.Content = IsSpread ? fromContent : toContent;
            toContentControl.Content = IsSpread ? toContent : fromContent;

            var duration = Duration;
            Storyboard story = new Storyboard { Duration = duration };

            if (IsHorizontal || IsFocal)
            {
                DoubleAnimation toTop = new DoubleAnimation
                {
                    From = IsSpread ? height / 2 : 0,
                    To = IsSpread ? 0 : height / 2,
                    Duration = duration,
                };
                DoubleAnimation toHeight = new DoubleAnimation
                {
                    From = IsSpread ? 0 : height,
                    To = IsSpread ? height : 0,
                    Duration = duration,
                };
                Storyboard.SetTarget(toTop, toContentControl);
                Storyboard.SetTargetProperty(toTop, new PropertyPath("(Canvas.Top)"));
                Storyboard.SetTarget(toHeight, toContentControl);
                Storyboard.SetTargetProperty(toHeight, new PropertyPath("Height"));

                story.Children.Add(toTop);
                story.Children.Add(toHeight);
            }
            if (IsVertical || IsFocal)
            {
                DoubleAnimation toLeft = new DoubleAnimation
                {
                    From = IsSpread ? width / 2 : 0,
                    To = IsSpread ? 0 : width / 2,
                    Duration = duration,
                };
                DoubleAnimation toWidth = new DoubleAnimation
                {
                    From = IsSpread ? 0 : width,
                    To = IsSpread ? width : 0,
                    Duration = duration,
                };
                Storyboard.SetTarget(toLeft, toContentControl);
                Storyboard.SetTargetProperty(toLeft, new PropertyPath("(Canvas.Left)"));
                Storyboard.SetTarget(toWidth, toContentControl);
                Storyboard.SetTargetProperty(toWidth, new PropertyPath("Width"));

                story.Children.Add(toWidth);
                story.Children.Add(toLeft);
            }

            story.Completed += OnStoryCompleted;
            story.Begin();
        }
        private void OnStoryCompleted(object sender, EventArgs e)
        {
            storyBoard.Children.Remove(fromContentControl);
            storyBoard.Children.Remove(toContentControl);
            toContentControl.Content = null;
            fromContentControl.Content = null;
            endAnimation?.Invoke(toContent);
        }
    }
    /// <summary>
    /// シンプルなスライド系のアニメーション
    /// </summary>
    /// <remarks><a href="http://techoh.net/wpf-control-storyboard-with-code/">2分でできるC#コードからWPFのアニメーションを操る方法</a></remarks>
    public class SimpleRaiseNavigationStory : INavigationStory
    {
        /// <summary>
        /// スライドアニメーションのタイプ定義
        /// </summary>
        public enum RaiseNavigationMode
        {
            /// <summary>
            /// 遷移先ページが右側からスライド
            /// </summary>
            RightToLeft,
            /// <summary>
            /// 遷移先ページが左側からスライド
            /// </summary>
            LeftToRight,
            /// <summary>
            /// 遷移先ページが下側からスライド
            /// </summary>
            BottomToTop,
            /// <summary>
            /// 遷移先ページが上側からスライド
            /// </summary>
            TopToBottom
        }

        private Grid storyBoard;
        private Action<FrameworkElement> endAnimation;
        private FrameworkElement fromContent;
        private FrameworkElement toContent;

        /// <summary>
        /// 再生するスライドアニメーションのタイプ
        /// </summary>
        public RaiseNavigationMode NavigationMode { get; set; } = RaiseNavigationMode.RightToLeft;
        /// <summary>
        /// アニメーションの再生時間
        /// </summary>
        public Duration Duration { get; set; } = new Duration(new TimeSpan(0, 0, 0, 0, 300));

        /// <summary>
        /// アニメーション再生の為のイニシャライザ
        /// </summary>
        /// <param name="fromContent">遷移元ページ</param>
        /// <param name="toContent">遷移先ページ</param>
        /// <param name="endAnimation">アニメーション終了時のイベントハンドラ</param>
        /// <returns>アニメーションを再生するコンテント</returns>
        public FrameworkElement Initialize(FrameworkElement fromContent, FrameworkElement toContent, Action<FrameworkElement> endAnimation)
        {
            this.toContent = toContent;
            if (toContent == null)
                return null;
            this.fromContent = fromContent;
            this.endAnimation = endAnimation;
            storyBoard = new Grid();
            if (NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.LeftToRight)
            {
                storyBoard.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 100 : 1, GridUnitType.Star) });
                storyBoard.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 1 : 100, GridUnitType.Star) });
            }
            else
            {
                storyBoard.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 100 : 1, GridUnitType.Star) });
                storyBoard.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 1 : 100, GridUnitType.Star) });
            }
            return storyBoard as FrameworkElement;
        }
        /// <summary>
        /// アニメーション再生
        /// </summary>
        public void Animate()
        {
            if (toContent == null)
                return;
            if (fromContent == null)
            {
                endAnimation?.Invoke(toContent);
                return;
            }

            if (NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.LeftToRight)
            {
                Grid.SetColumn(fromContent, NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 0 : 1);
                Grid.SetColumn(toContent, NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 1 : 0);
            }
            else
            {
                Grid.SetRow(fromContent, NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 0 : 1);
                Grid.SetRow(toContent, NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 1 : 0);
            }
            storyBoard.Children.Add(fromContent);
            storyBoard.Children.Add(toContent);

            var duration = Duration;
            Storyboard story = new Storyboard { Duration = duration };

            GridLengthAnimation fromSizeAnimation = new GridLengthAnimation
            {
                From = new GridLength(NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 100 : 1, GridUnitType.Star),
                To = new GridLength(NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 1 : 100, GridUnitType.Star),
                Duration = duration,
            };
            GridLengthAnimation toSizeAnimation = new GridLengthAnimation
            {
                From = new GridLength(NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 1 : 100, GridUnitType.Star),
                To = new GridLength(NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.BottomToTop ? 100 : 1, GridUnitType.Star),
                Duration = duration
            };
            if (NavigationMode == RaiseNavigationMode.RightToLeft || NavigationMode == RaiseNavigationMode.LeftToRight)
            {
                Storyboard.SetTarget(fromSizeAnimation, storyBoard.ColumnDefinitions[0]);
                Storyboard.SetTargetProperty(fromSizeAnimation, new PropertyPath("Width"));
                Storyboard.SetTarget(toSizeAnimation, storyBoard.ColumnDefinitions[1]);
                Storyboard.SetTargetProperty(toSizeAnimation, new PropertyPath("Width"));
            }
            else
            {
                Storyboard.SetTarget(fromSizeAnimation, storyBoard.RowDefinitions[0]);
                Storyboard.SetTargetProperty(fromSizeAnimation, new PropertyPath("Height"));
                Storyboard.SetTarget(toSizeAnimation, storyBoard.RowDefinitions[1]);
                Storyboard.SetTargetProperty(toSizeAnimation, new PropertyPath("Height"));
            }
            story.Children.Add(fromSizeAnimation);
            story.Children.Add(toSizeAnimation);

            story.Completed += OnStoryCompleted;
            story.Begin();
        }
        private void OnStoryCompleted(object sender, EventArgs e)
        {
            storyBoard.Children.Remove(fromContent);
            storyBoard.Children.Remove(toContent);
            fromContent.ClearValue(Grid.ColumnProperty);
            toContent.ClearValue(Grid.ColumnProperty);
            endAnimation?.Invoke(toContent);
        }
    }

    /// <summary>
    /// ページナビゲーションを行うクラス
    /// </summary>
    /// <remarks><a href="http://sourcechord.hatenablog.com/entry/2016/02/01/003758">WPFでシンプルな独自ナビゲーション処理のサンプルを書いてみた</a></remarks>
    public class NavigationServiceEx : DependencyObject
    {
        /// <summary>
        /// ページナビゲーションを行う領域となるContentControlを保持するAccessor
        /// </summary>
        protected ContentControl Content
        {
            get { return (ContentControl)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// ページナビゲーションを行う領域となるContentControlを保持するプロパティ
        /// </summary>
        protected static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
            typeof(ContentControl),
            typeof(NavigationServiceEx),
            new PropertyMetadata(null));

        #region ナビゲーションで利用する各種メソッド
        /// <summary>
        /// view引数で指定されたインスタンスのページへとナビゲーションを行います。
        /// </summary>
        /// <param name="view">遷移先ページのインスタンス</param>
        /// <exception cref="Exception">ナビゲーションアニメイニシャライザ内部エラー</exception>
        /// <exception cref="Exception">ナビゲーションアニメ内部エラー</exception>
        public void Navigate(FrameworkElement view)
        {
            var story = GetNavigationStory(this.Content);
            if (story == null)
            {
                this.Content.Content = view;
            }
            else
            {
                try
                {
                    this.Content.Content = story.Initialize(this.Content.Content as FrameworkElement, view, x => this.Content.Content = x);
                }
                catch (Exception e)
                {
                    throw new Exception("ナビゲーションアニメイニシャライザ内部エラー", e);
                }
                try
                {
                    story.Animate();
                }
                catch (Exception e)
                {
                    throw new Exception("ナビゲーションアニメ内部エラー", e);
                }
            }
        }

        /// <summary>
        /// viewType引数で指定された型のインスタンスを生成し、そのインスタンスのページへとナビゲーションを行います。
        /// </summary>
        /// <param name="viewType">遷移先ページのType</param>
        /// <exception cref="Exception">ナビゲーションアニメイニシャライザ内部エラー</exception>
        /// <exception cref="Exception">ナビゲーションアニメ内部エラー</exception>
        public void Navigate(Type viewType)
        {
            if (viewType == null)
            {
                this.Navigate((FrameworkElement)null);
            }
            var view = Activator.CreateInstance(viewType) as FrameworkElement;
            this.Navigate(view);
        }
        #endregion

        /// <summary>
        /// NavigationCommands.GoToPageコマンドに対する応答処理
        /// </summary>
        /// <param name="sender">未使用</param>
        /// <param name="e">e.Parameterに遷移するページのType</param>
        /// <exception cref="Exception">ナビゲーションアニメイニシャライザ内部エラー</exception>
        /// <exception cref="Exception">ナビゲーションアニメ内部エラー</exception>
        private void OnGoToPage(object sender, ExecutedRoutedEventArgs e)
        {
            var nextPage = e.Parameter as Type;
            this.Navigate(nextPage);
        }

        #region ページ遷移時のアニメーション
        /// <summary>
        /// ページ遷移アニメーションのゲッター
        /// </summary>
        /// <param name="obj">添付されたDependencyObject</param>
        /// <returns>ページ遷移時のアニメーションを実行するインターフェース</returns>
        public static INavigationStory GetNavigationStory(DependencyObject obj)
        {
            return (INavigationStory)obj.GetValue(NavigationStoryProperty);
        }
        /// <summary>
        /// ページ遷移アニメーションのセッター
        /// </summary>
        /// <param name="obj">添付されるDependencyObject</param>
        /// <param name="value">ページ遷移時のアニメーションを実行するインターフェース</param>
        public static void SetNavigationStory(DependencyObject obj, INavigationStory value)
        {
            obj.SetValue(NavigationStoryProperty, value);
        }
        /// <summary>
        /// ページ遷移時のアニメーションを実行するインターフェースの添付プロパティ
        /// </summary>
        public static readonly DependencyProperty NavigationStoryProperty = DependencyProperty.RegisterAttached(
            "NavigationStory",
            typeof(INavigationStory),
            typeof(NavigationServiceEx),
            new PropertyMetadata(null));
        #endregion

        #region ページナビゲーションを行う領域となるContentControlを指定するための添付プロパティ
        /// <summary>
        /// ページ遷移を実現するエリアのゲッター
        /// </summary>
        /// <param name="obj">添付されたDependencyObject</param>
        /// <returns>添付されたContentControl</returns>
        public static ContentControl GetTarget(DependencyObject obj)
        {
            return (ContentControl)obj.GetValue(TargetProperty);
        }
        /// <summary>
        /// ページ遷移を実現するエリアのセッター
        /// </summary>
        /// <param name="obj">添付されるDependencyObject</param>
        /// <param name="value">設定するContentControl</param>
        public static void SetTarget(DependencyObject obj, ContentControl value)
        {
            obj.SetValue(TargetProperty, value);
        }
        /// <summary>
        /// ページ遷移を実現するエリア。ContentControlを指定すること。
        /// </summary>
        /// <remarks>この添付プロパティで指定した値は、NavigationServiceEx.Contentプロパティとバインドして同期するようにして扱う。</remarks>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.RegisterAttached(
            "Target",
            typeof(ContentControl),
            typeof(NavigationServiceEx),
            new PropertyMetadata(null, OnTargetChanged));

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = e.NewValue as ContentControl;

            if (d is FrameworkElement element && target != null)
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
        /// <summary>
        /// 初期ページを指定するプロパティのゲッター
        /// </summary>
        /// <param name="obj">添付されたDependencyObject</param>
        /// <returns>添付された初期ページのType</returns>
        public static Type GetStartup(DependencyObject obj)
        {
            return (Type)obj.GetValue(StartupProperty);
        }
        /// <summary>
        /// 初期ページを指定するプロパティのセッター
        /// </summary>
        /// <param name="obj">添付されるDependencyObject</param>
        /// <param name="value">添付する初期ページのType</param>
        public static void SetStartup(DependencyObject obj, Type value)
        {
            obj.SetValue(StartupProperty, value);
        }
        /// <summary>
        /// 初期ページを指定するプロパティ
        /// </summary>
        public static readonly DependencyProperty StartupProperty = DependencyProperty.RegisterAttached(
            "Startup",
            typeof(Type),
            typeof(NavigationServiceEx),
            new PropertyMetadata(null, OnStartupChanged));

        private static void OnStartupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var startupType = e.NewValue as Type;

            if (d is FrameworkElement element && startupType != null)
            {
                var nav = NavigationServiceEx.GetNavigator(element);
                nav?.Navigate(startupType);
            }
        }
        #endregion

        #region 任意のコントロールに対して、NavigationServiceExをアタッチできるようにするための添付プロパティ
        /// <summary>
        /// アタッチしたNavigationServiceEx添付プロパティのゲッター
        /// </summary>
        /// <param name="obj">添付されたDependencyObject</param>
        /// <returns>添付されたNavigationServiceEx</returns>
        public static NavigationServiceEx GetNavigator(DependencyObject obj)
        {
            return (NavigationServiceEx)obj.GetValue(NavigatorProperty);
        }
        /// <summary>
        /// アタッチしたNavigationServiceEx添付プロパティのセッター
        /// </summary>
        /// <param name="obj">添付されるDependencyObject</param>
        /// <param name="value">添付するNavigationServiceEx</param>
        protected static void SetNavigator(DependencyObject obj, NavigationServiceEx value)
        {
            obj.SetValue(NavigatorProperty, value);
        }
        /// <summary>
        /// 任意のコントロールに対して、NavigationServiceExをアタッチできるようにするための添付プロパティ
        /// </summary>
        protected static readonly DependencyProperty NavigatorProperty = DependencyProperty.RegisterAttached(
            "Navigator",
            typeof(NavigationServiceEx),
            typeof(NavigationServiceEx),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        #endregion
    }

    /// <summary>
    /// ページナビゲートのための拡張メソッド定義クラス
    /// </summary>
    public static class NavigationServiceExtensions
    {
        /// <summary>
        /// view引数で指定されたインスタンスのページへとナビゲーションを行います。
        /// </summary>
        /// <param name="element">ターゲットを添付したFrameworkElement</param>
        /// <param name="view">遷移先ページ</param>
        /// <exception cref="Exception">ナビゲーションアニメイニシャライザ内部エラー</exception>
        /// <exception cref="Exception">ナビゲーションアニメ内部エラー</exception>
        public static　void Navigate(this FrameworkElement element, FrameworkElement view)
        {
            var navigator = NavigationServiceEx.GetNavigator(element);
            navigator.Navigate(view);
        }

        /// <summary>
        /// viewType引数で指定された型のインスタンスを生成し、そのインスタンスのページへとナビゲーションを行います。
        /// </summary>
        /// <param name="element">ターゲットを添付したFrameworkElement</param>
        /// <param name="viewType">遷移先ページのType</param>
        /// <exception cref="Exception">ナビゲーションアニメイニシャライザ内部エラー</exception>
        /// <exception cref="Exception">ナビゲーションアニメ内部エラー</exception>
        public static void Navigate(this FrameworkElement element, Type viewType)
        {
            var navigator = NavigationServiceEx.GetNavigator(element);
            navigator.Navigate(viewType);
        }
    }
}
