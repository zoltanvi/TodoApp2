using System;
using System.Collections.Generic;
using System.Globalization;
using TodoApp2.Core;
using TodoApp2.Core.Mappings;

namespace TodoApp2;

public class IdToCategoryNameConverter : BaseValueConverter
{
    private TimerService _timerService;
    private Guid _timer;
    private readonly Dictionary<int, string> _idNamePairCache;

    public IdToCategoryNameConverter()
    {
        _timerService = TimerService.Instance;
        _timer = _timerService.CreateTimer(OnClearCache);
        _idNamePairCache = new Dictionary<int, string>();
    }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int id)
        {
            _timerService.RestartTimer(_timer);

            if (_idNamePairCache.TryGetValue(id, out string name))
            {
                return name;
            }
            else
            {
                CategoryViewModel category = IoC.Context.Categories.First(x => x.Id == id).Map();
                if (category != null && !string.IsNullOrEmpty(category.Name))
                {
                    _idNamePairCache.Add(id, category.Name);
                    return category.Name;
                }
                else
                {
                    return "CATEGORY NOT FOUND";
                }
            }
        }
        else
        {
            return "CATEGORY CONVERTER ERROR!";
        }
    }

    private void OnClearCache(object sender, EventArgs e)
    {
        _idNamePairCache.Clear();
    }
}
