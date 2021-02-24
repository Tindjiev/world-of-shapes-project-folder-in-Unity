using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening
{
/*
    public static class MyDOTweenLib
    {

        public static Tween[] DOTweenSpriteRenderersAlpha(this GameObject gameObject, float targetAlpha, float duration)
        {
            return gameObject.transform.DOTweenSpriteRenderersAlpha(targetAlpha, duration);
        }

        public static Tween[] DOTweenSpriteRenderersAlpha(this Transform transform, float targetAlpha, float duration)
        {
            SpriteRenderer[] rends = transform.GetComponentsInChildren<SpriteRenderer>();
            Tween[] tweens = new Tween[rends.Length];
            for (int i = 0; i < rends.Length; ++i)
            {
                tweens[i] = rends[i].DOFade(rends[i].color.a * targetAlpha, duration);
            }
            return tweens;
        }

        public static Tween[] DOTweenCanvasRenderersAlpha(this GameObject gameObject, float targetAlpha, float duration)
        {
            return gameObject.transform.DOTweenCanvasRenderersAlpha(targetAlpha, duration);
        }

        public static Tween[] DOTweenCanvasRenderersAlpha(this Transform transform, float targetAlpha, float duration)
        {
            Graphic[] rends = transform.GetComponentsInChildren<Graphic>();
            Tween[] tweens = new Tween[rends.Length];
            for (int i = 0; i < rends.Length; ++i)
            {
                tweens[i] = rends[i].DOFade(rends[i].color.a * targetAlpha, duration);
            }
            return tweens;
        }

        public static Tween[] DOTweenRenderersAlpha(this IEnumerable<SpriteRenderer> renderers, float targetAlpha, float duration)
        {
            Tween[] tweens = new Tween[renderers.Count()];
            int i = -1;
            foreach (SpriteRenderer rend in renderers)
            {
                tweens[++i] = rend.DOFade(rend.color.a * targetAlpha, duration);
            }
            return tweens;
        }

        public static Tween[] DOTweenRenderersAlpha(this IEnumerable<Graphic> renderers, float targetAlpha, float duration)
        {
            Tween[] tweens = new Tween[renderers.Count()];
            int i = -1;
            foreach (Graphic rend in renderers)
            {
                tweens[++i] = rend.DOFade(rend.color.a * targetAlpha, duration);
            }
            return tweens;
        }

        public static Tween[] FadeOnSpriteRenderers(this GameObject gameObject, float duration)
        {
            return gameObject.transform.FadeOnSpriteRenderers(duration);
        }
        public static Tween[] FadeOnSpriteRenderers(this Transform transform, float duration)
        {
            SpriteRenderer[] rends = transform.GetComponentsInChildren<SpriteRenderer>();
            Tween[] tweens = new Tween[rends.Length];
            int i = -1;
            foreach (SpriteRenderer rend in rends)
            {
                float a = rend.color.a;
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0f);
                tweens[++i] = rend.DOFade(a, duration);
            }
            return tweens;
        }


        public static Tween[] FadeOnCanvasRenderers(this GameObject gameObject, float duration)
        {
            return gameObject.transform.FadeOnCanvasRenderers(duration);
        }
        public static Tween[] FadeOnCanvasRenderers(this Transform transform, float duration)
        {
            Graphic[] rends = transform.GetComponentsInChildren<Graphic>();
            Tween[] tweens = new Tween[rends.Length];
            int i = -1;
            foreach (Graphic rend in rends)
            {
                float a = rend.color.a;
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0f);
                tweens[++i] = rend.DOFade(a, duration);
            }
            return tweens;
        }



        public static Tween[] FadeOn(this IEnumerable<SpriteRenderer> renderers, float duration)
        {
            Tween[] tweens = new Tween[renderers.Count()];
            int i = -1;
            foreach (SpriteRenderer rend in renderers)
            {
                float a = rend.color.a;
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0f);
                tweens[++i] = rend.DOFade(a, duration);
            }
            return tweens;
        }
        public static Tween[] FadeOn(this IEnumerable<Graphic> renderers, float duration)
        {
            Tween[] tweens = new Tween[renderers.Count()];
            int i = -1;
            foreach (Graphic rend in renderers)
            {
                float a = rend.color.a;
                rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0f);
                tweens[++i] = rend.DOFade(a, duration);
            }
            return tweens;
        }

        public static Tween[] FadeOffSpriteRenderers(this GameObject gameObject, float duration)
        {
            return gameObject.transform.FadeOffSpriteRenderers(duration);
        }
        public static Tween[] FadeOffSpriteRenderers(this Transform transform, float duration)
        {
            return transform.DOTweenSpriteRenderersAlpha(0f, duration);
        }


        public static Tween[] FadeOffCanvasRenderers(this GameObject gameObject, float duration)
        {
            return gameObject.transform.FadeOffCanvasRenderers(duration);
        }
        public static Tween[] FadeOffCanvasRenderers(this Transform transform, float duration)
        {
            return transform.DOTweenCanvasRenderersAlpha(0f, duration);
        }



        public static Tween[] FadeOff(this IEnumerable<SpriteRenderer> renderers, float duration)
        {
            Tween[] tweens = new Tween[renderers.Count()];
            int i = -1;
            foreach (SpriteRenderer rend in renderers)
            {
                tweens[++i] = rend.DOFade(0f, duration);
            }
            return tweens;
        }
        public static Tween[] FadeOff(this IEnumerable<Graphic> renderers, float duration)
        {
            Tween[] tweens = new Tween[renderers.Count()];
            int i = -1;
            foreach (Graphic rend in renderers)
            {
                tweens[++i] = rend.DOFade(0f, duration);
            }
            return tweens;
        }
    }

    */
}
