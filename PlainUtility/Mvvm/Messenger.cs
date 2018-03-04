using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Mvvm
{
    public interface IViewAction
    {
        void Register(FrameworkElement recipient);
    }

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
    public class Message
    {
        public Message(object sender)
        {
            Sender = sender;
        }
        public object Sender { get; protected set; }
    }

    public class Messenger
    {
        private static Messenger _instance = new Messenger();
        public static Messenger Default
        {
            get { return _instance; }
        }
        private List<ActionInfo> list = new List<ActionInfo>();
        public void Register<TMessage>(FrameworkElement recipient, Action<TMessage> action)
        {
            list.Add(new ActionInfo {
                Type = typeof(TMessage),
                //sender = recipient.DataContext as INotifyPropertyChanged,
                action = action,
            });
        }
        public void Send<TMessage>(INotifyPropertyChanged sender, TMessage message)
        {
            var query = list.Where(o =>/* o.sender == sender &&*/ o.Type == message.GetType()).Select(o => o.action as Action<TMessage>);
            foreach (var action in query)
            {
                action(message);
            }
        }
        private class ActionInfo
        {
            public Type Type { get; set; }
            //public INotifyPropertyChanged sender { get; set; }
            public Delegate action { get; set; }
        }
    }

    public class VmMessage
    {
        public VmMessage(object sender)
        {
            Sender = sender;
        }
        public object Sender { get; protected set; }
    }

    public class DialogBoxMessage : VmMessage
    {
    public DialogBoxMessage(object sender) : base(sender)
    {
    }
        public string Message { get; set; }
        public MessageBoxButton Button { get; set; }
        public MessageBoxResult Result { get; set; }
    }

    public class ActionCollection : System.Collections.ObjectModel.Collection<IViewAction>
    {
        public void RegisterAll(FrameworkElement Recipient)
        {
            foreach (var action in this)
            {
                action.Register(Recipient);
            }
        }
    }

    public class MessengerBehavior
    {
        public static ActionCollection GetActions(Control target)
        {
            return (ActionCollection)target.GetValue(ActionsProperty);
        }

        public static void SetActions(Control target, ActionCollection value)
        {
            target.SetValue(ActionsProperty, value);
        }

        private static void OnActionsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Control window = sender as Control;
            if (window == null)
                return;
            if ((ActionCollection)e.NewValue == null)
                return;
            ((ActionCollection)e.NewValue).RegisterAll(window);
        }

        public static readonly DependencyProperty ActionsProperty = DependencyProperty.RegisterAttached(
                "Actions",
                typeof(ActionCollection),
                typeof(MessengerBehavior),
                new PropertyMetadata(null, OnActionsPropertyChanged));
    }
}
