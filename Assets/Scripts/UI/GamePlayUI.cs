using UnityEngine;
using UnityEngine.UI;
using Doulingo.Core;
using Doulingo.Gameplay;
using TMPro;
using System;
using DG.Tweening;

namespace Doulingo.UI
{
    public class GamePlayUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI judgmentText;
        [SerializeField] private TextMeshProUGUI modeText;
        [SerializeField] private TextMeshProUGUI streakText;
        [SerializeField] private TextMeshProUGUI bpmText;


        [Header("Judgment Animation")]
        [SerializeField] private float animationDuration = 1.5f;
        [SerializeField] private float moveUpDistance = 30f;
        [SerializeField] private float fadeOutDelay = 0.5f;

        private IScoringService scoringService;
        private Vector3 startPosJudgment;
        private Sequence currentJudgmentTween; // Track current animation

        private void Start()
        {
            // Subscribe to GameManager's note judged event
            GameManager.OnNoteJudged += HandleNoteJudged;
            GameManager.OnGameStart += OnGameStart;
            GameManager.OnSongEnd += OnSongEnd;
            startPosJudgment = judgmentText.transform.localPosition;
            Debug.Log("[GamePlayUI] Subscribed to GameManager's OnNoteJudged event");
        }

        private void OnSongEnd()
        {
            ServiceHub.Conductor.OnBPMChanged -= OnBPMChanged;
        }

        private void OnComboChanged(int combo)
        {
            streakText.text = $"Streak: {combo}";
        }

        private void OnGameStart()
        {
            modeText.text = GameManager.Instance.LevelConfig.LevelName;
            if (scoringService != null)
                scoringService.OnComboChanged -= OnComboChanged;
            scoringService = ServiceHub.ScoringService;
            scoringService.OnComboChanged += OnComboChanged;

            ServiceHub.Conductor.OnBPMChanged += OnBPMChanged;
            OnBPMChanged(ServiceHub.Conductor.CurrentBPM);
        }

        private void OnBPMChanged(float bpm)
        {
            bpmText.text = $"BPM: {bpm}";
        }

        void OnEnable()
        {
            OnGameStart();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            GameManager.OnNoteJudged -= HandleNoteJudged;
            GameManager.OnGameStart -= OnGameStart;
            GameManager.OnSongEnd -= OnSongEnd;

            // Kill any active tween to prevent memory leaks
            if (currentJudgmentTween != null && currentJudgmentTween.IsActive())
            {
                currentJudgmentTween.Kill();
            }
        }

        private void HandleNoteJudged(NoteData note, HitResult result)
        {
            // Kill any existing animation
            if (currentJudgmentTween != null && currentJudgmentTween.IsActive())
            {
                currentJudgmentTween.Kill();
            }

            judgmentText.gameObject.SetActive(true);
            // Set text and color
            judgmentText.text = result.ToString().ToUpper();
            judgmentText.color = GetJudgmentColor(result);

            // Reset position and alpha for animation
            judgmentText.transform.localPosition = startPosJudgment;
            judgmentText.color = new Color(judgmentText.color.r, judgmentText.color.g, judgmentText.color.b, 1f);

            // Create smooth animation sequence
            currentJudgmentTween = DOTween.Sequence();

            // Move up and fade out
            currentJudgmentTween.Append(judgmentText.transform.DOLocalMoveY(moveUpDistance + startPosJudgment.y, animationDuration).SetEase(Ease.OutQuad));
            currentJudgmentTween.Join(judgmentText.DOFade(0f, animationDuration).SetDelay(fadeOutDelay));

            // On complete, reset position
            currentJudgmentTween.OnComplete(() =>
            {
                judgmentText.transform.localPosition = startPosJudgment;
                judgmentText.color = new Color(judgmentText.color.r, judgmentText.color.g, judgmentText.color.b, 1f);
                currentJudgmentTween = null; // Clear reference
                judgmentText.gameObject.SetActive(false);
            });

            Debug.Log($"[GamePlayUI] Judgment displayed: {result} with animation");
        }

        private Color GetJudgmentColor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                    return Color.yellow;
                case HitResult.Great:
                    return Color.green;
                case HitResult.Good:
                    return Color.blue;
                case HitResult.Miss:
                    return Color.red;
                default:
                    return Color.white;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
