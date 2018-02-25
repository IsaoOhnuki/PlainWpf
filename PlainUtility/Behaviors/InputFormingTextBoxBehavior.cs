using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Text.RegularExpressions;
using Extensions;

namespace Behaviors
{
    /// <summary>
    /// フォーマットされた書式での入力制限テキストボックス
    /// </summary>
    public class InputFormingTextBoxBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Called when [attached].
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }
        /// <summary>
        /// Called when [detaching].
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }
        /// <summary>
        /// Raises the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }
        /// <summary>
        /// Called when [changed].
        /// </summary>
        protected override void OnChanged()
        {
            base.OnChanged();
        }

        private enum MatchItemType
        {
            Digit,
            XDigit,
        }
        private class MatchItem
        {
            public MatchItemType MatchItemType;
            public int Length;
        }
        private List<MatchItem> inputMatchStringItems = new List<MatchItem>();
        private string matchingPattern;
        private string matchString;
        /// <summary>
        /// テキストボックスの文字列検証用のパターン
        /// </summary>
        /// <value>
        /// #は10進数字桁、@は16進数字桁、¥先行でエスケープ、¥文字は\\と表記する。
        /// </value>
        public string MatchString
        {
            get { return matchString; }
            set
            {
                matchString = value;
                var matches = Regex.Matches(matchString, @"(\\\\)|(\\#)|(\\@)|((?<!\\)#)+|((?<!\\)@)+");
                inputMatchStringItems.Clear();
                StringBuilder inputMatchStr = new StringBuilder();
                int regStrIndex = 0;
                foreach (Match match in matches)
                {
                    if (regStrIndex < match.Index)
                    {
                        int sizs = match.Index - regStrIndex;
                        inputMatchStr.Append(matchString.Substring(regStrIndex, sizs).MatchPossibleStringToUnMatchString());
                        regStrIndex += sizs;
                    }
                    switch (matchString[regStrIndex])
                    {
                        case '#':
                            inputMatchStringItems.Add(new MatchItem { MatchItemType = MatchItemType.Digit, Length = match.Length });
                            inputMatchStr.Append(@"(\\d+)");
                            break;
                        case '@':
                            inputMatchStringItems.Add(new MatchItem { MatchItemType = MatchItemType.XDigit, Length = match.Length });
                            inputMatchStr.Append(@"(\\x+)");
                            break;
                        case '\\':
                            inputMatchStr.Append(matchString.Substring(match.Index + 1, 1));
                            break;
                    }
                    regStrIndex += match.Length;
                }
                if (regStrIndex < matchString.Length)
                {
                    inputMatchStr.Append(matchString.Substring(regStrIndex).MatchPossibleStringToUnMatchString());
                }
                matchingPattern = inputMatchStr.ToString();
            }
        }
        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            string targetString = this.AssociatedObject.Text;
            if (!string.IsNullOrWhiteSpace(MatchString) && !string.IsNullOrWhiteSpace(targetString))
            {
                StringBuilder retString = new StringBuilder();
                int retIndex = 0;
                var matches = Regex.Matches(targetString, matchingPattern);
                for (int index = 0; index < matches.Count && index < inputMatchStringItems.Count; ++index)
                {
                    if (retIndex < matches[index].Length)
                    {
                        int size = matches[index].Index - retIndex;
                        retString.Append(targetString.Substring(retIndex, size));
                        retIndex += size;
                    }
                    if (matches[index].Length < inputMatchStringItems[index].Length)
                    {
                        retString.Append(targetString.Substring(matches[index].Index, matches[index].Length).PadLeft(inputMatchStringItems[index].Length, '0'));
                    }
                    else if (matches[index].Length > inputMatchStringItems[index].Length)
                    {
                        retString.Append(targetString.Substring(matches[index].Index, matches[index].Length).Substring(matches[index].Length - inputMatchStringItems[index].Length));
                    }
                    retIndex += inputMatchStringItems[index].Length;
                }
                if (retIndex < targetString.Length)
                {
                    retString.Append(targetString.Substring(retIndex));
                }
                this.AssociatedObject.Text = retString.ToString();
            }
        }
    }
}
