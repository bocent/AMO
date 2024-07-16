using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class EditNote : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button calendarButton;

    public void Init(DatePicker_InputField datePicker)
    {
        calendarButton.onClick.RemoveAllListeners();
        calendarButton.onClick.AddListener(datePicker.ToggleDisplay);        
    }
}
