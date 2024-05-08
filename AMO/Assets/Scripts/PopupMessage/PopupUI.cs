using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PopupUI : MonoBehaviour
{
    public bool playOnActive = true;
    public Ease easeType;
    protected void PlayScaleTween(Transform target, Vector3 from, Vector3 to, float duration)
    {
        target.localScale = from;
        target.DOScale(to, duration).SetEase(easeType).Play();
    }

    protected void PlayPositionTween(Transform target, Vector3 from, Vector3 to, float duration)
    {
        ((RectTransform)target).anchoredPosition = from;
        ((RectTransform)target).DOAnchorPos(to, duration).SetEase(easeType).Play();
    }

}
