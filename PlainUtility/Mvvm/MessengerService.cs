using Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;

namespace Mvvm
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <a href="http://gushwell.ldblog.jp/archives/52146779.html">MVVM：Messengerを理解するために自作してみた(1)</a><br/>
    /// <a href="http://gushwell.ldblog.jp/archives/52146816.html">MVVM：Messengerを理解するために自作してみた(2)</a><br/>
    /// <a href="http://gushwell.ldblog.jp/archives/52146951.html">MVVM：Messengerを理解するために自作してみた(3)</a><br/>
    /// <a href="http://gushwell.ldblog.jp/archives/52146964.html">MVVM:Messenger + Behaviorを理解するために自作してみた(1)</a><br/>
    /// <a href="http://gushwell.ldblog.jp/archives/52146967.html">MVVM:Messenger + Behaviorを理解するために自作してみた(2)</a><br/>
    /// <a href="http://gushwell.ldblog.jp/archives/52152996.html">MVVM:Messenger + Behaviorを理解するために自作してみた(3)</a><br/>
    /// </remarks>
    public class MessengerService : DependencyObject
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string RequestNotifyPropertyName = nameof(MessengerService) + ":Request";
        /// <summary>
        /// 
        /// </summary>
        protected static List<KeyValuePair<object, IRequest>> Requests { get; private set; } = new List<KeyValuePair<object, IRequest>>();

        private object dataContext;
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        protected object DataContext
        {
            get { return dataContext; }
            set
            {
                // OldDataContextがあったら
                if (dataContext != null)
                {
                    // OldDataContextをKeyとしたりくえすとがあったら写像しておく
                    var list = Requests.Where(x => x.Key.Equals(dataContext)).ToArray();
                    if (list.Count() > 0)
                    {
                        // Keyだけ変更したいがKeyはリードオンリなのでいったん外す
                        foreach (var val in list)
                        {
                            MessengerService.Requests.Remove(val);
                        }
                        // KeyをMessengerService管轄にしておく
                        MessengerService.Requests.AddRange(list.Select(x => new KeyValuePair<object, IRequest>(this, x.Value)));
                    }
                }
                dataContext = value;
                // NewDataContextがあったら
                if (dataContext != null)
                {
                    // MessengerService管轄のリクエストを写像しておく
                    var list = Requests.Where(x => x.Key.Equals(this)).ToArray();
                    if (list.Count() > 0)
                    {
                        // Keyだけ変更したいがKeyはリードオンリなのでいったん外す
                        foreach (var val in list)
                        {
                            MessengerService.Requests.Remove(val);
                        }
                        // KeyをNewDataContext管轄にする
                        MessengerService.Requests.AddRange(list.Select(x => new KeyValuePair<object, IRequest>(dataContext, x.Value)));
                    }
                }
            }
        }

        private FrameworkElement targetView;
        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        protected FrameworkElement TargetView
        {
            get { return targetView; }
            private set
            {
                if (targetView != null)
                {
                    DataContext = null;
                    targetView.DataContextChanged -= Element_DataContextChanged;
                }
                targetView = value;
                if (targetView != null)
                {
                    targetView.DataContextChanged += Element_DataContextChanged;
                    if (targetView.DataContext != null)
                        Element_DataContextChanged(targetView, new DependencyPropertyChangedEventArgs(FrameworkElement.DataContextProperty, DataContext, targetView.DataContext));
                }
            }
        }
        private static void Element_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                MessengerService msgr = (MessengerService)element.GetValue(MessengerService.MessengerProperty);
                if (msgr != null)
                    msgr.DataContext = element.DataContext;
            }
        }

        /// <summary>
        /// Gets the messenger.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static MessengerService GetMessenger(DependencyObject obj)
        {
            return (MessengerService)obj.GetValue(MessengerProperty);
        }
        /// <summary>
        /// Sets the messenger.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        protected static void SetMessenger(DependencyObject obj, MessengerService value)
        {
            obj.SetValue(MessengerProperty, value);
        }
        /// <summary>
        /// The messenger property
        /// </summary>
        protected static readonly DependencyProperty MessengerProperty = DependencyProperty.RegisterAttached(
            "Messenger",
            typeof(MessengerService),
            typeof(MessengerService),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static RequestCollection GetRequests(FrameworkElement target)
        {
            return (RequestCollection)target.GetValue(RequestsProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public static void SetRequests(FrameworkElement target, RequestCollection value)
        {
            target.SetValue(RequestsProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RequestsProperty = DependencyProperty.RegisterAttached(
                "Requests",
                typeof(RequestCollection),
                typeof(MessengerService),
                new PropertyMetadata(null, OnRequestsPropertyChanged));

        private static void OnRequestsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                MessengerService msgr = MessengerService.GetMessenger(element);
                if (msgr == null)
                {
                    msgr = new MessengerService();
                    // MessengerServiceのインスタンスを、添付対象のコントロールに付加する。
                    MessengerService.SetMessenger(element, msgr);
                    // 添付対象のコントロールを、MessengerServiceのインスタンスに付加する。
                    msgr.TargetView = element;
                }
                // DataContextがあればKeyに、なければMessengerServiceのインスタンスをKeyに。
                foreach (var val in (IEnumerable<IRequest>)e.NewValue)
                {
                    MessengerService.Requests.Add(new KeyValuePair<object, IRequest>(msgr.DataContext ?? msgr, val));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IMessage SendMessage(Type typeOfViewModel, IMessage message)
        {
            var request = MessengerService.Requests
                .Where(x => x.Key.GetType().Equals(typeOfViewModel) && x.Value.MessageType.Equals(message.GetType()))
                .Select(x => x.Value).FirstOrDefault();
            request?.RequestAction?.Invoke(message);
            return message;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RequestCollection : Collection<IRequest>
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageRecipient
    {
        /// <summary>
        /// 
        /// </summary>
        IMessage Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool Closer { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// 
        /// </summary>
        string Title { get; }
        /// <summary>
        /// 
        /// </summary>
        object Content { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RequestMessage : IMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Content { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        Action<IMessage> RequestAction { get; }
        /// <summary>
        /// 
        /// </summary>
        Type TypeOfRecipientView { get; }
        /// <summary>
        /// 
        /// </summary>
        Type MessageType { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Request : DependencyObject, IRequest
    {
        /// <summary>
        /// 
        /// </summary>
        protected Request()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfRecipientView"></param>
        /// <param name="messageType"></param>
        public Request(Type typeOfRecipientView, Type messageType)
        {
            this.typeOfRecipientView = typeOfRecipientView;
            this.messageType = messageType;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfRecipientView"></param>
        /// <param name="messageType"></param>
        /// <param name="requestAction "></param>
        public Request(Type typeOfRecipientView, Type messageType, Action<IMessage> requestAction)
        {
            this.typeOfRecipientView = typeOfRecipientView;
            this.messageType = messageType;
            this.requestAction = requestAction;
        }
        /// <summary>
        /// 
        /// </summary>
        protected Action<IMessage> requestAction;
        /// <summary>
        /// 
        /// </summary>
        public Action<IMessage> RequestAction { get { return requestAction; } }
        /// <summary>
        /// 
        /// </summary>
        protected Type typeOfRecipientView;
        /// <summary>
        /// 
        /// </summary>
        public Type TypeOfRecipientView { get { return typeOfRecipientView; } }
        /// <summary>
        /// 
        /// </summary>
        protected Type messageType;
        /// <summary>
        /// 
        /// </summary>
        public Type MessageType { get { return messageType; } }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PopupWindowRequestToIRequestTypeConverter : TypeConverter
    {
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="destinationType"></param>
        ///// <returns></returns>
        //public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        //{
        //    return destinationType == typeof(IRequest);
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="culture"></param>
        ///// <param name="value"></param>
        ///// <param name="destinationType"></param>
        ///// <returns></returns>
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
    [TypeConverter(typeof(PopupWindowRequestToIRequestTypeConverter))]
    public class PopupWindowRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public PopupWindowRequest()
        {
            requestAction = ActionHnadler;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        protected virtual void OnCreateContent(ContentControl content)
        {
            if (!double.IsNaN(Width))
                content.Width = Width;
            if (!double.IsNaN(Height))
                content.Height = Height;
            if (!double.IsNaN(MinWidth))
                content.MinWidth = MinWidth;
            if (!double.IsNaN(MinHeight))
                content.MinHeight = MinHeight;
            if (!double.IsNaN(MaxWidth))
                content.MaxWidth = MaxWidth;
            if (!double.IsNaN(MaxHeight))
                content.MaxHeight = MaxHeight;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfRecipientView"></param>
        /// <param name="messageType"></param>
        public PopupWindowRequest(Type typeOfRecipientView, Type messageType)
            : base(typeOfRecipientView, messageType)
        {
            requestAction = ActionHnadler;
        }
        private void ActionHnadler(IMessage message)
        {
            var view = Activator.CreateInstance(TypeOfRecipientView);
            if (view is ContentControl content && content.DataContext is IMessageRecipient recipient)
            {
                recipient.Message = message;
                NoControlboxWindow window = new NoControlboxWindow
                {
                    ControlboxEnabled = ControlboxEnabled,
                    Content = content,
                    Owner = Application.Current.MainWindow,
                    Title = message.Title,
                    Icon = Icon,
                    WindowStyle = WindowStyle,
                    WindowState = WindowState,
                    ResizeMode = ResizeMode,
                    SizeToContent = SizeToContent,
                    WindowStartupLocation = WindowStartupLocation,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                };
                OnCreateContent(content);
                window.ShowDialog();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        new public Type TypeOfRecipientView
        {
            get { return typeOfRecipientView; }
            set
            {
                // TypeOfViewに代入された値がContentControlの派生クラスか、またはContentControlか
                if (value.IsSubclassOf(typeof(ContentControl)) || value.Equals(typeof(ContentControl)))
                {
                    typeOfRecipientView = value;
                }
                else
                {
                    throw new Exception("ViewにできるTypeはContentControlから派生している必要があります");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        new public Type MessageType
        {
            get { return messageType; }
            set
            {
                // MessageTypeに代入された値がIMessageに指定可能か
                if (typeof(IMessage).IsAssignableFrom(value))
                {
                    messageType = value;
                }
                else
                {
                    throw new Exception("MessageにできるTypeはIMessageを実装している必要があります");
                }
            }
        }
        public bool ControlboxEnabled { get; set; } = true;
        public ImageSource Icon { get; set; }
        public WindowStyle WindowStyle { get; set; } = WindowStyle.SingleBorderWindow;
        public WindowState WindowState { get; set; } = WindowState.Normal;
        public ResizeMode ResizeMode { get; set; } = ResizeMode.CanResize;
        public SizeToContent SizeToContent { get; set; } = SizeToContent.Manual;
        public WindowStartupLocation WindowStartupLocation { get; set; } = WindowStartupLocation.Manual;
        public double Width { get; set; } = double.NaN;
        public double MinWidth { get; set; } = double.NaN;
        public double MaxWidth { get; set; } = double.NaN;
        public double Height { get; set; } = double.NaN;
        public double MinHeight { get; set; } = double.NaN;
        public double MaxHeight { get; set; } = double.NaN;
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
            typeof(RecipientView),
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
}
