using Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Utilitys
{
    /// <summary>
    /// ログタイプ
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 通常ログ
        /// </summary>
        Information,
        /// <summary>
        /// デバッグ用ログ
        /// </summary>
        /// <remarks>DEBUGシンボル指定時のみログされる</remarks>
        Debug,
        /// <summary>
        /// エラーログ
        /// </summary>
        Error,
    }
    /// <summary>
    /// ログクラス
    /// </summary>
    public static class Logger
    {
        private static string _logFormat = "{LGT}[{YY}-{MM}-{dd} {hh}:{mm}:{ss}:{ff}] '{MSG}'";
        private static string logFormat = _logFormat;
        /// <summary>
        /// ログ文字列の整形フォーマット
        /// </summary>
        /// <value>
        /// 整形フォーマット
        /// </value>
        /// <remarks>フォーマット文字列内の'{MSG}'をログメッセージに置換、フォーマット文字列内の'{LGT}'ログタイプに置換
        /// デフォルト値 "{LGT}[{YY}-{MM}-{dd} {hh}:{mm}:{ss}:{ff}] -> '{MSG}'"</remarks>
        /// <seealso cref="JapaneseDateTimeExtensions"/>
        public static string LogFormat
        {
            get { return string.IsNullOrWhiteSpace(logFormat) ? _logFormat : logFormat; }
            set
            {
                logFormat = value;
            }
        }
        /// <summary>
        /// ロギングを行うファイルパス
        /// </summary>
        /// <value>
        /// ログファイルパス
        /// </value>
        public static string LogFilePath { get; set; }
        /// <summary>
        /// ログファイルのエンコードタイプ
        /// </summary>
        /// <value>
        /// ファイルエンコードタイプ
        /// </value>
        public static Encoding FileEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Writes the specified log type.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        public static void Write(LogType logType, string message)
        {
#if DEBUG
            string format = LogFormat;
#else
            string format = logType == LogType.Debug ? "" : LogFormat;
#endif
            Write(format, logType == LogType.Information ? "I" : logType == LogType.Error ? "E" : logType == LogType.Debug ? "D" : "@", message);
        }
        /// <summary>
        /// Writes the specified log type.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        public static void Write(string logType, string message)
        {
            Write(LogFormat, logType, message);
        }
        /// <summary>
        /// Writes the specified log format.
        /// </summary>
        /// <param name="logFormat">The log format.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="message">The message.</param>
        public static void Write(string logFormat, string logType, string message)
        {
            string msg = DateTime.Now.ToFormatString(logFormat)?
                .Replace("{LGT}", logType)
                .Replace("{MSG}", message)
                .Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ').Trim()
                ;
            Write(msg);
        }
        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Write(string message)
        {
            Console.WriteLine(message);
            if (!string.IsNullOrWhiteSpace(LogFilePath))
            {
                if (File.Exists(LogFilePath))
                {
                    try
                    {
                        using (FileStream fileStream = File.Open(LogFilePath, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(fileStream, FileEncoding))
                            {
                                streamWriter.WriteLine(message);
                            }
                            fileStream.Close();
                        }
                    }
                    catch
                    {
                        Console.WriteLine("* error!! >> log file is not accessed. [class Logger. Write method] : " + LogFilePath);
                    }
                }
                else
                {
                    Console.WriteLine("* error!! >> log file is not found. [class Logger. Write method] : " + LogFilePath);
                }
            }
        }
        /// <summary>
        /// Writes the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void Write(Exception exception, string message = "")
        {
            string msg = message + exception.GetType().ToString() + " \"" + exception.Message + "\" " + (exception.InnerException == null ? "" : "(" + exception.InnerException.Message + ") ") + exception.StackTrace;
            msg.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Trim();
            Write(LogType.Error, msg);
        }
        /// <summary>
        /// Reflashes this instance.
        /// </summary>
        public static void Reflash()
        {

        }
    }
}
