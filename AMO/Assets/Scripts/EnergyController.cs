using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class EnergyMeter
{
    public float maxEnergy;
    public float minEnergy;
    public Sprite sprite;
}

public class EnergyController : MonoBehaviour
{
    public List<EnergyMeter> energyMeterList;

    public Image energyImage;

    public void SetEnergy(float value)
    {
        Debug.LogWarning("set energy : " + value);
        energyImage.sprite = GetEnergySprite(value);
    }

    private Sprite GetEnergySprite(float value)
    {
        EnergyMeter energyMeter = energyMeterList.Where(x => x.maxEnergy >= value && x.minEnergy < value).FirstOrDefault();
        if(energyMeter != null) return energyMeter.sprite;
        return null;
    }

    private void Update()
    {
        SetEnergy(UserData.Energy);
    }
}
