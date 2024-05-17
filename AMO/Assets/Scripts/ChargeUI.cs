using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUI : MonoBehaviour
{
    public Button inventoryButton;

    private void Start()
    {
        inventoryButton.onClick.AddListener(ShowInventory);
    }

    private void ShowInventory()
    {
        HomeController.Instance.ShowInventory(true);
    }
}
