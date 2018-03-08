using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Utilitys;

namespace Mvvm
{
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
        /// <returns>アニメーションを再生するコンテント</returns>
        public FrameworkElement Initialize(FrameworkElement fromContent, FrameworkElement toContent)
        {
            this.toContent = toContent;
            if (toContent == null)
                return null;
            this.fromContent = fromContent;

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
        /// <param name="endAnimation">アニメーション終了時のイベントハンドラ</param>
        public void Animate(Action<FrameworkElement> endAnimation)
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

            story.Completed += (sender, e) => {
                storyBoard.Children.Remove(fromViewbox);
                storyBoard.Children.Remove(toViewbox);
                fromViewbox.Child = null;
                toViewbox.Child = null;
                toContentControl.Content = null;
                fromContentControl.Content = null;
                endAnimation?.Invoke(toContent);
                fromContent = null;
                toContent = null;
            };
            story.Begin();
        }
    }
}
