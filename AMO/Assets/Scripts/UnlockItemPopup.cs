using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnlockItemPopup : MonoBehaviour
{
    public GameObject container;
    public TMP_Text contentText;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Set(string content)
    {
        container.SetActive(true);
        contentText.text = content;
    }
}
