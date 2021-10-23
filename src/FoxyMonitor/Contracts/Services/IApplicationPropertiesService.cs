namespace FoxyMonitor.Contracts.Services
{
    public interface IApplicationPropertiesService
    {
        bool Contains(string key);
        T GetProperty<T>(string propertyName);

        object GetProperty(string propertyName);

        void SetProperty(string key, object value);

        void RestoreData();

        void PersistData();
    }
}
