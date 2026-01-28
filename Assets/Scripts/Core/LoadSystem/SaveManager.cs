using UnityEngine;

namespace Core.LoadSystem
{
    public static class SaveManager
    {
        public static void StoreSession(string key, string json) => PlayerPrefs.SetString(key, json);

        public static void RenameSession(string oldKey, string newKey)
        {
            if (!HaveSession(oldKey)) return;

            var oldJson = PlayerPrefs.GetString(oldKey);
            DeleteSession(oldKey);
            StoreSession(newKey, oldJson);
        }

        public static string GetSession(string key) => HaveSession(key) ? PlayerPrefs.GetString(key) : string.Empty;

        public static bool HaveSession(string key) => PlayerPrefs.HasKey(key);

        public static void DeleteSession(string key) => PlayerPrefs.DeleteKey(key);
    }
}