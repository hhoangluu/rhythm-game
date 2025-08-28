using UnityEngine;
using Doulingo.Core;
using System;
using TMPro;
using DG.Tweening;

namespace Doulingo.Gameplay
{
    public class NoteObject : MonoBehaviour
    {
        private NoteData noteData;

        // Events
        public event Action<NoteObject> OnNoteHit;
        public event Action<NoteObject> OnNoteMiss;
        [SerializeField]
        TMP_Text nameText;
        [SerializeField]
        SpriteRenderer spriteRenderer;
        [SerializeField] SpriteRenderer glowObject;
        // Properties
        public NoteData NoteData => noteData;
        bool isHitting;

        Tween tweenHitting;

        public void Initialize(NoteData data)
        {
            isHitting = false;
            noteData = data;
            nameText.text = PianoNoteHelper.GetNoteName(noteData.pianoKeyIndex);
            glowObject.transform.parent.localScale = new Vector3(1, glowObject.transform.parent.localScale.y);
            GameManager.OnKeyUp -= OnKeyUp;
            GameManager.OnKeyUp += OnKeyUp;
            ServiceHub.Conductor.OnBPMChanged -= OnBPMChanged;
            ServiceHub.Conductor.OnBPMChanged += OnBPMChanged;
        }

        private void OnBPMChanged(float bpm)
        {
            if (isHitting)
            {
                tweenHitting?.Kill();
                HitAnimation();
            }
        }

        private void OnKeyUp(int pianoKey, float holdTime)
        {
            if (pianoKey == noteData.pianoKeyIndex && isHitting)
            {
                Skip();
            }
        }

        public void SetColor(Color color)
        {
            glowObject.color = color;
            // Make the spriteRenderer color a bit darker by multiplying RGB by 0.7f
            spriteRenderer.color = new Color(color.r * 0.7f, color.g * 0.7f, color.b * 0.7f, 155f / 255f);

        }

        public void Hit()
        {
            OnNoteHit?.Invoke(this);
            isHitting = true;
            HitAnimation();
        }

        public void Miss()
        {
            OnNoteMiss?.Invoke(this);
            Skip();
        }

        void HitAnimation()
        {

            var conductor = ServiceHub.Conductor;
            float widthWorld = glowObject.bounds.size.x;
            var speed = GlobalSetting.UnitsPerBeatBase * conductor.CurrentBPM / 60;
            var beatOffset = conductor.SongPositionInBeats - noteData.beat;
            float delay = 0;
            if (beatOffset > 0) // pass the note
            {
                var a = (widthWorld - beatOffset * GlobalSetting.UnitsPerBeatBase) / widthWorld;
                glowObject.transform.parent.localScale = new Vector3(glowObject.transform.parent.localScale.x * a, glowObject.transform.parent.localScale.y);
                widthWorld -= beatOffset * GlobalSetting.UnitsPerBeatBase;
            }
            else
            {
                delay = (-beatOffset * GlobalSetting.UnitsPerBeatBase) / speed;
            }
            float duration = widthWorld / Mathf.Max(0.0001f, speed);
            tweenHitting = glowObject.transform.parent.DOScaleX(0, duration).SetEase(Ease.Linear).SetDelay(delay).OnComplete(() => Skip());
        }


        private void OnDestroy()
        {
            // Clean up event subscriptions
            OnNoteHit = null;
            OnNoteMiss = null;
        }

        internal void Skip()
        {
            tweenHitting?.Kill();
            isHitting = false;
            glowObject.transform.parent.localScale = new Vector3(0, glowObject.transform.parent.localScale.y);
        }
    }
}
