using Modules.Common.Views.ValueConverters;
using System;
using System.Globalization;
using System.Linq;

namespace TodoApp2;

/// <summary>
/// Used to pass a single object to the command handler method (Move to category)
/// </summary>
public class TaskContextMenuCommandParameterConverter : BaseMultiValueConverter<TaskContextMenuCommandParameterConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.ToList();
    }
}
