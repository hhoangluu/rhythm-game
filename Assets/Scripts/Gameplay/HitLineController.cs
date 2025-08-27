using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doulingo.Core;
using UnityEngine;
namespace Doulingo.Gameplay
{
    public class HitLineController : MonoBehaviour
    {
        [SerializeField] GameObject hitLine;
        [SerializeField] NoteOnHitLineEffect hitNoteEffect;
        float originHeight;
        Vector3 originPosition;
        Tween tween;
        void Awake()
        {
            originHeight = hitLine.transform.localScale.y;
            originPosition = transform.position;
        }
        PianoNote showingNote;
        public void ShowKey(PianoNote pianoNote, NoteData noteData)
        {
            hitNoteEffect.Show(pianoNote, noteData);
            showingNote = pianoNote;
            tween = hitLine.transform.DOScaleY(0.8f * originHeight, 0.1f).SetEase(Ease.OutBack)
                 .OnComplete(() => hitLine.transform.DOScaleY(originHeight, 0.1f).SetEase(Ease.InBack));
        }

        public void ReleaseKey(PianoNote pianoNote)
        {
            tween?.Kill();
            tween = hitLine.transform.DOScaleY(originHeight, 0.1f).SetEase(Ease.InBack);
            if (pianoNote == showingNote)
                hitNoteEffect.gameObject.SetActive(false);
        }

        internal void Init()
        {
            gameObject.SetActive(true);
            gameObject.transform.parent = Camera.main.transform;
            transform.position = originPosition;
        }

        public void Stop()
        {
            gameObject.SetActive(false);
            gameObject.transform.parent = GameManager.Instance.transform;
        }
    }
}