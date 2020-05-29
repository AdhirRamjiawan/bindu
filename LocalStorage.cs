using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace bindu
{
    public interface ILocalStorage
    {
        void Persist();
        LocalStorageData Load();
    }

    public class LocalStorageData
    {
        public List<String> PendingDownloadUrls {get;set;} = new List<String>();
    }

    public class LocalStorage : ILocalStorage
    {
        private const string _fileName = "bindu.storage";

        private static LocalStorage _instance;
        private LocalStorageData _data;

        private LocalStorage()
        {
            _data = new LocalStorageData();
        }

        public static LocalStorage GetInstance()
        {
            if (_instance == null)
                _instance = new LocalStorage();

            return _instance;
        }

        public void Persist()
        {
            using (StreamWriter stream = new StreamWriter(File.OpenWrite(_fileName)))
            {
                string jsonData = JsonSerializer.Serialize(_data, typeof(LocalStorageData));
                stream.WriteLine(jsonData);
            }
        }

        public LocalStorageData Load()
        {
            using (StreamReader stream = new StreamReader(File.OpenRead(_fileName)))
            {
                _data = (LocalStorageData)JsonSerializer
                    .Deserialize(stream.ReadToEnd(), typeof(LocalStorageData));
            }

            return _data;
        }

    }
}