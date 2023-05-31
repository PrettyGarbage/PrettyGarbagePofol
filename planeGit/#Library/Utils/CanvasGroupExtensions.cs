using DG.Tweening;
using UnityEngine;

namespace MJ.Utils
{
    public static class CanvasGroupExtensions
    {
        public static Tweener FadeOut(this CanvasGroup canvasGroup, float duration)
        {
            return canvasGroup.DOFade(0.0F, duration);
        }

        public static Tweener FadeIn(this CanvasGroup canvasGroup, float duration)
        {
            return canvasGroup.DOFade(1.0F, duration);
        }
    }
}