using FoxyMonitor.Contracts.Services;
using FoxyMonitor.Models;
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.IO;

namespace FoxyMonitor.Services
{
    public class ApplicationPropertiesService : ObservableObject, IApplicationPropertiesService
    {
        private readonly IFileService _fileService;
        private readonly AppConfig _appConfig;
        private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static IDictionary AppProperties { get => App.Current.Properties; }

        public ApplicationPropertiesService(IFileService fileService, IOptions<AppConfig> appConfig)
        {
            _fileService = fileService;
            _appConfig = appConfig.Value;
        }

        public bool Contains(string key) => App.Current.Properties.Contains(key);

        public T GetProperty<T>(string propertyName) => (T)App.Current.Properties[propertyName];

        public object GetProperty(string propertyName) => App.Current.Properties[propertyName];

        public void PersistData()
        {
            if (App.Current.Properties != null)
            {
                var folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
                var fileName = _appConfig.AppPropertiesFileName;
                _fileService.Save(folderPath, fileName, App.Current.Properties);
            }
        }

        public void RestoreData()
        {
            var folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
            var fileName = _appConfig.AppPropertiesFileName;
            var properties = _fileService.Read<IDictionary>(folderPath, fileName);
            if (properties != null)
            {
                foreach (DictionaryEntry property in properties)
                {
                    App.Current.Properties.Add(property.Key, property.Value);
                }
            }
        }

        public void SetProperty(string key, object value)
        {
            OnPropertyChanging(nameof(AppProperties));
            OnPropertyChanging(key);
            App.Current.Properties[key] = value;
            OnPropertyChanged(key);
            OnPropertyChanged(nameof(AppProperties));

            PersistData();
        }
    }
}
