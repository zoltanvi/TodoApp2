using System.Collections.Generic;

namespace TodoApp2.Core;

public class ThemeChangeNotifier
{
    private List<KeyValuePair<IPropertyChangeNotifier, string>> _recipients;
    public ThemeChangeNotifier()
    {
        _recipients = new List<KeyValuePair<IPropertyChangeNotifier, string>>();
        Mediator.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);
    }

    public void AddRecipient(IPropertyChangeNotifier recipient, string propertyName)
    {
        _recipients.Add(new KeyValuePair<IPropertyChangeNotifier, string>(recipient, propertyName));
    }

    private void OnThemeChanged(object obj)
    {
        foreach (var recipient in _recipients)
        {
            recipient.Key?.OnPropertyChanged(recipient.Value);
        }
    }
}
