using System.Threading.Tasks;

namespace SettingsDb
{
    public interface ISettingsDb
    {
        void Store<T>(string settingName, T value);
        
        Task StoreAsync<T>(string settingName, T value);

        T Read<T>(string settingName, T defaultValue = default);

        Task<T> ReadAsync<T>(string settingName, T defaultValue = default);

        void Clear(string settingName);

        Task ClearAsync(string settingName);

        void ClearAll();
        
        Task ClearAllAsync();
        
        long Count();
        
        Task<long> CountAsync();
    }
}