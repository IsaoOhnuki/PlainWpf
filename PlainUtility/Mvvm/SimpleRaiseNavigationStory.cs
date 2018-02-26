using Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Utilitys;

namespace PlainUtility.Mvvm
{
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

            Logger.Write(LogType.Debug, "RaiseNavigationMode" + NavigationMode.ToString());

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
}
