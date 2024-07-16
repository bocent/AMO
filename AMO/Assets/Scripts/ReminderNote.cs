using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReminderNote : MonoBehaviour
{
    private Button button;
    private Image image;
    [SerializeField] private TMP_InputField titleInputField;
    [SerializeField] private TMP_InputField contentInputField;
    [SerializeField] private GameObject lineObj;

    private ToDoController controller;

    private void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(ShowTimer);
        titleInputField.onValueChanged.AddListener(SetTitleText);
        contentInputField.onValueChanged.AddListener(SetContentText);
    }

    public void Init(ToDoController controller, string title, string content)
    {
        Debug.LogError("init");
        this.controller = controller;
        titleInputField.text = title;
        contentInputField.text = content;

    }

    public string GetTitleText()
    {
        return titleInputField.text;
    }


    public void SetTitleText(string value)
    {
        titleInputField.text = value;
    }

    public void SetContentText(string value)
    {
        contentInputField.text = value;
    }

    public void ShowTimer()
    {
        
    }
}
