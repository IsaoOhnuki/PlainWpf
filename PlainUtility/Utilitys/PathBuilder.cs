using Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilitys
{
    /// <summary>
    /// パス文字列校正クラス
    /// <br/>
    /// </summary>
    public class PathBuilder : NotifyBase
    {
        private static readonly char[] invalidPathChars = Path.GetInvalidPathChars();
        private static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        private static readonly string FullPathMatchString = @"^([c-zC-Z]:\\)?(([^\\:" + string.Join("", invalidPathChars).Replace("\\", "\\\\").Replace("]", "\\]") + @"]+|\.{1,2})\\)*[^:" + string.Join("", invalidFileNameChars).Replace("\\", "\\\\").Replace("]", "\\]") + @"]*$";
        private static readonly string FilePathMatchString = @"^([c-zC-Z]:\\)?(([^\\:" + string.Join("", invalidPathChars).Replace("\\", "\\\\").Replace("]", "\\]") + @"]+|\.{1,2})\\)*$";
        private static readonly string FileNameMatchString = @"^[^:" + string.Join("", invalidFileNameChars).Replace("\\", "\\\\").Replace("]", "\\]") + @"]+$";
        private bool pathAnalyzing;
        private string fullPath = "";
        /// <summary>
        /// フルパスプロパティ
        /// <br/>
        /// ￥終端はルートまたはディレクトリ
        /// <br/>
        /// それ以外はファイル名またはパス付ファイル名
        /// <br/>
        /// </summary>
        public string FullPath
        {
            get { return fullPath; }
            set
            {
                if (!pathAnalyzing)
                {
                    pathAnalyzing = true;
                    var val = value ?? "";
                    if (fullPath != val)
                    {
                        int separatorIndex = val.LastIndexOf(Path.DirectorySeparatorChar);
                        FilePath = separatorIndex >=0 ? val.Substring(0, separatorIndex + 1) : "";
                        string path = IsFilePathVerified ? FilePath : "";
                        FileName = separatorIndex >= 0 ? val.Substring(separatorIndex + 1) : val;
                        string name = IsFileNameVerified ? fileName : "";
                        IsFullPathVerified = (IsFileNameVerified || IsFilePathVerified) && Regex.IsMatch(val, FullPathMatchString);
                        if (IsFullPathVerified)
                        {
                            if (path != "" && name != "")
                            {
                                SetProperty(ref fullPath, path + name);
                            }
                            else if (path == "" && name == "")
                            {
                                SetProperty(ref fullPath, "");
                            }
                            else
                            {
                                SetProperty(ref fullPath, name == "" ? path : name);
                            }
                        }
                        else
                        {
                            SetProperty(ref fullPath, val);
                        }
                    }
                    pathAnalyzing = false;
                    OnPropertyChanged(nameof(IsFullPathVerified));
                }
            }
        }

        /// <summary>
        /// FullPathプロパティの検証結果
        /// </summary>
        public bool IsFullPathVerified { get; private set; }
        /// <summary>
        /// ファイルの存在、アクセス権の確認
        /// </summary>
        public bool IsExists { get { return IsFullPathVerified && File.Exists(fullPath); } }

        private string filePath = "";
        /// <summary>
        /// ファイルパスプロパティ
        /// <br/>
        /// 必ず￥終端
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set
            {
                var val = value ?? "";
                IsFilePathVerified = val.Length > 0 && !val.Any(x => invalidPathChars.Contains(x)) && Regex.IsMatch(val, FilePathMatchString);
                if (filePath != val)
                {
                    SetProperty(ref filePath, val);
                    if (IsFilePathVerified || val.Length == 0)
                    {
                        if (val == "")
                        {
                            FullPath = IsFileNameVerified ? FileName : "";
                        }
                        else
                        {
                            FullPath = val + (IsFileNameVerified ? FileName : "");
                        }
                    }
                }
                OnPropertyChanged(nameof(IsFilePathVerified));
            }
        }

        /// <summary>
        /// FilePathプロパティの検証結果
        /// </summary>
        public bool IsFilePathVerified { get; private set; }

        private string fileName = "";
        /// <summary>
        /// ファイル名プロパティ
        /// <br/>
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set
            {
                var val = value ?? "";
                IsFileNameVerified = val.Length > 0 && !val.Any(x => invalidFileNameChars.Contains(x)) && Regex.IsMatch(val, FileNameMatchString);
                if (fileName != val)
                {
                    SetProperty(ref fileName, val);
                    if (IsFileNameVerified || val.Length == 0)
                    {
                        if (val == "")
                        {
                            FullPath = IsFilePathVerified ? FilePath : "";
                        }
                        else
                        {
                            FullPath = IsFilePathVerified ? FilePath + val : val;
                        }
                    }
                    OnPropertyChanged(nameof(Extension));
                }
                OnPropertyChanged(nameof(IsFileNameVerified));
            }
        }

        /// <summary>
        /// 拡張子の取り出し、変更を行います
        /// 拡張子があれば取り出しは必ず.からの拡張子
        /// </summary>
        public string Extension
        {
            get { return Path.GetExtension(FileName); }
            set { FileName = Path.ChangeExtension(FileName, value); }
        }

        /// <summary>
        /// FileNameプロパティの検証結果
        /// </summary>
        public bool IsFileNameVerified { get; private set; }

        /// <summary>
        /// PathBuilderコンストラクタ
        /// </summary>
        public PathBuilder()
        {
        }

        /// <summary>
        /// フルパス指定コンストラクタ
        /// </summary>
        /// <param name="fullPath"><see cref="FullPath"/></param>
        public PathBuilder(string fullPath)
        {
            FullPath = fullPath;
        }
    }
}
