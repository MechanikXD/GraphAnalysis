using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.LoadSystem
{
    public static class SaveManager
    {
        private const string KEYS_STORAGE_KEY = "Session Key List";
        private static HashSet<string> _sessionKeys;
        public static List<string> SessionList => _sessionKeys.ToList();

        public static void SerializeSessionKeys()
        {
            PlayerPrefs.SetString(KEYS_STORAGE_KEY, JsonConvert.SerializeObject(_sessionKeys));
        }

        public static void DeserializeSessionKeys()
        {
            _sessionKeys = PlayerPrefs.HasKey(KEYS_STORAGE_KEY)
                ? JsonConvert.DeserializeObject<HashSet<string>>(
                    PlayerPrefs.GetString(KEYS_STORAGE_KEY))
                : new HashSet<string>();
        }
        
        public static void StoreSession(string key, string json)
        {
            PlayerPrefs.SetString(key, json);
            _sessionKeys.Add(key);
        }

        public static void RenameSession(string oldKey, string newKey)
        {
            if (!HaveSession(oldKey)) return;

            var oldJson = PlayerPrefs.GetString(oldKey);
            DeleteSession(oldKey);
            StoreSession(newKey, oldJson);
        }

        public static string GetSession(string key) => HaveSession(key) ? PlayerPrefs.GetString(key) : string.Empty;

        public static bool HaveSession(string key) => PlayerPrefs.HasKey(key);

        public static void DeleteSession(string key)
        {
            PlayerPrefs.DeleteKey(key);
            _sessionKeys.Remove(key);
        }
    }
}