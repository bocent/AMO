using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    public Camera targetCamera;
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.root.transform, Input.mousePosition, targetCamera, out Vector2 localPos);
        ((RectTransform)transform).anchoredPosition = localPos;
        particle.Play();
    }
}
