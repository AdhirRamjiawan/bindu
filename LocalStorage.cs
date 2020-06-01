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
        void Load();
        LocalStorageData GetData();
    }

    public class LocalStorageData
    {
        public HashSet<String> PendingDownloadUrls {get;set;} = new HashSet<String>();
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

            try
            {
                _instance.Load();
            }
            catch(FileNotFoundException)
            {
                _instance.Persist();
            }

            return _instance;
        }

        public void Persist()
        {
            string jsonData = JsonSerializer.Serialize(_data, typeof(LocalStorageData));
            File.WriteAllText(_fileName, jsonData);
        }

        public LocalStorageData GetData() =>  _data;

        public void Load()
        {
            using (StreamReader stream = new StreamReader(File.OpenRead(_fileName)))
            {
                _data = (LocalStorageData)JsonSerializer
                    .Deserialize(stream.ReadToEnd(), typeof(LocalStorageData));
            }
        }

    }
}