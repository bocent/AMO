using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingStation : MonoBehaviour
{
    public Vector3 from;
    public Vector3 to;
    public float duration;

    private Tweener tween;

    private void OnEnable()
    {
        Show();
    }

    public void Show()
    {
        transform.localScale = from;
        transform.DOScale(to, duration).SetEase(Ease.OutBack).Play();        
    }

    public void Hide()
    {
        transform.localScale = to;
        transform.DOScale(from, 0.1f).SetEase(Ease.Linear).OnComplete(() => gameObject.SetActive(false)).Play();
    }
}
