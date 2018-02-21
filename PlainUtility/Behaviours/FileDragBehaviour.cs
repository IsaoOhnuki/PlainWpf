using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Behaviours
{
    /// <summary>
    /// ファイルのＤ＆Ｄを受け入れるビヘイビア
    /// </summary>
    public class FileDragBehaviour
    {
        /// <summary>
        /// 複数ファイルドロップ可能添付プロパティのGetter
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True=複数可、False=一つのファイルのみ</returns>
        public static bool GetMultiSelect(DependencyObject obj)
        {
            return (bool)obj.GetValue(MultiSelectProperty);
        }

        /// <summary>
        /// 複数ファイルドロップ可能添付プロパティのSetter
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value">True=複数可、False=一つのファイルのみ</param>
        public static void SetMultiSelect(DependencyObject obj, bool value)
        {
            obj.SetValue(MultiSelectProperty, value);
        }

        /// <summary>
        /// 複数ファイルドロップ可能添付プロパティ
        /// </summary>
        public static readonly DependencyProperty MultiSelectProperty = DependencyProperty.RegisterAttached(
            "MultiSelect",
            typeof(bool),
            typeof(FileDragBehaviour),
            new PropertyMetadata(defaultValue: false));

        /// <summary>
        /// Ｄ＆Ｄ許可する拡張子をセットする添付プロパティのGetter
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>ドットからの拡張子、コロンセパレータで複数指定可、ワイルドカード指定可</returns>
        public static string GetDragFileExt(DependencyObject obj)
        {
            return (string)obj.GetValue(DragFileExtProperty);
        }

        /// <summary>
        /// Ｄ＆Ｄ許可する拡張子をセットする添付プロパティのSetter
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value">ドットからの拡張子、コロンセパレータで複数指定可、ワイルドカード指定可</param>
        public static void SetDragFileExt(DependencyObject obj, string value)
        {
            obj.SetValue(DragFileExtProperty, value);
        }

        /// <summary>
        /// Ｄ＆Ｄ許可する拡張子をセットする添付プロパティ
        /// </summary>
        public static readonly DependencyProperty DragFileExtProperty = DependencyProperty.RegisterAttached(
            "DragFileExt",
            typeof(string),
            typeof(FileDragBehaviour),
            new PropertyMetadata(defaultValue: "", propertyChangedCallback: (d, e) => {
                UIElement obj = d as UIElement;
                string oldExt = (string)e.OldValue;
                string newExt = (string)e.NewValue;
                if (obj != null)
                {
                    obj.AllowDrop = false;
                    obj.DragOver -= Obj_DragOver;
                    obj.Drop -= Obj_Drop;
                    if (Regex.IsMatch(newExt, FileExtRegex, RegexOptions.IgnoreCase))
                    {
                        obj.AllowDrop = true;
                        obj.DragOver += Obj_DragOver;
                        obj.Drop += Obj_Drop;
                    }
                }
            }));

        /// <summary>
        /// DragFileExtの検証用正規表現
        /// </summary>
        private static string FileExtRegex = @"^(?:(?:\.(?:[*a-zA-Z0-9]+))+:?)+$";

        /// <summary>
        /// ファイルがドロップされたときに呼び出されるコマンドの添付プロパティのGetter
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ICommand GetDropCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DropCommandProperty);
        }

        /// <summary>
        /// ファイルがドロップされたときに呼び出されるコマンドの添付プロパティのSetter
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetDropCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DropCommandProperty, value);
        }

        /// <summary>
        /// ファイルがドロップされたときに呼び出されるコマンドの添付プロパティ
        /// </summary>
        public static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached(
            "DropCommand",
            typeof(ICommand),
            typeof(FileDragBehaviour),
            new PropertyMetadata(defaultValue: null));

        /// <summary>
        /// ドロップ時のイベントハンドラ
        /// </summary>
        /// <param name="sender">ドロップ先のコントロール</param>
        /// <param name="e">ドロップパラメータ</param>
        private static void Obj_Drop(object sender, DragEventArgs e)
        {
            List<string> files = new List<string>();
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            bool ret = false;
            if (GetMultiSelect((DependencyObject)sender) || fileNames.Count() == 1)
            {
                foreach (var fileName in fileNames)
                {
                    string fileExt = fileName.Substring(fileName.IndexOf('.'));
                    string[] exts = GetDragFileExt((DependencyObject)sender).Replace(".", "\\.").Replace("*", "[^.]++").Split(new char[] { ':' });
                    foreach (var ext in exts)
                    {
                        ret = Regex.IsMatch(fileExt, ext, RegexOptions.IgnoreCase);
                        if (ret)
                        {
                            files.Add(fileName);
                            break;
                        }
                    }
                }
            }
            e.Effects = ret ? e.AllowedEffects & DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
            bool? canExecute = GetDropCommand((DependencyObject)sender)?.CanExecute(files.ToArray());
            if (!canExecute.HasValue || canExecute.Value)
            {
                GetDropCommand((DependencyObject)sender).Execute(files.ToArray());
            }
        }

        /// <summary>
        /// ドラッグ時のイベントハンドラ
        /// </summary>
        /// <param name="sender">ドラッグ先のコントロール</param>
        /// <param name="e">ドラッグパラメータ</param>
        private static void Obj_DragOver(object sender, DragEventArgs e)
        {
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            bool ret = false;
            if (GetMultiSelect((DependencyObject)sender) || fileNames.Count() == 1)
            {
                foreach (var fileName in fileNames)
                {
                    int extIndex = fileName.IndexOf('.');
                    if (extIndex >= 0)
                    {
                        string fileExt = fileName.Substring(extIndex);
                        string[] exts = GetDragFileExt((DependencyObject)sender).Replace(".", "\\.").Replace('?', '.').Replace("*", "[^.]+").Split(new char[] { ':' });
                        foreach (var ext in exts)
                        {
                            ret = Regex.IsMatch(fileExt, ext, RegexOptions.IgnoreCase);
                            if (ret)
                                break;
                        }
                        if (ret)
                            break;
                    }
                }
            }
            e.Effects = ret ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }
    }
}
