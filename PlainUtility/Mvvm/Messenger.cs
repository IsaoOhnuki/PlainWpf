using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Mvvm
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks><a href="http://gushwell.ldblog.jp/archives/52146779.html">MVVM：Messengerを理解するために自作してみた(1)</a></remarks>
    /// <remarks><a href="http://gushwell.ldblog.jp/archives/52146816.html">MVVM：Messengerを理解するために自作してみた(2)</a></remarks>
    /// <remarks><a href="http://gushwell.ldblog.jp/archives/52146951.html">MVVM：Messengerを理解するために自作してみた(3)</a></remarks>
    /// <remarks><a href="http://gushwell.ldblog.jp/archives/52146964.html">MVVM:Messenger + Behaviorを理解するために自作してみた(1)</a></remarks>
    /// <remarks><a href="http://gushwell.ldblog.jp/archives/52146967.html">MVVM:Messenger + Behaviorを理解するために自作してみた(2)</a></remarks>
    /// <remarks><a href="http://gushwell.ldblog.jp/archives/52152996.html">MVVM:Messenger + Behaviorを理解するために自作してみた(3)</a></remarks>
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
                sender = recipient.DataContext as INotifyPropertyChanged,
                action = action,
            });
        }
        public void Send<TMessage>(INotifyPropertyChanged sender, TMessage message)
        {
            var query = list.Where(o => o.sender == sender && o.Type == message.GetType()).Select(o => o.action as Action<TMessage>);
            foreach (var action in query)
            {
                action(message);
            }
        }
        private class ActionInfo
        {
            public Type Type { get; set; }
            public INotifyPropertyChanged sender { get; set; }
            public Delegate action { get; set; }
        }
    }
}
