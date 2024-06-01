using Modules.Common.ViewModel;
using System.Collections.Generic;

namespace TodoApp2.Core;

public class DateTimeSettingsPageViewModel : BaseViewModel
{
    private List<string> _dateFormats = new List<string>()
    {
        "dddd, MMMM dd, yyyy h:mm:ss tt",
        "dddd, d MMMM yyyy HH:mm:ss",
        "dddd, d. MMMM yyyy HH:mm:ss",
        "yyyy MMMM d. dddd, HH:mm:ss",
        "dddd, dd MMMM yyyy h:mm:ss tt",


        "dddd, MMMM dd, yyyy",
        "dddd, d MMMM yyyy",
        "dddd, d. MMMM yyyy",
        "yyyy MMMM d. dddd",
        "dddd, dd MMMM yyyy",


        "MM/dd/yyyy HH:mm:ss",
        "dd/MM/yyyy HH:mm:ss",
        "dd.MM.yyyy HH:mm:ss",
        "yyyy/MM/dd HH:mm:ss",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy.MM.dd. HH:mm:ss",
        "dd-MM-yyyy HH:mm:ss",


        "MM/dd/yyyy HH:mm",
        "dd/MM/yyyy HH:mm",
        "dd.MM.yyyy HH:mm",
        "yyyy/MM/dd HH:mm",
        "yyyy-MM-dd HH:mm",
        "yyyy.MM.dd. HH:mm",
        "dd-MM-yyyy HH:mm",


        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "dd.MM.yyyy",
        "yyyy/MM/dd",
        "yyyy.MM.dd.",
        "dd-MM-yyyy",


        "h:mm:ss tt",
        "HH:mm:ss",
        "H:mm:ss",
        "h:mm:ss tt",


        "h:mm tt",
        "HH:mm",
        "H:mm",
        "h:mm tt",
    };

    public List<string> TitleBarDateFormats => _dateFormats;
    public List<string> ReminderDateFormats => _dateFormats;
    public List<string> TaskCreationDateFormats => _dateFormats;
}
