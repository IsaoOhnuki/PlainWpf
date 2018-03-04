using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                if (dataContext != null)
                {
                    var list = Requests.Where(x => x.Key.Equals(dataContext)).ToArray();
                    if (list.Count() > 0)
                    {
                        foreach (var val in list)
                        {
                            MessengerService.Requests.Remove(val);
                        }
                        MessengerService.Requests.AddRange(list.Select(x => new KeyValuePair<object, IRequest>(this, x.Value)));
                    }
                }
                dataContext = value;
                if (dataContext != null)
                {
                    var list = Requests.Where(x => x.Key.Equals(this)).ToArray();
                    if (list.Count() > 0)
                    {
                        foreach (var val in list)
                        {
                            MessengerService.Requests.Remove(val);
                        }
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
        public static IEnumerable<IRequest> GetRequests(FrameworkElement target)
        {
            return (IEnumerable<IRequest>)target.GetValue(RequestsProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public static void SetRequests(FrameworkElement target, IEnumerable<IRequest> value)
        {
            target.SetValue(RequestsProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RequestsProperty = DependencyProperty.RegisterAttached(
                "Requests",
                typeof(IEnumerable<IRequest>),
                typeof(MessengerService),
                new PropertyMetadata(null, OnRequestsPropertyChanged));

        private static void OnRequestsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                MessengerService msgr = MessengerService.GetMessenger(element);
                if (msgr == null)
                {
                    // 添付対象のコントロールを、MessengerServiceのインスタンスに付加する。
                    msgr = new MessengerService { TargetView = element };
                    // MessengerServiceのインスタンスを、添付対象のコントロールに付加する。
                    MessengerService.SetMessenger(element, msgr);
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
            var request = MessengerService.Requests.Where(x => x.Key.GetType().Equals(typeOfViewModel) && x.Value.MessageType.Equals(message.GetType()))
                .Select(x => x.Value).FirstOrDefault();
            if (request != null)
            {
                var view = Activator.CreateInstance(request.TypeOfRecipientView);
                if (view is FrameworkElement element && element.DataContext is IMessageRecipient recipient)
                {
                    recipient.Message = message;
                    var window = new Window { Content = element, Owner = Application.Current.MainWindow
                        , Title = message.Title
                        , SizeToContent = SizeToContent.WidthAndHeight
                        , VerticalContentAlignment = VerticalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch };
                    window.ShowDialog();
                }
            }
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
    public class Request : IRequest
    {
        private Type typeOfRecipientView;
        /// <summary>
        /// 
        /// </summary>
        public Type TypeOfRecipientView
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
        private Type messageType;
        /// <summary>
        /// 
        /// </summary>
        public Type MessageType
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
    }

    public class PopupWindowRequest : Request
    {
        public new bool WindowContent { get; set; } = true;
        public WindowStyle WindowStyle { get; set; }
        public WindowState WindowState { get; set; }
        public ResizeMode ResizeMode { get; set; }
        public SizeToContent SizeToContent { get; set; }
        public double Width { get; set; }
        public double MinWidth { get; set; }
        public double MaxWidth { get; set; }
        public double Height { get; set; }
        public double MinHeight { get; set; }
        public double MaxHeight { get; set; }
    }
}
