using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : PopupUI
{
    public enum AnimationType
    {
        SCALE,
        MOTION
    }
    
    public AnimationType animationType;
    public Vector3 from;
    public Vector3 to;

    public RectTransform panel;

    private void Awake()
    {
        //playOnActive = true;
    }

    protected virtual void OnEnable()
    {
        if (playOnActive)
        {
            Init();
        }
    }

    //protected override void Init()
    protected void Init()
    {
        if (animationType == AnimationType.MOTION)
        {
            panel.anchoredPosition = from;
            //tween = panel.DOAnchorPos(to, duration);
            PlayPositionTween(panel, from, to, 0.2f);
        }
        else
        {
            panel.localScale = from;
            //tween = panel.DOScale(to, duration);
            PlayScaleTween(panel, from, to, 0.2f);
        }
        //base.Init();
    }
}
