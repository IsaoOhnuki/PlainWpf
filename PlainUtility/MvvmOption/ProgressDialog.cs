using Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MvvmOption
{
    public enum ProgressDialogResult
    {
        /// <summary>
        /// The none
        /// </summary>
        Completed,
        /// <summary>
        /// The cancel
        /// </summary>
        Cancel,
    }

    public class ProgressDialogRequest : PopupWindowRequest
    {
        public double ContentMargin { get; set; } = double.NaN;
        public Brush ContentBackground { get; set; }
        public double ProgressWidth { get; set; } = double.NaN;
        public double ProgressHeight { get; set; } = double.NaN;
        public double ButtonWidth { get; set; } = double.NaN;
        public double ButtonHeight { get; set; } = double.NaN;
        public FontFamily ButtonFontFamily { get; set; }
        public double ButtonFontSize { get; set; } = double.NaN;
        public Brush ProgressBackground { get; set; }
        public Brush ProgressForeground { get; set; }
        public Brush ProgressBorder { get; set; }
        public Thickness ProgressBorderThickness { get; set; } = new Thickness(double.NaN);
        public Brush ButtonBackground { get; set; }
        public Brush ButtonForeground { get; set; }
        public Brush ButtonBorder { get; set; }
        public Thickness ButtonBorderThickness { get; set; } = new Thickness(double.NaN);
        public FontFamily LabelFontFamily { get; set; }
        public double LabelFontSize { get; set; } = double.NaN;
        public Brush LabelForeground { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmationDialogRequest"/> class.
        /// </summary>
        public ProgressDialogRequest()
            : base(typeof(ProgressDialogContent), typeof(ProgressDialogRequestMessage))
        {
            ControlboxEnabled = false;
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Normal;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        protected override void OnCreateContent(ContentControl content)
        {
            base.OnCreateContent(content);

            if (content is ProgressDialogContent dialogContent)
            {
                if (!double.IsNaN(ProgressWidth))
                {
                    dialogContent.ProgressWidth = ProgressWidth;
                }
                if (!double.IsNaN(ProgressHeight))
                {
                    dialogContent.ProgressHeight = ProgressHeight;
                }
                if (!double.IsNaN(ButtonWidth))
                {
                    dialogContent.ButtonWidth = ButtonWidth;
                }
                if (!double.IsNaN(ButtonHeight))
                {
                    dialogContent.ButtonHeight = ButtonHeight;
                }
                if (!double.IsNaN(ContentMargin))
                {
                    dialogContent.ControlMargin = ContentMargin;
                }
                if (!double.IsNaN(LabelFontSize))
                {
                    dialogContent.LabelFontSize = LabelFontSize;
                }
                if (LabelFontFamily != null)
                {
                    dialogContent.LabelFontFamily = LabelFontFamily;
                }
                if (!double.IsNaN(ButtonFontSize))
                {
                    dialogContent.ButtonFontSize = ButtonFontSize;
                }
                if (ButtonFontFamily != null)
                {
                    dialogContent.ButtonFontFamily = ButtonFontFamily;
                }
                if (!double.IsNaN(ProgressBorderThickness.Left) || !double.IsNaN(ProgressBorderThickness.Top) || !double.IsNaN(ProgressBorderThickness.Right) || !double.IsNaN(ProgressBorderThickness.Bottom))
                {
                    dialogContent.ProgressBorderThickness = ProgressBorderThickness;
                }
                if (!double.IsNaN(ButtonBorderThickness.Left) || !double.IsNaN(ButtonBorderThickness.Top) || !double.IsNaN(ButtonBorderThickness.Right) || !double.IsNaN(ButtonBorderThickness.Bottom))
                {
                    dialogContent.ButtonBorderThickness = ButtonBorderThickness;
                }
                if (ContentBackground != null)
                {
                    dialogContent.ContentBackground = ContentBackground;
                }
                if (ProgressBackground != null)
                {
                    dialogContent.ProgressBackground = ProgressBackground;
                }
                if (ProgressForeground != null)
                {
                    dialogContent.ProgressForeground = ProgressForeground;
                }
                if (ButtonBackground != null)
                {
                    dialogContent.ButtonBackground = ButtonBackground;
                }
                if (ButtonForeground != null)
                {
                    dialogContent.ButtonForeground = ButtonForeground;
                }
                if (LabelForeground != null)
                {
                    dialogContent.LabelForeground = LabelForeground;
                }
                if (ButtonBorder != null)
                {
                    dialogContent.ButtonBorder = ButtonBorder;
                }
                if (ProgressBorder != null)
                {
                    dialogContent.ProgressBorder = ProgressBorder;
                }
            }
        }
    }

    public class ProgressDialogRequestMessage : RequestMessage
    {
        /// <summary>
        /// Gets or sets the button1 text.
        /// </summary>
        /// <value>
        /// The button1 text.
        /// </value>
        public string Button1Text { get; set; }
        public ProgressDialogResult ProgressDialogResult { get; set; }
        //public CancellationTokenSource CancellationTokenSource { get; set; }
        //public Task Work { get; set; }
        public Action<CancellationToken> WorkAction { get; set; }
        public Action<ProgressDialogResult> WorkCompleted { get; set; }
    }

    public class ProgressDialogContent : RecipientView
    {
        private Grid grid;
        private TextBlock textBlock;
        private Button button1;
        private ProgressBar progress;
        private StackPanel buttonPanel;
        private double contentMargin = 10;
        public double ControlMargin
        {
            get { return contentMargin; }
            set
            {
                contentMargin = value;
                textBlock.Margin = new Thickness(ControlMargin);
                button1.Margin = new Thickness(0, 0, 0, 0);
                buttonPanel.Margin = new Thickness(ControlMargin, 0, ControlMargin, ControlMargin);
            }
        }
        private Brush contentBackground;
        public Brush ContentBackground
        {
            get { return contentBackground; }
            set
            {
                contentBackground = value;
                grid.Background = ContentBackground;
            }
        }
        private Brush progressBackground;
        public Brush ProgressBackground
        {
            get { return progressBackground; }
            set
            {
                progressBackground = value;
                progress.Background = progressBackground;
            }
        }
        private Brush progressForeground;
        public Brush ProgressForeground
        {
            get { return progressForeground; }
            set
            {
                progressForeground = value;
                progress.Foreground = progressForeground;
            }
        }
        private Brush progressBorder;
        public Brush ProgressBorder
        {
            get { return progressBorder; }
            set
            {
                progressBorder = value;
                progress.BorderBrush = progressBorder;
            }
        }
        private Brush buttonBackground;
        public Brush ButtonBackground
        {
            get { return buttonBackground; }
            set
            {
                buttonBackground = value;
                button1.Background = buttonBackground;
            }
        }
        private Brush buttonForeground;
        public Brush ButtonForeground
        {
            get { return buttonForeground; }
            set
            {
                buttonForeground = value;
                button1.Foreground = buttonForeground;
            }
        }
        private Brush buttonBorder;
        public Brush ButtonBorder
        {
            get { return buttonBorder; }
            set
            {
                buttonBorder = value;
                button1.BorderBrush = buttonBorder;
            }
        }
        private Thickness progressBorderThickness;
        public Thickness ProgressBorderThickness
        {
            get { return progressBorderThickness; }
            set
            {
                progressBorderThickness = value;
                progress.BorderThickness = progressBorderThickness;
            }
        }
        private Thickness buttonBorderThickness;
        public Thickness ButtonBorderThickness
        {
            get { return buttonBorderThickness; }
            set
            {
                buttonBorderThickness = value;
                button1.BorderThickness = buttonBorderThickness;
            }
        }
        private Brush labelForeground;
        public Brush LabelForeground
        {
            get { return labelForeground; }
            set
            {
                labelForeground = value;
                textBlock.Foreground = labelForeground;
            }
        }
        private double progressWidth = 300;
        public double ProgressWidth
        {
            get { return progressWidth; }
            set
            {
                progressWidth = value;
                progress.Width = progressWidth;
            }
        }
        private double progressHeight = 20;
        public double ProgressHeight
        {
            get { return progressHeight; }
            set
            {
                progressHeight = value;
                progress.Height = progressHeight;
            }
        }
        private double buttonWidth = 80;
        public double ButtonWidth
        {
            get { return buttonWidth; }
            set
            {
                buttonWidth = value;
                button1.Width = ButtonWidth;
            }
        }
        private double buttonHeight = 20;
        public double ButtonHeight
        {
            get { return buttonHeight; }
            set
            {
                buttonHeight = value;
                button1.Height = ButtonHeight;
            }
        }
        private FontFamily buttonFontFamily = new FontFamily("Yu Gothic UI");
        public FontFamily ButtonFontFamily
        {
            get { return buttonFontFamily; }
            set
            {
                buttonFontFamily = value;
                button1.FontFamily = buttonFontFamily;
            }
        }
        private double buttonFontSize = 12;
        public double ButtonFontSize
        {
            get { return buttonFontSize; }
            set
            {
                buttonFontSize = value;
                button1.FontSize = buttonFontSize;
            }
        }
        private FontFamily labelFontFamily = new FontFamily("Yu Gothic UI");
        public FontFamily LabelFontFamily
        {
            get { return labelFontFamily; }
            set
            {
                labelFontFamily = value;
                textBlock.FontFamily = labelFontFamily;
            }
        }
        private double labelFontSize = 12;
        public double LabelFontSize
        {
            get { return labelFontSize; }
            set
            {
                labelFontSize = value;
                textBlock.FontSize = labelFontSize;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmationDialogContent"/> class.
        /// </summary>
        /// <param name="dialogType">Type of the dialog.</param>
        public ProgressDialogContent()
        {
            DataContext = new ProgressDialogContentViewModel();
            grid = new Grid();
            var row1 = new RowDefinition { Height = new System.Windows.GridLength(1, GridUnitType.Star) };
            var row2 = new RowDefinition { Height = new System.Windows.GridLength(0, GridUnitType.Auto) };
            var row3 = new RowDefinition { Height = new System.Windows.GridLength(0, GridUnitType.Auto) };
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            textBlock = new TextBlock { Name = "TextBlock", Margin = new Thickness(ControlMargin), FontFamily = LabelFontFamily, FontSize = LabelFontSize };
            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, new Binding("Text"));
            Grid.SetRow(textBlock, 0);
            grid.Children.Add(textBlock);
            progress = new ProgressBar { IsIndeterminate = true, Margin = new Thickness(ControlMargin, 0, ControlMargin, ControlMargin), Width = ProgressWidth, Height = ProgressHeight };
            Grid.SetRow(progress, 1);
            grid.Children.Add(progress);
            buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(ControlMargin, 0, ControlMargin, ControlMargin), FlowDirection = FlowDirection.RightToLeft };
            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);
            button1 = new Button { Name = "Button1", Margin = new Thickness(0, 0, 0, 0), Width = ButtonWidth, Height = ButtonHeight, FontFamily = ButtonFontFamily, FontSize = ButtonFontSize };
            BindingOperations.SetBinding(button1, Button.CommandProperty, new Binding("Button1Command"));
            BindingOperations.SetBinding(button1, Button.ContentProperty, new Binding("Button1Text"));
            BindingOperations.SetBinding(button1, Button.VisibilityProperty, new Binding("Button1Visibility"));
            buttonPanel.Children.Add(button1);
            Content = grid;
            //MinWidth = (ControlMargin + ButtonWidth) * (dialogType == ConfirmationDialogType.Ok ? 2 : dialogType == ConfirmationDialogType.OkCancel ? 3 : 4);
        }
    }

    public class ProgressDialogContentViewModel : BindableBase, IMessageRecipient
    {
        private bool closer = false;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ConfirmationDialogContentViewModel"/> is closer.
        /// </summary>
        /// <value>
        ///   <c>true</c> if closer; otherwise, <c>false</c>.
        /// </value>
        public bool Closer
        {
            get { return closer; }
            set { SetProperty(ref closer, value); }
        }
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; private set; }
        /// <summary>
        /// Gets the button1 command.
        /// </summary>
        /// <value>
        /// The button1 command.
        /// </value>
        public DelegateCommand Button1Command { get; private set; }
        /// <summary>
        /// Gets the button1 text.
        /// </summary>
        /// <value>
        /// The button1 text.
        /// </value>
        public string Button1Text { get; private set; }
        /// <summary>
        /// Gets the button1 visibility.
        /// </summary>
        /// <value>
        /// The button1 visibility.
        /// </value>
        public Visibility Button1Visibility { get; private set; }
        private ProgressDialogRequestMessage requestMessage;
        /// <summary>
        /// Gets or sets the request message.
        /// </summary>
        /// <value>
        /// The request message.
        /// </value>
        public ProgressDialogRequestMessage RequestMessage
        {
            get { return requestMessage; }
            set
            {
                SetProperty(ref requestMessage, value);

                if (!string.IsNullOrEmpty(RequestMessage.Button1Text))
                {
                    Button1Text = RequestMessage.Button1Text;
                }

                TaskWait();
            }
        }

        private CancellationTokenSource cancellationTokenSource;

        private async void TaskWait()
        {
            cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(() => requestMessage.WorkAction.Invoke(cancellationTokenSource.Token), cancellationTokenSource.Token);
            Closer = true;
            requestMessage.WorkCompleted?.Invoke(cancellationTokenSource.IsCancellationRequested ? ProgressDialogResult.Cancel : ProgressDialogResult.Completed);
        }

        private IMessage message;
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public IMessage Message
        {
            get { return message; }
            set
            {
                message = value;
                Text = message.Content?.ToString();
                RequestMessage = Message as ProgressDialogRequestMessage;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmationDialogContentViewModel"/> class.
        /// </summary>
        /// <param name="dialogType">Type of the dialog.</param>
        public ProgressDialogContentViewModel()
        {
            Button1Command = new DelegateCommand(() => {
                cancellationTokenSource.Cancel();
            });
            Button1Text = "キャンセル";
            Button1Visibility = Visibility.Visible;
        }
    }
}
