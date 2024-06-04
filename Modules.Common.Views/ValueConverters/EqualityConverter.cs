using System.Globalization;

namespace Modules.Common.Views.ValueConverters;

public class EqualityConverter : BaseMultiValueConverter<EqualityConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.Length < 2 ? false : (object)values[0].Equals(values[1]);
    }
}
