using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Mvvm
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmationDialogRequest"/> class.
        /// </summary>
        public ConfirmationDialogRequest()
        {
            WindowStyle = WindowStyle.ToolWindow;
            WindowState = WindowState.Normal;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //Width;
            //MinWidth;
            //MaxWidth;
            //Height;
            //MinHeight;
            //MaxHeight;
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
    /// <summary>
    /// 
    /// </summary>
    public class OkDialogRequest : ConfirmationDialogRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OkDialogRequest"/> class.
        /// </summary>
        public OkDialogRequest()
        {
            TypeOfRecipientView = typeof(OkDialogContent);
            MessageType = typeof(OkDialogRequestMessage);
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
    /// <summary>
    /// 
    /// </summary>
    public class OkCancelDialogRequest : ConfirmationDialogRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OkCancelDialogRequest"/> class.
        /// </summary>
        public OkCancelDialogRequest()
        {
            TypeOfRecipientView = typeof(OkCancelDialogContent);
            MessageType = typeof(OkCancelDialogRequestMessage);
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
    public class YesNoCancelDialogRequest : ConfirmationDialogRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YesNoCancelDialogRequest"/> class.
        /// </summary>
        public YesNoCancelDialogRequest()
        {
            TypeOfRecipientView = typeof(YesNoCancelDialogContent);
            MessageType = typeof(YesNoCancelDialogRequestMessage);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RecipientView : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipientView"/> class.
        /// </summary>
        public RecipientView()
        {
            BindingOperations.SetBinding(this, CloserProperty, new Binding("Closer") { Mode = BindingMode.TwoWay });
        }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RecipientView"/> is closer.
        /// </summary>
        /// <value>
        ///   <c>true</c> if closer; otherwise, <c>false</c>.
        /// </value>
        public bool Closer
        {
            get { return (bool)GetValue(CloserProperty); }
            set { SetValue(CloserProperty, value); }
        }
        /// <summary>
        /// The closer property
        /// </summary>
        public static readonly DependencyProperty CloserProperty = DependencyProperty.Register(
            "Closer",
            typeof(bool),
            typeof(ConfirmationDialogContent),
            new PropertyMetadata(default(bool), (sender, e) => {
                if (sender is ContentControl control && control.Parent is Window owner)
                {
                    if ((bool)e.NewValue)
                    {
                        owner.DialogResult = true;
                    }
                }
            }));
    }
    /// <summary>
    /// 
    /// </summary>
    public class ConfirmationDialogContent : RecipientView
    {
        readonly double marginSize = 10;
        readonly double buttonWidth = 70;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmationDialogContent"/> class.
        /// </summary>
        /// <param name="dialogType">Type of the dialog.</param>
        public ConfirmationDialogContent(ConfirmationDialogType dialogType)
        {
            DataContext = new ConfirmationDialogContentViewModel(dialogType);
            var grid = new Grid();
            var row1 = new RowDefinition { Height = new System.Windows.GridLength(1, GridUnitType.Star) };
            var row2 = new RowDefinition { Height = new System.Windows.GridLength(0, GridUnitType.Auto) };
            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            var text = new TextBlock { Margin = new Thickness(marginSize, marginSize, marginSize, 0) };
            BindingOperations.SetBinding(text, TextBlock.TextProperty, new Binding("Text"));
            Grid.SetRow(text, 0);
            grid.Children.Add(text);
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(marginSize), FlowDirection = FlowDirection.RightToLeft };
            Grid.SetRow(buttonPanel, 1);
            grid.Children.Add(buttonPanel);
            var button1 = new Button { Margin = new Thickness(0, 0, marginSize, 0), Width = buttonWidth };
            var button2 = new Button { Margin = new Thickness(0, 0, marginSize, 0), Width = buttonWidth };
            var button3 = new Button { Margin = new Thickness(0, 0, marginSize, 0), Width = buttonWidth };
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
            MinWidth = (marginSize + buttonWidth) * (dialogType == ConfirmationDialogType.Ok ? 2 : dialogType == ConfirmationDialogType.OkCancel ? 3 : 4);
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
            Button1Text = dialogType == ConfirmationDialogType.YesNoCancel　? "はい" : "OK";
            Button2Text = dialogType == ConfirmationDialogType.YesNoCancel ? "いいえ" : "キャンセル";
            Button3Text = "キャンセル";
            Button1Visibility = Visibility.Visible;
            Button2Visibility = dialogType != ConfirmationDialogType.Ok ? Visibility.Visible : Visibility.Collapsed;
            Button3Visibility = dialogType == ConfirmationDialogType.YesNoCancel ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
