using System;
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
        
        private const string SETTINGS_KEY = "Settings";
        private static Dictionary<string, object> _settings;
        private static bool _settingsLoaded;
        
        public static T GetSetting<T>(string key)
        {
            if (!_settingsLoaded) LoadSettings();
            var value = _settings[key];
            // funny cast because Newton serializes floats as double, so we have to cast back to float and preserve generic.
            return typeof(T) == typeof(float) ? (T)(object)Convert.ToSingle(value) : (T)value;
        }

        public static bool HasSetting(string key)
        {
            if (!_settingsLoaded) LoadSettings();
            return _settings.ContainsKey(key);
        }

        public static void SetSetting<T>(string key, T value)
        {
            if (!_settingsLoaded) LoadSettings();
            if (!_settings.TryAdd(key, value))
            {
                _settings[key] = value;
            }
        }
        
        private static void LoadSettings()
        {
            if (!PlayerPrefs.HasKey(SETTINGS_KEY))
            {
                _settings = new Dictionary<string, object>();
                _settingsLoaded = true;
                return;
            } 
            
            var json = PlayerPrefs.GetString(SETTINGS_KEY);
            _settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            _settingsLoaded = true;
        }
        
        public static void SaveSettings()
        {
            if (_settings == null) return;
            var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            PlayerPrefs.SetString(SETTINGS_KEY, json);
        }

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