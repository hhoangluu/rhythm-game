using UnityEngine;
using Doulingo.UI;
using Doulingo.Gameplay;
using System;

namespace Doulingo.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GamePlayUI gamePlayUI;
        [SerializeField] private LevelSelector levelSelector;

        private void Start()
        {
            // Subscribe to game start event
            GameManager.OnGameStart += OnGameStart;
            GameManager.OnSongEnd += OnSongEnd;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            GameManager.OnGameStart -= OnGameStart;
            GameManager.OnSongEnd -= OnSongEnd;
        }

        private void OnSongEnd()
        {
            gamePlayUI.Hide();
            levelSelector.Show();
        }

        private void OnGameStart()
        {
            // Enable GamePlayUI when game starts
            if (gamePlayUI != null)
            {
                gamePlayUI.Show();
                levelSelector.Hide();
                Debug.Log("[GameUI] GamePlayUI enabled");
            }
            else
            {
                Debug.LogError("[GameUI] GamePlayUI reference not assigned!");
            }
        }
    }
}
