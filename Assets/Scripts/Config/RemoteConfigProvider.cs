using UnityEngine;
using Doulingo.Core;
using System.IO;

namespace Doulingo.Config
{
    public class RemoteConfigProvider : MonoBehaviour, IConfigProvider
    {
        private GameConfig gameConfig;
        private string configPath;

        private void Awake()
        {
            configPath = Path.Combine(Application.streamingAssetsPath, "RemoteConfig.json");
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (File.Exists(configPath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(configPath);
                    gameConfig = JsonUtility.FromJson<GameConfig>(jsonContent);
                    Debug.Log("RemoteConfig loaded successfully");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load RemoteConfig: {e.Message}");
                    CreateDefaultConfig();
                }
            }
            else
            {
                CreateDefaultConfig();
            }
        }

        private void CreateDefaultConfig()
        {
            gameConfig = new GameConfig();
            SaveConfig();
        }

        private void SaveConfig()
        {
            try
            {
                string jsonContent = JsonUtility.ToJson(gameConfig, true);
                File.WriteAllText(configPath, jsonContent);
                Debug.Log("Default config saved to RemoteConfig.json");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save config: {e.Message}");
            }
        }

        public GameConfig GetGameConfig()
        {
            return gameConfig;
        }

        public void ReloadConfig()
        {
            LoadConfig();
        }
    }
}
