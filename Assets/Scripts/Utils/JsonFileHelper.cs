using System;
using System.IO;
using UnityEngine;

namespace BusinessEmpire.Utils
{
    /// <summary>
    /// JSON serialization utilities using Unity's JsonUtility with file I/O.
    /// </summary>
    public static class JsonFileHelper
    {
        public static bool Save<T>(T data, string fileName)
        {
            try
            {
                string path = GetPath(fileName);
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonFileHelper] Save failed: {ex.Message}");
                return false;
            }
        }

        public static bool TryLoad<T>(string fileName, out T data) where T : new()
        {
            data = new T();
            try
            {
                string path = GetPath(fileName);
                if (!File.Exists(path)) return false;
                string json = File.ReadAllText(path);
                data = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonFileHelper] Load failed: {ex.Message}");
                return false;
            }
        }

        public static string GetPath(string fileName) =>
            Path.Combine(Application.persistentDataPath, fileName);

        public static bool Exists(string fileName) =>
            File.Exists(GetPath(fileName));
    }
}
