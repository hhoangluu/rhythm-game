using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Doulingo.UI
{
    /// <summary>
    /// Piano key button with press/release animation
    /// </summary>
    public class PianoKeyButton : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float pressDistance = 10f; // How far down to move when pressed
        [SerializeField] private float animationDuration = 0.1f; // Animation speed
        [SerializeField] private Ease pressEase = Ease.OutQuad; // Easing for press
        [SerializeField] private Ease releaseEase = Ease.OutBack; // Easing for release with bounce

        // References
        [SerializeField] RectTransform pressRectTransform;
        private Vector3 originalPosition;
        private Tween currentAnimation;

        private void Awake()
        {
            originalPosition = pressRectTransform.localPosition;
        }

        /// <summary>
        /// Animate button press - move down
        /// </summary>
        public void Press()
        {
            if (pressRectTransform == null) return;

            // Kill any existing animation
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }

            // Animate to pressed position
            currentAnimation = pressRectTransform.DOLocalMoveY(originalPosition.y - pressDistance, animationDuration)
                .SetEase(pressEase)
                .OnComplete(() => currentAnimation = null);

            Debug.Log($"[PianoKeyButton] Button pressed, moving down by {pressDistance} units");
        }

        /// <summary>
        /// Animate button release - move back to original position
        /// </summary>
        public void Release()
        {
            if (pressRectTransform == null) return;

            // Kill any existing animation
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }

            // Animate back to original position
            currentAnimation = pressRectTransform.DOLocalMove(originalPosition, animationDuration)
                .SetEase(releaseEase)
                .OnComplete(() => currentAnimation = null);

            Debug.Log($"[PianoKeyButton] Button released, moving back to original position");
        }

        /// <summary>
        /// Reset button to original position immediately (no animation)
        /// </summary>
        public void ResetPosition()
        {
            if (pressRectTransform == null) return;

            // Kill any existing animation
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
                currentAnimation = null;
            }

            // Reset to original position immediately
            pressRectTransform.localPosition = originalPosition;
            Debug.Log("[PianoKeyButton] Button position reset immediately");
        }

        private void OnDestroy()
        {
            // Clean up any active animations
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }
        }
    }
}
