using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Converters
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <i>left double</i>&lt;|=&lt;|&lt;=|=&lt;=<i>right double</i>
    /// <br/>
    /// <b>&lt;</b> (<i>left double</i> == null ? true : <i>left double</i> &lt; value) &amp;&amp; (<i>right double</i> == null ? true : value &lt; <i>right double</i>)
    /// <br/>
    /// <b>=&lt;</b> (<i>left double</i> == null ? true : <i>left double</i> &lt;= value) &amp;&amp; (<i>right double</i> == null ? true : value &lt; <i>right double</i>)
    /// <br/>
    /// <b>&lt;=</b> (<i>left double</i> == null ? true : <i>left double</i> &lt; value) &amp;&amp; (<i>right double</i> == null ? true : value &lt;= <i>right double</i>)
    /// <br/>
    /// <b>=&lt;=</b> (<i>left double</i> == null ? true : <i>left double</i> &lt;= value) &amp;&amp; (<i>right double</i> == null ? true : value &lt;= <i>right double</i>)
    /// <br/>
    /// '<b>&lt;</b>' is hassle at xaml. replaces at '<b>@</b>'.
    /// </remarks>
    [ValueConversion(typeof(double), typeof(bool))]
    public class DoubleComparisonToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                var val = (double)value;
                var param = (string)parameter;
                var matches = Regex.Matches(param, @"^\{(\d*)(=?)[<@](=?)(\d*)\}$");
                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    bool paraL = double.TryParse(match.Groups[1].Value, out double first);
                    bool paraR = double.TryParse(match.Groups[4].Value, out double second);
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
