using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class EnergyMeter
{
    public int minEnergy;
    public Sprite sprite;
}

public class EnergyController : MonoBehaviour
{
    public List<EnergyMeter> energyMeterList;

    public Image energyImage;

    public void SetEnergy(float value)
    {
        energyImage.sprite = GetEnergySprite(value);
    }

    private Sprite GetEnergySprite(float value)
    {
        float maxEnergy = 100;
        for (int i = 0; i < energyMeterList.Count; i++)
        {
            if (value > energyMeterList[i].minEnergy && value <= maxEnergy)
            {
                return energyMeterList[i].sprite;
            }
            maxEnergy = energyMeterList[i].minEnergy;
        }

        return null;
    }
}
