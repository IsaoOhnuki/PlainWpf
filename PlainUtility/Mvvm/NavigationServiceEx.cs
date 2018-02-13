﻿using Behaviours;
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
    public interface INavigationContents
    {
        /// <summary>
        /// 
        /// </summary>
        FrameworkElement FromContent { get; }
        /// <summary>
        /// 
        /// </summary>
        FrameworkElement ToContent { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface INavigationStory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        FrameworkElement Initialize(INavigationContents contents, Action<FrameworkElement> endAnimation);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endAnimation"></param>
        void Animate();
    }

    public class SimplNavigationContents : INavigationContents
    {
        public SimplNavigationContents(FrameworkElement fromContent, FrameworkElement toContent)
        {
            this.FromContent = fromContent;
            this.ToContent = toContent;
        }
        public FrameworkElement FromContent { get; private set; }
        public FrameworkElement ToContent { get; private set; }
    }

    public class SimpleScrollNavigationStory : INavigationStory
    {
        public enum ScrollNavigationMode
        {
            RightToLeft,
            LeftToRight,
            BottomToTop,
            TopToBottom
        }

        // http://techoh.net/wpf-control-storyboard-with-code/ 2分でできるC#コードからWPFのアニメーションを操る方法
        private Grid storyBoard{ get; set; }
        private Action<FrameworkElement> endAnimation;
        private INavigationContents contents;
        public ScrollNavigationMode navigationMode { get; private set; }

        public SimpleScrollNavigationStory(ScrollNavigationMode navigationMode = ScrollNavigationMode.RightToLeft)
        {
            this.navigationMode = navigationMode;
        }

        public FrameworkElement Initialize(INavigationContents contents, Action<FrameworkElement> endAnimation)
        {
            this.contents = contents;
            this.endAnimation = endAnimation;
            storyBoard = new Grid();
            if (navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.LeftToRight)
            {
                storyBoard.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 100 : 1, GridUnitType.Star) });
                storyBoard.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 1 : 100, GridUnitType.Star) });
            }
            else
            {
                storyBoard.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 100 : 1, GridUnitType.Star) });
                storyBoard.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 1 : 100, GridUnitType.Star) });
            }
            return storyBoard as FrameworkElement;
        }

        public void Animate()
        {
            if (contents.ToContent == null)
                return;
            if (contents.FromContent == null)
            {
                endAnimation?.Invoke(contents.ToContent);
                return;
            }

            if (navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.LeftToRight)
            {
                Grid.SetColumn(contents.FromContent, navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 0 : 1);
                Grid.SetColumn(contents.ToContent, navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 1 : 0);
            }
            else
            {
                Grid.SetRow(contents.FromContent, navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 0 : 1);
                Grid.SetRow(contents.ToContent, navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 1 : 0);
            }
            storyBoard.Children.Add(contents.FromContent);
            storyBoard.Children.Add(contents.ToContent);

            var duration = new Duration(TimeSpan.FromMilliseconds(300));
            Storyboard story = new Storyboard { Duration = duration };

            GridLengthAnimation colLeft = new GridLengthAnimation
            {
                From = new GridLength(navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 100 : 1, GridUnitType.Star),
                To = new GridLength(navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 1 : 100, GridUnitType.Star),
                Duration = duration,
            };
            if (navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.LeftToRight)
            {
                Storyboard.SetTarget(colLeft, storyBoard.ColumnDefinitions[0]);
                Storyboard.SetTargetProperty(colLeft, new PropertyPath("Width"));
            }
            else
            {
                Storyboard.SetTarget(colLeft, storyBoard.RowDefinitions[0]);
                Storyboard.SetTargetProperty(colLeft, new PropertyPath("Height"));
            }
            story.Children.Add(colLeft);

            GridLengthAnimation colRight = new GridLengthAnimation
            {
                From = new GridLength(navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 1 : 100, GridUnitType.Star),
                To = new GridLength(navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.BottomToTop ? 100 : 1, GridUnitType.Star),
                Duration = duration
            };
            if (navigationMode == ScrollNavigationMode.RightToLeft || navigationMode == ScrollNavigationMode.LeftToRight)
            {
                Storyboard.SetTarget(colRight, storyBoard.ColumnDefinitions[1]);
                Storyboard.SetTargetProperty(colRight, new PropertyPath("Width"));
            }
            else
            {
                Storyboard.SetTarget(colRight, storyBoard.RowDefinitions[1]);
                Storyboard.SetTargetProperty(colRight, new PropertyPath("Height"));
            }
            story.Children.Add(colRight);

            story.Completed += new EventHandler((object sender, EventArgs e) => {
                storyBoard.Children.Remove(contents.FromContent);
                storyBoard.Children.Remove(contents.ToContent);
                contents.FromContent?.ClearValue(Grid.ColumnProperty);
                contents.ToContent?.ClearValue(Grid.ColumnProperty);
                endAnimation?.Invoke(contents.ToContent);
            });
            story.Begin();
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
        private void EndAnimation(FrameworkElement contnts)
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
                var story = new SimpleScrollNavigationStory();
                var contents = new SimplNavigationContents(this.Content.Content as FrameworkElement, view);
                this.Content.Content = story.Initialize(contents, EndAnimation);
                story.Animate();
                return true;
            }
            catch (Exception e)
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