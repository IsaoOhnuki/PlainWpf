using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    public class FormatTextInputBehavior
    {
        ///<summary>
        /// Get
        ///</summary>
        ///<param name="target">DependencyObject</param>
        ///<returns>ICommand</returns>
        public static string GetFormatText(DependencyObject target)
        {
            return (string)target.GetValue(FormatTextProperty);
        }

        ///<summary>
        /// Set
        ///</summary>
        ///<param name="target">DependencyObject</param>
        ///<param name="value">ICommand</param>
        public static void SetFormatText(DependencyObject target, string value)
        {
            target.SetValue(FormatTextProperty, value);
        }

        ///<summary>
        /// DependencyProperty
        ///</summary>
        public static readonly DependencyProperty FormatTextProperty = DependencyProperty.RegisterAttached(
            "FormatText",
            typeof(string),
            typeof(FormatTextInputBehavior),
            new PropertyMetadata(false, OnFormatTextChanged));

        ///<summary>1
        /// OnChanged
        ///</summary>
        ///<param name="target">DependencyObject</param>
        ///<param name="e">DependencyPropertyChangedEventArgs</param>
        public static void OnFormatTextChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target.GetType() == typeof(TextBox))
            {
                TextBox t = (TextBox)target;
                if ((bool)e.NewValue)
                {
                    t.PreviewKeyDown += T_PreviewKeyDown;
                }
                else
                {
                    t.PreviewKeyDown -= T_PreviewKeyDown;
                }
            }
        }

        private static void T_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)e.Source;

            if (!string.IsNullOrEmpty(textBox.Text))
            {
                Key[] BAD_KEYS = new Key[] { Key.Back, Key.Delete };
                Key[] WRK_KEYS = new Key[] { Key.Left, Key.Up, Key.Right, Key.Down, Key.Enter };
                if (BAD_KEYS.Contains(e.Key))
                {
                    e.Handled = true;
                }
                else if (textBox.CaretIndex == textBox.Text.Length && e.Key != Key.Left)
                {
                    e.Handled = true;
                }
                else if (WRK_KEYS.Contains(e.Key) == false && string.IsNullOrEmpty(textBox.Text) == false)
                {
                    textBox.Select(textBox.CaretIndex, 1);
                }
            }
        }
    }
}
