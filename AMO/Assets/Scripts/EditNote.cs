using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditNote : MonoBehaviour
{
    public TMP_Text noteText;
    public TMP_InputField noteInputField;
    public Button closeButton;
    public GameObject container;
    public Image noteImage;

    private ToDoController controller;
    private ReminderNote currentNote;

    private void Start()
    {
        noteInputField.onValueChanged.AddListener(SubmitNote);
        closeButton.onClick.AddListener(Hide);
    }

    private void SubmitNote(string content)
    {
        noteText.text = content;
        currentNote.SetText(content);
    }

    public void Show(ToDoController controller, ReminderNote currentNote, Sprite sprite)
    {
        this.controller = controller;
        this.currentNote = currentNote;
        noteImage.sprite = sprite;
        container.SetActive(true);
        noteInputField.text = currentNote.GetText();
    }

    private void Hide()
    {
        controller.SaveNotes();
        container.SetActive(false);
    }
}
