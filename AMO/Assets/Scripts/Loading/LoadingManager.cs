using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    public GameObject spinLoading;
    public GameObject barLoading;

    public static LoadingManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowSpinLoading()
    {
        spinLoading.SetActive(true);
    }

    public void HideSpinLoading()
    {
        spinLoading.SetActive(false);
    }
}
