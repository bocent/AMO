using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenColor : Tweening
{
    public Color from = Color.white;
    public Color to = Color.black;
    public enum FadeType
    {
        graphic,
        spriteRenderer
    }
    public FadeType fadeType;

    private Graphic graphic;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        graphic = GetComponent<Graphic>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Init()
    {
        base.Init();
    }

    public override void Play()
    {
        switch (fadeType)
        {
            case FadeType.graphic:
                graphic.color = from;
                tween = graphic.DOColor(to, duration);
                break;
            case FadeType.spriteRenderer:
                spriteRenderer.color = from;
                tween = spriteRenderer.DOColor(to, duration);
                break;
        }
        Init();
        tween.Play();
    }
}
