using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReminderNote : MonoBehaviour
{
    private Button button;
    private Image image;
    [SerializeField] private TMP_Text text;

    private ToDoController controller;
    private EditNote editNote;

    private void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(ShowNote);
    }

    public void Init(ToDoController controller, EditNote editNote, string content)
    {
        Debug.LogError("init");
        this.controller = controller;
        this.editNote = editNote;
        text.text = content;
    }

    public string GetText()
    {
        return text.text;
    }

    public void SetText(string value)
    {
        text.text = value;
    }

    private void ShowNote()
    {
        editNote.Show(controller, this, image.sprite);
    }
}
