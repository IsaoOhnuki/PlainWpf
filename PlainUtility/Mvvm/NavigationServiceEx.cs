﻿using Behaviors;
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
        FrameworkElement Initialize(FrameworkElement fromContent, FrameworkElement toContent);
        /// <summary>
        /// アニメーション再生
        /// </summary>
        void Animate(Action<FrameworkElement> endAnimation);
    }

    /// <summary>
    /// ページナビゲーションを行うクラス
    /// </summary>
    /// <remarks><a href="http://sourcechord.hatenablog.com/entry/2016/02/01/003758">WPFでシンプルな独自ナビゲーション処理のサンプルを書いてみた</a></remarks>
    public class NavigationServiceEx : DependencyObject
    {
        private static Dictionary<FrameworkElement, HistoryStack<FrameworkElement>> pageStacks = new Dictionary<FrameworkElement, HistoryStack<FrameworkElement>>();
        protected HistoryStack<FrameworkElement> PageStack
        {
            get { return pageStacks[Content]; }
            set
            {
                if (!pageStacks.ContainsKey(Content))
                    pageStacks[Content] = value;
            }
        }

        public static bool GetHistoryTracingAnimation(DependencyObject obj)
        {
            return (bool)obj.GetValue(HistoryTracingAnimationProperty);
        }
        public static void SetHistoryTracingAnimation(DependencyObject obj, bool value)
        {
            obj.SetValue(HistoryTracingAnimationProperty, value);
        }
        /// <summary>
        /// The history tracing animation property
        /// </summary>
        public static readonly DependencyProperty HistoryTracingAnimationProperty = DependencyProperty.RegisterAttached(
            "HistoryTracingAnimation",
            typeof(bool),
            typeof(NavigationServiceEx),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// Gets or sets a value indicating whether this instance can navigation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can navigation; otherwise, <c>false</c>.
        /// </value>
        public bool CanNavigation
        {
            get { return (bool)(Content.GetValue(CanNavigationProperty)); }
            set { Content.SetValue(CanNavigationProperty, value); }
        }
        /// <summary>
        /// The can navigation property
        /// </summary>
        public static readonly DependencyProperty CanNavigationProperty = DependencyProperty.RegisterAttached(
            nameof(CanNavigation),
            typeof(bool),
            typeof(NavigationServiceEx),
            new PropertyMetadata(defaultValue: true));

        /// <summary>
        /// Gets or sets a value indicating whether this instance can undo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can undo; otherwise, <c>false</c>.
        /// </value>
        public bool CanUndo
        {
            get { return (bool)(Content.GetValue(CanUndoProperty)); }
            set { Content.SetValue(CanUndoProperty, value); }
        }
        /// <summary>
        /// The can undo property
        /// </summary>
        public static readonly DependencyProperty CanUndoProperty = DependencyProperty.RegisterAttached(
            nameof(CanUndo),
            typeof(bool),
            typeof(NavigationServiceEx),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// Gets or sets a value indicating whether this instance can redo.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can redo; otherwise, <c>false</c>.
        /// </value>
        public bool CanRedo
        {
            get { return (bool)(Content.GetValue(CanRedoProperty)); }
            set { Content.SetValue(CanRedoProperty, value); }
        }
        /// <summary>
        /// The can redo property
        /// </summary>
        public static readonly DependencyProperty CanRedoProperty = DependencyProperty.RegisterAttached(
            nameof(CanRedo),
            typeof(bool),
            typeof(NavigationServiceEx),
            new PropertyMetadata(default(bool)));

        //protected HistoryStack<FrameworkElement> PageStack
        //{
        //    get { return (HistoryStack<FrameworkElement>)Content.GetValue(PageStackProperty); }
        //    set { Content.SetValue(PageStackProperty , value); }
        //}
        //protected static readonly DependencyProperty PageStackProperty = DependencyProperty.RegisterAttached(
        //    nameof(PageStack),
        //    typeof(HistoryStack<FrameworkElement>),
        //    typeof(NavigationServiceEx),
        //    new PropertyMetadata(null));

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
        public void Navigate(FrameworkElement view)
        {
            var story = GetNavigationStory(this.Content);
            if (CanNavigation)
            {
                CanNavigation = false;
                if (story == null || (!GetHistoryTracingAnimation(this.Content) && historyTrace))
                {
                    this.Content.Content = view;
                    if (!historyTrace)
                    {
                        PageStack.Push(view);
                        CanUndo = PageStack.CanUndo;
                        CanRedo = PageStack.CanRedo;
                    }
                    historyTrace = false;
                    CanNavigation = true;
                }
                else
                {
                    FrameworkElement animationElement;
                    try
                    {
                        animationElement = story.Initialize(this.Content.Content as FrameworkElement, view);
                    }
                    catch (Exception e)
                    {
                        animationElement = view;
                        if (!historyTrace)
                        {
                            PageStack.Push(view);
                            CanUndo = PageStack.CanUndo;
                            CanRedo = PageStack.CanRedo;
                        }
                        historyTrace = false;
                        CanNavigation = true;
                        Logger.Write(e, "ナビゲーションアニメイニシャライザ内部エラー");
                    }
                    this.Content.Content = animationElement;
                    if (animationElement == null || animationElement.Equals(view))
                    {
                        historyTrace = false;
                        CanNavigation = true;
                    }
                    else
                    {
                        try
                        {
                            story.Animate(x => {
                                this.Content.Content = x;
                                if (!historyTrace)
                                {
                                    PageStack.Push(x);
                                    CanUndo = PageStack.CanUndo;
                                    CanRedo = PageStack.CanRedo;
                                }
                                historyTrace = false;
                                CanNavigation = true;
                            });
                        }
                        catch (Exception e)
                        {
                            this.Content.Content = view;
                            if (!historyTrace)
                            {
                                PageStack.Push(view);
                                CanUndo = PageStack.CanUndo;
                                CanRedo = PageStack.CanRedo;
                            }
                            historyTrace = false;
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
        public void Navigate(Type viewType)
        {
            if (viewType == null)
            {
                this.Navigate((FrameworkElement)null);
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
        private void OnGoToPage(object sender, ExecutedRoutedEventArgs e)
        {
            var nextPage = e.Parameter as Type;
            this.Navigate(nextPage);
        }

        private bool historyTrace;
        /// <summary>
        /// NavigationCommands.PreviousPageコマンドに対する応答処理
        /// </summary>
        /// <param name="sender">未使用</param>
        /// <param name="e">e.Parameterに遷移するページのType</param>
        private void OnPreviousPage(object sender, ExecutedRoutedEventArgs e)
        {
            if (PageStack.CanUndo)
            {
                historyTrace = true;
                this.Navigate(PageStack.Undo());
                CanUndo = PageStack.CanUndo;
                CanRedo = PageStack.CanRedo;
            }
        }

        /// <summary>
        /// NavigationCommands.NextPageコマンドに対する応答処理
        /// </summary>
        /// <param name="sender">未使用</param>
        /// <param name="e">e.Parameterに遷移するページのType</param>
        private void OnNextPage(object sender, ExecutedRoutedEventArgs e)
        {
            if (PageStack.CanRedo)
            {
                historyTrace = true;
                this.Navigate(PageStack.Redo());
                CanUndo = PageStack.CanUndo;
                CanRedo = PageStack.CanRedo;
            }
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

                nav.PageStack = new HistoryStack<FrameworkElement>();

                // ナビゲーション用のコマンドバインディング
                element.CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage, nav.OnGoToPage));
                element.CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack, nav.OnPreviousPage));
                element.CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseForward, nav.OnNextPage));

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
        public static void Navigate(this FrameworkElement element, Type viewType)
        {
            var navigator = NavigationServiceEx.GetNavigator(element);
            navigator.Navigate(viewType);
        }
    }
}
