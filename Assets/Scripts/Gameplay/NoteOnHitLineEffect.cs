using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doulingo.Core;
using TMPro;
using UnityEngine;
namespace Doulingo.Gameplay
{
    public class NoteOnHitLineEffect : MonoBehaviour
    {
        [SerializeField] TMP_Text nameNoteText;
        [SerializeField] GameObject hitEffect;
        [SerializeField] GameObject notHitEffect;
        // [SerializeField] ParticleSystem particle;

        public void Show(PianoNote pianoNote, NoteData noteData)
        {
            gameObject.SetActive(true);
            float y = StaffMapCalculator.GetYPosition(pianoNote);
            var pos = new Vector2(transform.position.x, y);
            transform.position = pos;
            if (noteData != null && noteData.isHited)
            {
                hitEffect.gameObject.SetActive(true);
                notHitEffect.gameObject.SetActive(false);
            }
            else
            {
                nameNoteText.text = pianoNote.ToString();
                hitEffect.gameObject.SetActive(false);
                notHitEffect.gameObject.SetActive(true);
                // var main = particle.main;
                // main.duration = ServiceHub.Conductor.BeatDuration;
            }
        }

        void OnDisable()
        {
            hitEffect.gameObject.SetActive(false);
            notHitEffect.gameObject.SetActive(false);
        }
    }
}