using Modules.Settings.Contracts.Models;

namespace Modules.Settings.Contracts;

public interface ISettingsRepository
{
    List<Setting> GetAllSettings();
    void AddSettings(IEnumerable<Setting> settings);
    void UpdateSettings(IEnumerable<Setting> settings);
}
