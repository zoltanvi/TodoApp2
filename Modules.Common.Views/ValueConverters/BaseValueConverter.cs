using System.Globalization;
using System.Windows.Data;

namespace Modules.Common.Views.ValueConverters;

/// <summary>
/// A base value converter that allows setting properties on it
/// </summary>
public abstract class BaseValueConverter : IValueConverter
{
    /// <summary>
    /// The method to convert one type to another
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    /// <summary>
    /// The method to convert a value back to it's source type
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
