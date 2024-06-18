using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class NoteInfo
{
    public string content;
}

[Serializable]
public class NoteList
{
    public List<NoteInfo> noteList;
}

public class ToDoController : MonoBehaviour
{
    private Button button;
    public Button backButton;
    public GameObject container;
    public EditNote editNote;
    public List<ReminderNote> reminderNoteList;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ShowReminderList);
        backButton.onClick.AddListener(HideReminderList);
        LoadNotes();

        //Debug.LogError("tes notes : " +  LoadNotesAsText());
    }

    public string LoadNotesAsText()
    {
        if (PlayerPrefs.HasKey("notes"))
        {
            string list = "";
            string notesJson = PlayerPrefs.GetString("notes");
            Debug.LogError("notesJson : " + notesJson);
            NoteList noteList = JsonUtility.FromJson<NoteList>(notesJson);
            for (int i = 0; i < noteList.noteList.Count; i++)
            {
                list += noteList.noteList[i].content;
                if (i < noteList.noteList.Count - 1)
                {
                    if (!string.IsNullOrEmpty(noteList.noteList[i].content))
                    {
                        list += "<break time=\"0.5s\" />";
                    }
                }
            }
            return list;
        }
        else
        {
            return "Saat ini daftar masih kosong";
        }
    }

    private void LoadNotes()
    {
        NoteList noteList = null;

        if (PlayerPrefs.HasKey("notes"))
        {
            string notesJson = PlayerPrefs.GetString("notes");
            Debug.LogError("notesJson : " + notesJson);
            noteList = JsonUtility.FromJson<NoteList>(notesJson);
        }
        else
        {
            noteList = new NoteList { noteList = new List<NoteInfo> { 
                new NoteInfo { content = "" },
                new NoteInfo { content = "" },
                new NoteInfo { content = "" },
                new NoteInfo { content = "" },
                new NoteInfo { content = "" }
            } };
            string notesJson = JsonUtility.ToJson(noteList);
            Debug.LogError("notesJson : " + notesJson);
            PlayerPrefs.SetString("notes", notesJson);
            PlayerPrefs.Save();
        }

        Debug.LogError("note count : " + noteList.noteList.Count);

        for(int i = 0; i < reminderNoteList.Count; i++)
        {
            reminderNoteList[i].Init(this, editNote, noteList.noteList[i].content);
        }
    }

    public void SaveNotes()
    {
        NoteList noteList = new NoteList { noteList = new List<NoteInfo>() };
        for (int i = 0; i < reminderNoteList.Count; i++)
        {
            noteList.noteList.Add(new NoteInfo { content = reminderNoteList[i].GetText() });
        }
        string notesJson = JsonUtility.ToJson(noteList);
        PlayerPrefs.SetString("notes", notesJson);
        PlayerPrefs.Save();
    }

    private void ShowReminderList()
    {
        container.SetActive(true);
    }

    private void HideReminderList()
    {
        container.SetActive(false);
    }
}
