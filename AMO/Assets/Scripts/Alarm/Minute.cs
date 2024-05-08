using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Minute : MonoBehaviour
{
    public Transform parent;
    public GameObject prefab;

    public UnityEvent onInstantiateComplete;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        string time = "";
        for (int i = 0; i < 60; i++)
        {
            if (i < 10)
            {
                time = "0" + i;
            }
            else
            {
                time = i.ToString();
            }
            GameObject item = Instantiate(prefab, parent, false);
            item.GetComponent<TMP_Text>().text = time;
        }
        onInstantiateComplete?.Invoke();
    }
}
