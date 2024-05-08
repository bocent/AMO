using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour
{
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color deselectedColor = Color.white;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;

    private Toggle toggle;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnSelected);
        if (toggle.isOn)
        {
            toggle.onValueChanged?.Invoke(toggle.isOn);
        }
    }

    public void OnSelected(bool toggle)
    {
        Debug.Log("selected " + gameObject.name + " : " + toggle);
        if (toggle)
        {
            icon.color = label.color = selectedColor;
        }
        else
        {
            icon.color = label.color = deselectedColor;
        }
    }
}
