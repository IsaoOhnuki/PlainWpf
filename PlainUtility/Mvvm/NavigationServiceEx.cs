using Behaviors;
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
using System.ComponentModel;

namespace Mvvm
{
    /// <summary>
    /// 
    /// </summary>
    public interface INotifyNavigationStory
    {
        /// <summary>
        /// アニメーション再生中であることの通知
        /// </summary>
        /// <value>
        /// true アニメーション再生中
        /// </value>
        bool IsAnimation { get; set; }
    }
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
        private enum WipeNavigationModeCheck
        {
            Direction = 3,
            Spread = 1,
            Narrowed = 2,
            Horizontal = 4,
            Vertical = 8,
            Center = 16,
            Left = 0,
            Right = 32,
            Top = 64,
            Bottom = 64 + 32,
            Posision = 16 + 32 + 64,
        }
        /// <summary>
        /// ワイプアニメーションのタイプ定義
        /// </summary>
        public enum WipeNavigationMode
        {
            /// <summary>
            /// 遷移先が中心から広がる
            /// </summary>
            CenterSpread = WipeNavigationModeCheck.Spread | WipeNavigationModeCheck.Center,
            /// <summary>
            /// 遷移先が水平方向に広がる
            /// </summary>
            HorizontalSpread = WipeNavigationModeCheck.Spread | WipeNavigationModeCheck.Horizontal | WipeNavigationModeCheck.Center,
            /// <summary>
            /// 遷移先が垂直方向に広がる
            /// </summary>
            VerticalSpread = WipeNavigationModeCheck.Spread | WipeNavigationModeCheck.Vertical | WipeNavigationModeCheck.Center,
            /// <summary>
            /// 遷移元が中心に窄まる
            /// </summary>
            CenterNarrowed = WipeNavigationModeCheck.Narrowed | WipeNavigationModeCheck.Center,
            /// <summary>
            /// 遷移元が左右から中心に窄まる
            /// </summary>
            HorizontalNarrowed = WipeNavigationModeCheck.Narrowed | WipeNavigationModeCheck.Horizontal | WipeNavigationModeCheck.Center,
            /// <summary>
            /// 遷移元が上下から中心に窄まる
            /// </summary>
            VerticalNarrowed = WipeNavigationModeCheck.Narrowed | WipeNavigationModeCheck.Vertical | WipeNavigationModeCheck.Center,
            /// <summary>
            /// 遷移先が左から右に水平方向に広がる
            /// </summary>
            LeftToHorizontal = WipeNavigationModeCheck.Horizontal | WipeNavigationModeCheck.Left,
            /// <summary>
            /// 遷移先が右から左に水平方向に広がる
            /// </summary>
            RightToHorizontal = WipeNavigationModeCheck.Horizontal | WipeNavigationModeCheck.Right,
            /// <summary>
            /// 遷移先が上から下に垂直方向に広がる
            /// </summary>
            TopToVertical = WipeNavigationModeCheck.Vertical | WipeNavigationModeCheck.Top,
            /// <summary>
            /// 遷移先が下から上に垂直方向に広がる
            /// </summary>
            BottomToVertical = WipeNavigationModeCheck.Vertical | WipeNavigationModeCheck.Bottom,
        }

        private bool IsSpread { get { return ((WipeNavigationModeCheck)NavigationMode & WipeNavigationModeCheck.Direction) == WipeNavigationModeCheck.Spread; } }
        private bool IsNarrowed { get { return ((WipeNavigationModeCheck)NavigationMode & WipeNavigationModeCheck.Direction) == WipeNavigationModeCheck.Narrowed; } }
        private bool IsHorizontal { get { return ((WipeNavigationModeCheck)NavigationMode & WipeNavigationModeCheck.Horizontal) == WipeNavigationModeCheck.Horizontal; } }
        private bool IsVertical { get { return ((WipeNavigationModeCheck)NavigationMode & WipeNavigationModeCheck.Vertical) == WipeNavigationModeCheck.Vertical; } }
        private bool IsCenter { get { return ((WipeNavigationModeCheck)NavigationMode & WipeNavigationModeCheck.Posision) == WipeNavigationModeCheck.Center; } }
        private bool IsLeft { get { return ((WipeNavigationModeCheck)NavigationMode & WipeNavigationModeCheck.Posision) == WipeNavigationModeCheck.Left; } }
        private bool IsRight { get { return ((WipeNavigationModeCheck)NavigationMode & WipeNavigationModeCheck.Posision) == WipeNavigationModeCheck.Right; } }
        private bool IsTop { get { return ((WipeNavigationModeCheck)NavigationMode & WipeNavigationModeCheck.Posision) == WipeNavigationModeCheck.Top; } }
        private bool IsBottom { get { return ((WipeNavigationModeCheck)NavigationMode & WipeNavigationModeCheck.Posision) == WipeNavigationModeCheck.Bottom; } }

        private Canvas storyBoard;
        private Action<FrameworkElement> endAnimation;
        private FrameworkElement fromContent;
        private FrameworkElement toContent;
        private ContentControl fromContentControl;
        private ContentControl toContentControl;
        private Viewbox fromViewbox;
        private Viewbox toViewbox;

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
            fromViewbox = new Viewbox { Stretch = Stretch.Fill, Child = fromContentControl };
            toViewbox = new Viewbox { Stretch = Stretch.Fill, Child = toContentControl };
            // 裏画面
            storyBoard.Children.Add(fromViewbox);
            // 表画面
            storyBoard.Children.Add(toViewbox);

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

            Logger.Write(LogType.Debug, "WipeNavigationMode" + NavigationMode.ToString());

            // 現在のサイズ
            double width = fromContent.ActualWidth;
            double height = fromContent.ActualHeight;

            // コンテンツサイズViewboxが基準に使う
            fromContentControl.Width = width;
            fromContentControl.Height = height;
            toContentControl.Width = width;
            toContentControl.Height = height;

            // 遷移元のサイズ
            Canvas.SetLeft(fromViewbox, 0);
            fromViewbox.Width = width;
            Canvas.SetTop(fromViewbox, 0);
            fromViewbox.Height = height;

            // 遷移先のサイズ
            if (IsCenter)
            {
                Canvas.SetLeft(toViewbox, IsNarrowed || IsVertical ? 0 : width / 2);
                toViewbox.Width = IsNarrowed || IsVertical ? width : 0;
                Canvas.SetTop(toViewbox, IsNarrowed || IsHorizontal ? 0 : height / 2);
                toViewbox.Height = IsNarrowed || IsHorizontal ? height : 0;
            }
            else
            {
                Canvas.SetLeft(toViewbox, IsLeft || IsTop || IsBottom ? 0 : width);
                toViewbox.Width = IsTop || IsBottom ? width : 0;
                Canvas.SetTop(toViewbox, IsTop || IsLeft || IsRight ? 0 : height);
                toViewbox.Height = IsLeft || IsRight ? height : 0;
            }

            // Spread系なら裏が元画面、Narrowed系なら表が元画面
            fromContentControl.Content = IsSpread || !IsNarrowed ? fromContent : toContent;
            toContentControl.Content = IsSpread || !IsNarrowed ? toContent : fromContent;

            var duration = Duration;
            Storyboard story = new Storyboard { Duration = duration };

            if (IsLeft || IsRight || IsTop || IsBottom)
            {
                DoubleAnimation fromLeft = new DoubleAnimation
                {
                    From = Canvas.GetLeft(fromViewbox),
                    To = IsLeft ? width : 0,
                    Duration = duration,
                };
                if (fromLeft.From != fromLeft.To)
                {
                    Storyboard.SetTarget(fromLeft, fromViewbox);
                    Storyboard.SetTargetProperty(fromLeft, new PropertyPath("(Canvas.Left)"));
                    story.Children.Add(fromLeft);
                }
                DoubleAnimation fromWidth = new DoubleAnimation
                {
                    From = fromViewbox.Width,
                    To = IsLeft || IsRight ? 0 : width,
                    Duration = duration,
                };
                if (fromWidth.From != fromWidth.To)
                {
                    Storyboard.SetTarget(fromWidth, fromViewbox);
                    Storyboard.SetTargetProperty(fromWidth, new PropertyPath("Width"));
                    story.Children.Add(fromWidth);
                }
                DoubleAnimation fromTop = new DoubleAnimation
                {
                    From = Canvas.GetTop(fromViewbox),
                    To = IsTop ? height : 0,
                    Duration = duration,
                };
                if (fromTop.From != fromTop.To)
                {
                    Storyboard.SetTarget(fromTop, fromViewbox);
                    Storyboard.SetTargetProperty(fromTop, new PropertyPath("(Canvas.Top)"));
                    story.Children.Add(fromTop);
                }
                DoubleAnimation fromHeight = new DoubleAnimation
                {
                    From = fromViewbox.Height,
                    To = IsTop || IsBottom ? 0 : height,
                    Duration = duration,
                };
                if (fromHeight.From != fromHeight.To)
                {
                    Storyboard.SetTarget(fromHeight, fromViewbox);
                    Storyboard.SetTargetProperty(fromHeight, new PropertyPath("Height"));
                    story.Children.Add(fromHeight);
                }
            }

            DoubleAnimation toLeft = new DoubleAnimation
            {
                From = Canvas.GetLeft(toViewbox),
                To = IsSpread || IsVertical || IsLeft || IsRight ? 0 : IsCenter ? width / 2 : width,
                Duration = duration,
            };
            if (toLeft.From != toLeft.To)
            {
                Storyboard.SetTarget(toLeft, toViewbox);
                Storyboard.SetTargetProperty(toLeft, new PropertyPath("(Canvas.Left)"));
                story.Children.Add(toLeft);
            }
            DoubleAnimation toWidth = new DoubleAnimation
            {
                From = toViewbox.Width,
                To = IsSpread || IsVertical || IsLeft || IsRight ? width : 0,
                Duration = duration,
            };
            if (toWidth.From != toWidth.To)
            {
                Storyboard.SetTarget(toWidth, toViewbox);
                Storyboard.SetTargetProperty(toWidth, new PropertyPath("Width"));
                story.Children.Add(toWidth);
            }
            DoubleAnimation toTop = new DoubleAnimation
            {
                From = Canvas.GetTop(toViewbox),
                To = IsSpread || IsHorizontal || IsTop || IsBottom ? 0 : IsCenter ? height / 2 : height,
                Duration = duration,
            };
            if (toTop.From != toTop.To)
            {
                Storyboard.SetTarget(toTop, toViewbox);
                Storyboard.SetTargetProperty(toTop, new PropertyPath("(Canvas.Top)"));
                story.Children.Add(toTop);
            }
            DoubleAnimation toHeight = new DoubleAnimation
            {
                From = toViewbox.Height,
                To = IsSpread || IsHorizontal || IsTop || IsBottom ? height : 0,
                Duration = duration,
            };
            if (toHeight.From != toHeight.To)
            {
                Storyboard.SetTarget(toHeight, toViewbox);
                Storyboard.SetTargetProperty(toHeight, new PropertyPath("Height"));
                story.Children.Add(toHeight);
            }

            if (fromContent is INotifyNavigationStory)
            {
                ((INotifyNavigationStory)fromContent).IsAnimation = true;
            }
            if (toContent is INotifyNavigationStory)
            {
                ((INotifyNavigationStory)toContent).IsAnimation = true;
            }

            story.Completed += OnStoryCompleted;
            story.Begin();
        }
        private void OnStoryCompleted(object sender, EventArgs e)
        {
            storyBoard.Children.Remove(fromViewbox);
            storyBoard.Children.Remove(toViewbox);
            fromViewbox.Child = null;
            toViewbox.Child = null;
            toContentControl.Content = null;
            fromContentControl.Content = null;
            endAnimation?.Invoke(toContent);
            if (fromContent is INotifyNavigationStory)
            {
                ((INotifyNavigationStory)fromContent).IsAnimation = false;
            }
            if (toContent is INotifyNavigationStory)
            {
                ((INotifyNavigationStory)toContent).IsAnimation = false;
            }
            fromContent = null;
            toContent = null;
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
            if (CanNavigation)
            {
                if (story == null)
                {
                    this.Content.Content = view;
                }
                else
                {
                    CanNavigation = false;
                    FrameworkElement animationElement;
                    try
                    {
                        animationElement = story.Initialize(this.Content.Content as FrameworkElement, view,
                            x =>
                            {
                                this.Content.Content = x;
                                CanNavigation = true;
                            });
                    }
                    catch (Exception e)
                    {
                        animationElement = view;
                        CanNavigation = true;
                        Logger.Write(e, "ナビゲーションアニメイニシャライザ内部エラー");
                    }
                    this.Content.Content = animationElement;
                    if (!animationElement.Equals(view))
                    {
                        try
                        {
                            story.Animate();
                        }
                        catch (Exception e)
                        {
                            this.Content.Content = view;
                            CanNavigation = true;
                            Logger.Write(e, "ナビゲーションアニメ内部エラー");
                        }
                    }
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
                throw new Exception();
            }
            else
            {
                var view = Activator.CreateInstance(viewType) as FrameworkElement;
                this.Navigate(view);
            }
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

        /// <summary>
        /// ナビゲーション可能であることのフラグ
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can navigation; otherwise, <c>false</c>.
        /// </value>
        private static bool CanNavigation { get; set; } = true;

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
