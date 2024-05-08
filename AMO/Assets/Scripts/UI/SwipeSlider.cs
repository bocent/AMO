using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SwipeSlider : MonoBehaviour
{
    public Slider slider;
    public EventTrigger trigger;

    private void Start()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(OnSliderReleased);
        trigger.triggers.Add(entry);
        slider.interactable = true;
    }

    private void OnSliderReleased(BaseEventData data)
    {
        Debug.LogWarning("OnSliderReleased");
        if (slider.value < 1f)
        {
            RevertSlider();
        }
        else
        {
            slider.interactable = false;
        }
    }

    private void RevertSlider()
    {
        slider.DOValue(0f, 0.2f).Play();
    }
}
