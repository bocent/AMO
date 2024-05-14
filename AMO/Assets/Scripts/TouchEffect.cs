using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    private ParticleSystem particle;
    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Touch();
        }
    }

    private void Touch()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = worldPos;
        Debug.LogWarning("touch : " + worldPos);
        particle.Emit(10);
    }
}
