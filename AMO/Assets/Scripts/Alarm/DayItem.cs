using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayItem : MonoBehaviour
{
    public DayPair dayPair;
    public TMP_Text activeDayText;
    public TMP_Text inactiveDayText;
    public Toggle onToggle;

    private void Start()
    {
        CultureInfo culture = new CultureInfo("id-ID");
        string day = culture.DateTimeFormat.GetDayName((DayOfWeek)dayPair.day);
        activeDayText.text = inactiveDayText.text = day;
        onToggle.onValueChanged.AddListener(OnToggleValueChanged);
        onToggle.onValueChanged.Invoke(onToggle.isOn);
    }

    public void Init(bool isOn)
    {
        onToggle.isOn = isOn;
        dayPair.isActive = isOn;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        dayPair.isActive = isOn;     
    }
}
