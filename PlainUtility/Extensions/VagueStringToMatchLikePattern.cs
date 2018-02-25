using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    /// <summary>
    /// 正規表現パターン用String拡張関数
    /// </summary>
    public static class RegexStringExtensions
    {
        /// <summary>
        /// プレーン文字列をマッチパターンに使えるように、含まれているメタ文字をエスケープする
        /// </summary>
        /// <param name="vagueString">The vague.</param>
        /// <returns>マッチパターンに指定可能なプレーン文字列</returns>
        public static string MatchPossibleStringToUnMatchString(this string vagueString)
        {
            return string.Join("", vagueString.Select<char, string>(x => {
                switch (x)
                {
                    case '\\':
                        return @"\\";
                    case '*':
                        return @"\*";
                    case '+':
                        return @"\+";
                    case '?':
                        return @"\?";
                    case '.':
                        return @"\.";
                    case '{':
                        return @"\{";
                    case '(':
                        return @"\(";
                    case '[':
                        return @"\[";
                    case '^':
                        return @"\^";
                    case '$':
                        return @"\$";
                    case '|':
                        return @"\|";
                    default:
                        return new string(x, 1);
                }
            }));
        }
    }
}
