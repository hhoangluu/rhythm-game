using UnityEngine;
using UnityEngine.UI;
using Doulingo.Core;
using Doulingo.Gameplay;
using TMPro;
using System;

namespace Doulingo.UI
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private Button level1Button;
        [SerializeField] private Button level2Button;

        private LevelConfig config;
        private GameManager gameManager;

        void Start()
        {
            gameManager = GameManager.Instance;

            level1Button.onClick.AddListener(() => SelectLevel(1));
            level2Button.onClick.AddListener(() => SelectLevel(2));
        }

        void SelectLevel(int level)
        {
            config = Resources.Load<LevelConfig>($"LevelConfigs/LevelConfig{level}");
            StartGame();
        }

        void StartGame()
        {
            if (config != null)
            {
                gameManager.SetLevelConfig(config);
                gameManager.InitializeGame();
                gameManager.StartGame();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
