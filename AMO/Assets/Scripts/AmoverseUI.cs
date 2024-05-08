using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AmoverseUI : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ShowAmoverse);
    }

    public void ShowAmoverse()
    {
        SceneStackManager.Instance.LoadScene("Home", "CodeReader");
    }
}
