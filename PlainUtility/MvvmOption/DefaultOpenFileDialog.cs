using Microsoft.Win32;
using Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Security;
using System.Text;
using System.Windows;

namespace MvvmOption
{
    public class OpenFileDialogMessage : IMessage
    {
        //
        // 概要:
        //     選択されたファイルごとに 1 つずつ安全なファイル名を格納する配列を取得します。
        //
        // 戻り値:
        //     選択されたファイルごとに 1 つずつ安全なファイル名を格納する System.String の配列。既定値は、値が System.String.Empty
        //     である 1 つの項目を持つ配列です。
        public string[] SafeFileNames { get; set; }
        //
        // 概要:
        //     選択されたファイルのファイル名のみを格納する文字列を取得します。
        //
        // 戻り値:
        //     選択されたファイルのファイル名のみを格納する System.String。既定値は System.String.Empty です。ファイルが選択されていない場合やディレクトリが選択された場合もこの値が使用されます。
        public string SafeFileName { get; set; }
        ////
        //// 概要:
        ////     コモン ダイアログ ボックスを表示します。
        ////
        //// 戻り値:
        ////     表示されているダイアログ ボックス (Microsoft.Win32.OpenFileDialog や Microsoft.Win32.SaveFileDialog
        ////     など) でユーザーが [OK] ボタンをクリックした場合は true を返します。それ以外の場合は、false を返します。
        //[SecurityCritical]
        //public virtual bool? ShowDialog();
        //
        // 概要:
        //     ダイアログが有効な Win32 ファイル名だけを受け入れるかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     無効なファイル名が入力されたときに警告を表示する場合は true。それ以外の場合は false。既定値は、false です。
        public bool ValidateNames { get; set; }
        //
        // 概要:
        //     ファイル ダイアログのタイトル バーに表示されるテキストを取得または設定します。
        //
        // 戻り値:
        //     ファイル ダイアログのタイトル バーに表示されるテキストである System.String。既定値は、System.String.Empty です。
        public string Title { get; set; }
        //
        // 概要:
        //     このプロパティは実装されていません。
        //
        // 戻り値:
        //     実装されていません。
        public bool RestoreDirectory { get; set; }
        //
        // 概要:
        //     ファイル ダイアログに表示される初期ディレクトリを取得または設定します。
        //
        // 戻り値:
        //     初期ディレクトリを格納している System.String。既定値は、System.String.Empty です。
        public string InitialDirectory { get; set; }
        //
        // 概要:
        //     ファイル ダイアログで現在選択されているフィルターのインデックスを取得または設定します。
        //
        // 戻り値:
        //     選択されたフィルターのインデックスである System.Int32。既定値は 1 です。
        public int FilterIndex { get; set; }
        //
        // 概要:
        //     Microsoft.Win32.OpenFileDialog または Microsoft.Win32.SaveFileDialog で表示されるファイルの種類を決定するフィルター文字列を取得または設定します。
        //
        // 戻り値:
        //     フィルターを格納している System.String。既定値は System.String.Empty です。これは、フィルターが適用されず、すべてのファイルの種類が表示されることを意味します。
        //
        // 例外:
        //   T:System.ArgumentException:
        //     The filter string is invalid.
        public string Filter { get; set; }
        //
        // 概要:
        //     選択されたファイルごとに 1 つずつファイル名を格納する配列を取得します。
        //
        // 戻り値:
        //     選択されたファイルごとに 1 つずつファイル名を格納する System.String の配列。既定値は、値が System.String.Empty である
        //     1 つの項目を持つ配列です。
        public string[] FileNames { get; set; }
        //
        // 概要:
        //     ファイル ダイアログで選択されたファイルの完全なパスを含む文字列を取得または設定します。
        //
        // 戻り値:
        //     ファイル ダイアログで選択されたファイルの完全なパスである System.String。既定値は、System.String.Empty です。
        public string FileName { get; set; }
        //
        // 概要:
        //     ファイル ダイアログ ボックスのカスタム プレースのリストを取得または設定します。
        //
        // 戻り値:
        //     カスタム プレースのリスト。
        public IList<FileDialogCustomPlace> CustomPlaces { get; set; }
        //
        // 概要:
        //     表示されるファイル リストにフィルターを適用するための既定の拡張子文字列を指定する値を取得または設定します。
        //
        // 戻り値:
        //     既定の拡張子文字列。既定値は、System.String.Empty です。
        public string DefaultExt { get; set; }
        //
        // 概要:
        //     ユーザーが無効なパスとファイル名を入力した場合に警告を表示するかどうかを指定する値を取得または設定します。
        //
        // 戻り値:
        //     警告を表示する場合は true。それ以外の場合は false。既定値は、true です。
        public bool CheckPathExists { get; set; }
        //
        // 概要:
        //     存在しないファイル名をユーザーが指定した場合に、ファイル ダイアログで警告を表示するかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     警告を表示する場合は true。それ以外の場合は false。この基本クラスの既定値は false です。
        public virtual bool CheckFileExists { get; set; }
        //
        // 概要:
        //     ユーザーが拡張子を省略した場合に、ファイル ダイアログで自動的にファイル名に拡張子を付けるかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     拡張子を付ける場合は true。それ以外の場合は false。既定値は、true です。
        public bool AddExtension { get; set; }
        //
        // 概要:
        //     ファイル ダイアログが、ショートカットで参照されたファイルの場所を返すか、ショートカット ファイル (.lnk) の場所を返すかを示す値を取得または設定します。
        //
        // 戻り値:
        //     参照先の場所を返す場合は true。ショートカットの場所を返す場合は false。既定値は、false です。
        public bool DereferenceLinks { get; set; }
        //
        // 概要:
        //     ファイル ダイアログで初期化のために使用される Win32 コモン ファイル ダイアログ フラグを取得します。
        //
        // 戻り値:
        //     ファイル ダイアログで初期化のために使用される Win32 コモン ファイル ダイアログ フラグを格納する System.Int32。
        protected int Options { get; }
        //
        // 概要:
        //     Microsoft.Win32.OpenFileDialog でユーザーが複数のファイルを選択できるかどうかを示すオプションを取得または設定します。
        //
        // 戻り値:
        //     複数のファイルを選択できる場合は true。それ以外の場合は false。既定値は、false です。
        public bool Multiselect { get; set; }
        //
        // 概要:
        //     Microsoft.Win32.OpenFileDialog で表示される読み取り専用チェック ボックスがオンかオフかを示す値を取得または設定します。
        //
        // 戻り値:
        //     チェック ボックスがオンの場合は true。それ以外の場合は false。既定値は、false です。
        public bool ReadOnlyChecked { get; set; }
        //
        // 概要:
        //     Microsoft.Win32.OpenFileDialog に読み取り専用チェック ボックスが表示されているかどうかを示す値を取得または設定します。
        //
        // 戻り値:
        //     チェック ボックスが表示されている場合は true。それ以外の場合は false。既定値は、false です。
        public bool ShowReadOnly { get; set; }

        public object Content { get; set; }

        public bool DialogResult { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DefaultOpenFileDialogRequestToIRequestTypeConverter : TypeConverter
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
    [TypeConverter(typeof(DefaultOpenFileDialogRequestToIRequestTypeConverter))]
    public class DefaultOpenFileDialogRequest : Request
    {
        public DefaultOpenFileDialogRequest()
            : base(typeof(OpenFileDialog), typeof(OpenFileDialogMessage), ActionHnadler)
        {

        }
        private static void ActionHnadler(IMessage message)
        {
            var msg = message as OpenFileDialogMessage;
            var dialog = new OpenFileDialog()
            {
                AddExtension = msg.AddExtension,
                CheckFileExists = msg.CheckFileExists,
                CheckPathExists = msg.CheckPathExists,
                CustomPlaces = msg.CustomPlaces,
                DefaultExt = msg.DefaultExt,
                DereferenceLinks = msg.DereferenceLinks,
                FileName = msg.FileName,
                Filter = msg.Filter,
                FilterIndex = msg.FilterIndex,
                InitialDirectory = msg.InitialDirectory,
                Multiselect = msg.Multiselect,
                ReadOnlyChecked = msg.ReadOnlyChecked,
                RestoreDirectory = msg.RestoreDirectory,
                ShowReadOnly = msg.ShowReadOnly,
                Title = msg.Title,
                ValidateNames = msg.ValidateNames,
            };

            var ret = dialog.ShowDialog(Application.Current.MainWindow);
            msg.DialogResult = ret.HasValue && ret.Value;
            if (msg.DialogResult)
            {
                msg.FileName = dialog.FileName;
                msg.FileNames = dialog.FileNames;
                msg.SafeFileName = dialog.SafeFileName;
                msg.SafeFileNames = dialog.SafeFileNames;
            }
        }
    }
}
