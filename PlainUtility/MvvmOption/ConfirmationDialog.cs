using Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MvvmOption
{
    /// <summary>
    /// 
    /// </summary>
    public enum ConfirmationDialogType
    {
        /// <summary>
        /// The ok
        /// </summary>
        Ok,
        /// <summary>
        /// The ok cancel
        /// </summary>
        OkCancel,
        /// <summary>
        /// The yes no cancel
        /// </summary>
        YesNoCancel,
    }
    /// <summary>
    /// 
    /// </summary>
    public enum ConfirmationDialogResult
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The ok
        /// </summary>
        Ok,
        /// <summary>
        /// The cancel
        /// </summary>
        Cancel,
        /// <summary>
        /// The yes
        /// </summary>
        Yes,
        /// <summary>
        /// The no
        /// </summary>
        No,
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConfirmationDialogRequestMessage : RequestMessage
    {
        /// <summary>
        /// Gets or sets the button1 text.
        /// </summary>
        /// <value>
        /// The button1 text.
        /// </value>
        public string Button1Text { get; set; }
        /// <summary>
        /// Gets or sets the button2 text.
        /// </summary>
        /// <value>
        /// The button2 text.
        /// </value>
        public string Button2Text { get; set; }
        /// <summary>
        /// Gets or sets the button3 text.
        /// </summary>
        /// <value>
        /// The button3 text.
        /// </value>
        public string Button3Text { get; set; }
        /// <summary>
        /// Gets or sets the confirmation dialog result.
        /// </summary>
        /// <value>
        /// The confirmation dialog result.
        /// </value>
        public ConfirmationDialogResult ConfirmationDialogResult { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ConfirmationDialogRequest : PopupWindowRequest
    {
        public double ControlMargin { get; set; } = double.NaN;
        public double ButtonWidth { get; set; } = double.NaN;
        public double ButtonHeight { get; set; } = double.NaN;
        public FontFamily ButtonFontFamily { get; set; }
        public double ButtonFontSize { get; set; } = double.NaN;
        public FontFamily LabelFontFamily { get; set; }
        public double LabelFontSize { get; set; } = double.NaN;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmationDialogRequest"/> class.
        /// </summary>
        public ConfirmationDialogRequest(Type typeOfRecipientView, Type messageType)
            : base(typeOfRecipientView, messageType)
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

            if (content is ConfirmationDialogContent dialogContent)
            {
                if (!double.IsNaN(ButtonWidth))
                {
                    dialogContent.ButtonWidth = ButtonWidth;
                }
                if (!double.IsNaN(ButtonHeight))
                {
                    dialogContent.ButtonHeight = ButtonHeight;
                }
                if (!double.IsNaN(ControlMargin))
                {
                    dialogContent.ControlMargin = ControlMargin;
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
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class OkDialogContent : ConfirmationDialogContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OkDialogContent"/> class.
        /// </summary>
        public OkDialogContent()
            : base(ConfirmationDialogType.Ok)
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class OkDialogRequestMessage : ConfirmationDialogRequestMessage
    {
    }

    public class OkDialogRequestToIRequestTypeConverter : TypeConverter
    {
        //public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        //{
        //    return destinationType == typeof(IRequest);
        //}

        //public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        //{
        //    return value as IRequest;
        //}

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(IRequest);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value as IRequest;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [TypeConverter(typeof(OkDialogRequestToIRequestTypeConverter))]
    public class OkDialogRequest : ConfirmationDialogRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OkDialogRequest"/> class.
        /// </summary>
        public OkDialogRequest()
            : base(typeof(OkDialogContent), typeof(OkDialogRequestMessage))
        {
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class OkCancelDialogContent : ConfirmationDialogContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OkCancelDialogContent"/> class.
        /// </summary>
        public OkCancelDialogContent()
            : base(ConfirmationDialogType.OkCancel)
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class OkCancelDialogRequestMessage : ConfirmationDialogRequestMessage
    {
    }

    public class OkCancelDialogRequestToIRequestTypeConverter : TypeConverter
    {
        //public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        //{
        //    return destinationType == typeof(IRequest);
        //}

        //public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        //{
        //    return value as IRequest;
        //}

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(IRequest);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value as IRequest;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [TypeConverter(typeof(OkCancelDialogRequestToIRequestTypeConverter))]
    public class OkCancelDialogRequest : ConfirmationDialogRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OkCancelDialogRequest"/> class.
        /// </summary>
        public OkCancelDialogRequest()
            : base(typeof(OkCancelDialogContent), typeof(OkCancelDialogRequestMessage))
        {
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class YesNoCancelDialogContent : ConfirmationDialogContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YesNoCancelDialogContent"/> class.
        /// </summary>
        public YesNoCancelDialogContent()
            : base(ConfirmationDialogType.YesNoCancel)
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class YesNoCancelDialogRequestMessage : ConfirmationDialogRequestMessage
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public class YesNoCancelDialogRequestToIRequestTypeConverter : TypeConverter
    {
        //public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        //{
        //    return destinationType == typeof(IRequest);
        //}

        //public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        //{
        //    return value as IRequest;
        //}

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(IRequest);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value as IRequest;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [TypeConverter(typeof(YesNoCancelDialogRequestToIRequestTypeConverter))]
    public class YesNoCancelDialogRequest : ConfirmationDialogRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YesNoCancelDialogRequest"/> class.
        /// </summary>
        public YesNoCancelDialogRequest()
            : base(typeof(YesNoCancelDialogContent), typeof(YesNoCancelDialogRequestMessage))
        {
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ConfirmationDialogContent : RecipientView
    {
        private TextBlock textBlock;
        private Button button1;
        private Button button2;
        private Button button3;
        private StackPanel buttonPanel;
        private ConfirmationDialogType dialogType;
        private double controlMargin = 10;
        public double ControlMargin
        {
            get { return controlMargin; }
            set
            {
                controlMargin = value;
                textBlock.Margin = new Thickness(ControlMargin);
                button1.Margin = new Thickness(dialogType == ConfirmationDialogType.Ok ? 0 : ControlMargin, 0, 0, 0);
                button2.Margin = new Thickness(dialogType == ConfirmationDialogType.OkCancel ? 0 : ControlMargin, 0, 0, 0);
                button3.Margin = new Thickness(0, 0, 0, 0);
                buttonPanel.Margin = new Thickness(ControlMargin, 0, ControlMargin, ControlMargin);
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
                button2.Width = ButtonWidth;
                button3.Width = ButtonWidth;
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
                button2.Height = ButtonHeight;
                button3.Height = ButtonHeight;
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
                button2.FontFamily = buttonFontFamily;
                button3.FontFamily = buttonFontFamily;
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
                button2.FontSize = buttonFontSize;
                button3.FontSize = buttonFontSize;
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
        public ConfirmationDialogContent(ConfirmationDialogType dialogType)
        {
            this.dialogType = dialogType;
            DataContext = new ConfirmationDialogContentViewModel(dialogType);
            var grid = new Grid();
            var row1 = new RowDefinition { Height = new System.Windows.GridLength(1, GridUnitType.Star) };
            var row2 = new RowDefinition { Height = new System.Windows.GridLength(0, GridUnitType.Auto) };
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            textBlock = new TextBlock { Name = "TextBlock", Margin = new Thickness(ControlMargin), FontFamily = LabelFontFamily, FontSize = LabelFontSize };
            BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, new Binding("Text"));
            Grid.SetRow(textBlock, 0);
            grid.Children.Add(textBlock);
            buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(ControlMargin, 0, ControlMargin, ControlMargin), FlowDirection = FlowDirection.RightToLeft };
            Grid.SetRow(buttonPanel, 1);
            grid.Children.Add(buttonPanel);
            button1 = new Button { Name = "Button1", Margin = new Thickness(dialogType == ConfirmationDialogType.Ok ? 0 : ControlMargin, 0, 0, 0), Width = ButtonWidth, Height = ButtonHeight, FontFamily = ButtonFontFamily, FontSize = ButtonFontSize };
            button2 = new Button { Name = "Button2", Margin = new Thickness(dialogType == ConfirmationDialogType.OkCancel ? 0 : ControlMargin, 0, 0, 0), Width = ButtonWidth, Height = ButtonHeight, FontFamily = ButtonFontFamily, FontSize = ButtonFontSize };
            button3 = new Button { Name = "Button3", Margin = new Thickness(0, 0, 0, 0), Width = ButtonWidth, Height = ButtonHeight, FontFamily = ButtonFontFamily, FontSize = ButtonFontSize };
            BindingOperations.SetBinding(button1, Button.CommandProperty, new Binding("Button1Command"));
            BindingOperations.SetBinding(button2, Button.CommandProperty, new Binding("Button2Command"));
            BindingOperations.SetBinding(button3, Button.CommandProperty, new Binding("Button3Command"));
            BindingOperations.SetBinding(button1, Button.ContentProperty, new Binding("Button1Text"));
            BindingOperations.SetBinding(button2, Button.ContentProperty, new Binding("Button2Text"));
            BindingOperations.SetBinding(button3, Button.ContentProperty, new Binding("Button3Text"));
            BindingOperations.SetBinding(button1, Button.VisibilityProperty, new Binding("Button1Visibility"));
            BindingOperations.SetBinding(button2, Button.VisibilityProperty, new Binding("Button2Visibility"));
            BindingOperations.SetBinding(button3, Button.VisibilityProperty, new Binding("Button3Visibility"));
            buttonPanel.Children.Add(button3);
            buttonPanel.Children.Add(button2);
            buttonPanel.Children.Add(button1);
            Content = grid;
            //MinWidth = (ControlMargin + ButtonWidth) * (dialogType == ConfirmationDialogType.Ok ? 2 : dialogType == ConfirmationDialogType.OkCancel ? 3 : 4);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ConfirmationDialogContentViewModel : BindableBase, IMessageRecipient
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
        /// Gets the button2 command.
        /// </summary>
        /// <value>
        /// The button2 command.
        /// </value>
        public DelegateCommand Button2Command { get; private set; }
        /// <summary>
        /// Gets the button3 command.
        /// </summary>
        /// <value>
        /// The button3 command.
        /// </value>
        public DelegateCommand Button3Command { get; private set; }
        /// <summary>
        /// Gets the button1 text.
        /// </summary>
        /// <value>
        /// The button1 text.
        /// </value>
        public string Button1Text { get; private set; }
        /// <summary>
        /// Gets the button2 text.
        /// </summary>
        /// <value>
        /// The button2 text.
        /// </value>
        public string Button2Text { get; private set; }
        /// <summary>
        /// Gets the button3 text.
        /// </summary>
        /// <value>
        /// The button3 text.
        /// </value>
        public string Button3Text { get; private set; }
        /// <summary>
        /// Gets the button1 visibility.
        /// </summary>
        /// <value>
        /// The button1 visibility.
        /// </value>
        public Visibility Button1Visibility { get; private set; }
        /// <summary>
        /// Gets the button2 visibility.
        /// </summary>
        /// <value>
        /// The button2 visibility.
        /// </value>
        public Visibility Button2Visibility { get; private set; }
        /// <summary>
        /// Gets the button3 visibility.
        /// </summary>
        /// <value>
        /// The button3 visibility.
        /// </value>
        public Visibility Button3Visibility { get; private set; }
        private ConfirmationDialogRequestMessage requestMessage;
        /// <summary>
        /// Gets or sets the request message.
        /// </summary>
        /// <value>
        /// The request message.
        /// </value>
        public ConfirmationDialogRequestMessage RequestMessage
        {
            get { return requestMessage; }
            set
            {
                SetProperty(ref requestMessage, value);

                if (!string.IsNullOrEmpty(RequestMessage.Button1Text))
                {
                    Button1Text = RequestMessage.Button1Text;
                }
                if (!string.IsNullOrEmpty(RequestMessage.Button2Text))
                {
                    Button2Text = RequestMessage.Button2Text;
                }
                if (!string.IsNullOrEmpty(RequestMessage.Button3Text))
                {
                    Button3Text = RequestMessage.Button3Text;
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
                RequestMessage = Message as ConfirmationDialogRequestMessage;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmationDialogContentViewModel"/> class.
        /// </summary>
        /// <param name="dialogType">Type of the dialog.</param>
        public ConfirmationDialogContentViewModel(ConfirmationDialogType dialogType)
        {
            Button1Command = new DelegateCommand(() => {
                RequestMessage.ConfirmationDialogResult = dialogType == ConfirmationDialogType.YesNoCancel ? ConfirmationDialogResult.Yes : ConfirmationDialogResult.Ok;
                Closer = true;
            });
            Button2Command = new DelegateCommand(() => {
                RequestMessage.ConfirmationDialogResult = dialogType == ConfirmationDialogType.YesNoCancel ? ConfirmationDialogResult.No : ConfirmationDialogResult.Cancel;
                Closer = true;
            });
            Button3Command = new DelegateCommand(() => {
                RequestMessage.ConfirmationDialogResult = ConfirmationDialogResult.Cancel;
                Closer = true;
            });
            Button1Text = dialogType == ConfirmationDialogType.YesNoCancel ? "はい" : "OK";
            Button2Text = dialogType == ConfirmationDialogType.YesNoCancel ? "いいえ" : "キャンセル";
            Button3Text = "キャンセル";
            Button1Visibility = Visibility.Visible;
            Button2Visibility = dialogType != ConfirmationDialogType.Ok ? Visibility.Visible : Visibility.Collapsed;
            Button3Visibility = dialogType == ConfirmationDialogType.YesNoCancel ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
