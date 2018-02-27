using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Converters
{
    /// <summary>
    /// 数値を比較して一致以外でもTrueとなる数値&lt;-&gt;Booleanコンバーター
    /// <br/>
    /// xamlで Converter=IntegerComparisonToBooleanConverter, ConverterParameter={}{100=&lt;=200} のようにConverterParameterで指定する。
    /// </summary>
    /// <remarks>
    /// 左値、右値どちらかがない場合、そちら側の比較はTrueとなる。どちらもない場合はFalse。
    /// <br/>
    /// また、左値&lt;右値を期待した動作となる。
    /// <br/>
    /// 構文
    /// <br/>
    /// <i>left int</i>&lt;|=&lt;|&lt;=|=&lt;=<i>right int</i>
    /// <br/>
    /// &lt; (<i>left int</i> == null ? true : <i>left int</i> &lt; value) &amp;&amp; (<i>right int</i> == null ? true : value &lt; <i>right int</i>)
    /// <br/>
    /// =&lt; (<i>left int</i> == null ? true : <i>left int</i> &lt;= value) &amp;&amp; (<i>right int</i> == null ? true : value &lt; <i>right int</i>)
    /// <br/>
    /// &lt;= (<i>left int</i> == null ? true : <i>left int</i> &lt; value) &amp;&amp; (<i>right int</i> == null ? true : value &lt;= <i>right int</i>)
    /// <br/>
    /// =&lt;= (<i>left int</i> == null ? true : <i>left int</i> &lt;= value) &amp;&amp; (<i>right int</i> == null ? true : value &lt;= <i>right int</i>)
    /// <br/>
    /// '&lt;'はxamlでの指定が面倒なので'@'でも可能。
    /// <br/>
    /// ConvertBackはnullを返す。
    /// </remarks>
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntegerComparisonToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                var val = (int)value;
                var param = (string)parameter;
                var matches = Regex.Matches(param, @"^\{(\d*)(=?)[<@](=?)(\d*)\}$");
                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    bool paraL = int.TryParse(match.Groups[1].Value, out int first);
                    bool paraR = int.TryParse(match.Groups[4].Value, out int second);
                    bool preEqual = !string.IsNullOrEmpty(match.Groups[2].Value);
                    bool postEqual = !string.IsNullOrEmpty(match.Groups[3].Value);
                    if (paraL && !paraR)
                    {
                        if (preEqual)
                            return first <= val;
                        else
                            return first < val;
                    }
                    else if (!paraL && paraR)
                    {
                        if (postEqual)
                            return val <= second;
                        else
                            return val < second;
                    }
                    else
                    {
                        if (preEqual && postEqual)
                            return first <= val && val <= second;
                        else if (preEqual && !postEqual)
                            return first <= val && val < second;
                        else if (!preEqual && postEqual)
                            return first < val && val <= second;
                        else
                            return first < val && val < second;
                    }
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
