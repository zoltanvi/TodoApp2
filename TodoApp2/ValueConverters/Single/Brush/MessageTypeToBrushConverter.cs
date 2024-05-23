using Modules.Common.DataModels;
using System;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2;

internal class MessageTypeToBrushConverter : BaseValueConverter
{
    private TagColorConverter _tagColorConverter;
    public ColorType ColorType { get; set; }

    public MessageTypeToBrushConverter()
    {
        _tagColorConverter = new TagColorConverter();
    }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is MessageType messageType)
        {
            _tagColorConverter.ColorType = ColorType;
            switch (messageType)
            {
                case MessageType.Warning:
                    return _tagColorConverter.Convert(TagPresetColor.Lime);
                case MessageType.Info:
                    return _tagColorConverter.Convert(TagPresetColor.Geekblue);
                case MessageType.Error:
                    return _tagColorConverter.Convert(TagPresetColor.Magenta);
                case MessageType.Success:
                    return _tagColorConverter.Convert(TagPresetColor.Green);
            }
        }

        return null;
    }
}
