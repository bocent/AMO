using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessory : MonoBehaviour
{
    public AccessoryInfo Info { get; private set; }

    public void Init(AccessoryInfo info)
    {
        Info = info;
    }
}
