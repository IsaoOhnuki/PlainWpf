using Behaviours;
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

namespace Mvvm
{
    /// <summary>
    /// 
    /// </summary>
    public interface INavigationStory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        FrameworkElement Initialize(FrameworkElement fromContent, FrameworkElement toContent, Action<FrameworkElement> endAnimation);
        /// <summary>
        /// 
        /// </summary>
        void Animate();
    }
    /// <summary>
    /// 
    /// </summary>
    public class SimpleWipeNavigationStory : INavigationStory
    {
        /// <summary>
        /// 
        /// </summary>
        public enum WipeNavigationMode
        {
            /// <summary>
            /// 
            /// </summary>
            Spread,
            /// <summary>
            /// 
            /// </summary>
            HorizontalSpread,
            /// <summary>
            /// 
            /// </summary>
            VerticalSpread,
            /// <summary>
            /// 
            /// </summary>
            Narrowed,
            /// <summary>
            /// 
            /// </summary>
            HorizontalNarrowed,
            /// <summary>
            /// 
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
        /// 
        /// </summary>
        public WipeNavigationMode NavigationMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromContent"></param>
        /// <param name="toContent"></param>
        /// <param name="endAnimation"></param>
        /// <returns></returns>
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
        /// 
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

            var duration = new Duration(TimeSpan.FromMilliseconds(300));
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
    /// 
    /// </summary>
    /// <uri name="http://techoh.net/wpf-control-storyboard-with-code/">2分でできるC#コードからWPFのアニメーションを操る方法</uri>
    public class SimpleRaiseNavigationStory : INavigationStory
    {
        /// <summary>
        /// 
        /// </summary>
        public enum RaiseNavigationMode
        {
            /// <summary>
            /// 
            /// </summary>
            RightToLeft,
            /// <summary>
            /// 
            /// </summary>
            LeftToRight,
            /// <summary>
            /// 
            /// </summary>
            BottomToTop,
            /// <summary>
            /// 
            /// </summary>
            TopToBottom
        }

        private Grid storyBoard;
        private Action<FrameworkElement> endAnimation;
        private FrameworkElement fromContent;
        private FrameworkElement toContent;

        /// <summary>
        /// 
        /// </summary>
        public RaiseNavigationMode NavigationMode { get; set; } = RaiseNavigationMode.RightToLeft;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromContent"></param>
        /// <param name="toContent"></param>
        /// <param name="endAnimation"></param>
        /// <returns></returns>
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
        /// 
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

            var duration = new Duration(TimeSpan.FromMilliseconds(300));
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
        private void OnEndAnimation(FrameworkElement contnts)
        {
            this.Content.Content = contnts;
        }
        /// <summary>
        /// view引数で指定されたインスタンスのページへとナビゲーションを行います。
        /// </summary>
        /// <param name="view"></param>
        /// <returns>成功True、失敗False</returns>
        public bool Navigate(FrameworkElement view)
        {
            try
            {
                var story = GetNavigationStory(this.Content);
                if (story == null)
                {
                    this.Content.Content = view;
                }
                else
                {
                    this.Content.Content = story.Initialize(this.Content.Content as FrameworkElement, view, OnEndAnimation);
                    story.Animate();
                }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static INavigationStory GetNavigationStory(DependencyObject obj)
        {
            return (INavigationStory)obj.GetValue(NavigationStoryProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetNavigationStory(DependencyObject obj, INavigationStory value)
        {
            obj.SetValue(NavigationStoryProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NavigationStoryProperty = DependencyProperty.RegisterAttached(
            "NavigationStory",
            typeof(INavigationStory),
            typeof(NavigationServiceEx),
            new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => {
            }));

        #region ページナビゲーションを行う領域となるContentControlを指定するための添付プロパティ
        /// <summary>
        /// この添付プロパティで指定した値は、NavigationServiceEx.Contentプロパティとバインドして同期するようにして扱う。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ContentControl GetTarget(DependencyObject obj)
        {
            return (ContentControl)obj.GetValue(TargetProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTarget(DependencyObject obj, ContentControl value)
        {
            obj.SetValue(TargetProperty, value);
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        /// </summary>
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
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetStartup(DependencyObject obj)
        {
            return (Type)obj.GetValue(StartupProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetStartup(DependencyObject obj, Type value)
        {
            obj.SetValue(StartupProperty, value);
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Startup.  This enables animation, styling, binding, etc...
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
            var storyboard = e.NewValue as Type;

            if (d is FrameworkElement element)
            {

            }
        }
        #endregion

        #region 任意のコントロールに対して、NavigationServiceExをアタッチできるようにするための添付プロパティ
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static NavigationServiceEx GetNavigator(DependencyObject obj)
        {
            return (NavigationServiceEx)obj.GetValue(NavigatorProperty);
        }
        /// <summary>
        /// ↓protectedにして外部からは利用できないように。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetNavigator(DependencyObject obj, NavigationServiceEx value)
        {
            obj.SetValue(NavigatorProperty, value);
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for Navigator.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NavigatorProperty = DependencyProperty.RegisterAttached(
            "Navigator",
            typeof(NavigationServiceEx),
            typeof(NavigationServiceEx),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
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
