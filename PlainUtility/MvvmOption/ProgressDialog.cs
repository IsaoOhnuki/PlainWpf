using Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MvvmOption
{
    public enum ProgressDialogType
    {
        Cancel,
    }
    public enum ProgressDialogResult
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The cancel
        /// </summary>
        Cancel,
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
    }

    public class ProgressDialogContent : RecipientView
    {
        private Grid grid;
        private TextBlock textBlock;
        private Button button1;
        private ProgressBar progress;
        private StackPanel buttonPanel;
        private ProgressDialogType dialogType;
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
        public ProgressDialogContent(ProgressDialogType dialogType)
        {
            this.dialogType = dialogType;
            DataContext = new ProgressDialogContentViewModel(dialogType);
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
            progress = new ProgressBar { IsIndeterminate = true, Margin = new Thickness(ControlMargin, 0, ControlMargin, ControlMargin) };
            Grid.SetRow(progress, 2);
            buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(ControlMargin, 0, ControlMargin, ControlMargin), FlowDirection = FlowDirection.RightToLeft };
            Grid.SetRow(buttonPanel, 3);
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
            }
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
        public ProgressDialogContentViewModel(ProgressDialogType dialogType)
        {
            Button1Command = new DelegateCommand(() => {
                RequestMessage.ProgressDialogResult = dialogType == ProgressDialogType.Cancel ? ProgressDialogResult.Cancel : ProgressDialogResult.None;
                Closer = true;
            });
            Button1Text = dialogType == ProgressDialogType.Cancel ? "キャンセル" : "";
            Button1Visibility = dialogType == ProgressDialogType.Cancel ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
