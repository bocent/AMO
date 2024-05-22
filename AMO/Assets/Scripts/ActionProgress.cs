using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ActionProgress : MonoBehaviour
{
    public GameObject container;
    public Image fillBar;
    public Button closeButton;

    private ActivityTask activeTask;

    public static ActionProgress Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        closeButton.onClick.AddListener(Hide);
    }

    public void SetFillBar(float value)
    {
        fillBar.fillAmount = value;
    }

    public void Show(ActivityTask activeTask)
    {
        this.activeTask = activeTask;
        container.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).Play();
        closeButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        closeButton.gameObject.SetActive(false);
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(OnComplete).Play();
        void OnComplete()
        {
            container.SetActive(false);
            if (activeTask)
            {
                activeTask.Close();
            }
        }
    }
}
