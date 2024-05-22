using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BubbleNeeds : MonoBehaviour
{
    public Image icon;
    public ParticleSystem popParticle;
    public TweenScale tweenScale;
    public TweenFade tweenFade;

    public Image bubbleImage;

    public Main.RequirementType type { get; private set; }

    public void Reset()
    {
        bubbleImage.transform.localScale = Vector3.one;
        Color color = bubbleImage.color;
        color.a = 1f;
        bubbleImage.color = color;
        color = icon.color;
        color.a = 1f;
        icon.color = color;
    }

    public void Init(Main.RequirementType type, Sprite sprite)
    {
        this.type = type;
        icon.sprite = sprite;
    }

    public void Pop()
    {
        Debug.LogError("pop : " + type.ToString(), this);
        icon.DOFade(0, 0.2f).Play();
        popParticle.Play();
        tweenScale.Play();
        tweenFade.Play();
    }
}
