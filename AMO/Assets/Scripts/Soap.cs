using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Soap : MonoBehaviour
{
    public LayerMask targetLayer;
    public ParticleSystem soapParticle;
    public Image soapImage;

    private Clean clean;
    private RectTransform rectTransform;

    public void Show(Clean clean)
    {
        this.clean = clean;
        gameObject.SetActive(true);
        soapImage.enabled = false;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        rectTransform = (RectTransform)transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Touch();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            soapImage.enabled = false;
            StopSoapParticle();
        }
    }

    private void Touch()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.root.transform, Input.mousePosition, null, out Vector2 localPos);
        rectTransform.anchoredPosition = localPos;
        soapImage.enabled = true;

        RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)transform.root.transform, Input.mousePosition, Camera.main, out Vector3 rayPos);
        rayPos.z -= 1f;
        soapParticle.transform.position = rayPos;
        if (Physics.Raycast(rayPos, transform.forward, out RaycastHit hitInfo, 10f, targetLayer))
        {
            Debug.LogWarning("touch : " + hitInfo.collider.name);
            if (hitInfo.collider != null)
            {
                float value = clean.Cleaning();
                if (value >= 1)
                {
                    StopSoapParticle();
                }
                else
                {
                    PlaySoapParticle();
                }
            }
        }
        else
        {
            StopSoapParticle();
        }
    }

    private void PlaySoapParticle()
    {
        if (!soapParticle.isPlaying)
            soapParticle.Play(true);
    }

    private void StopSoapParticle()
    {
        if (soapParticle.isPlaying)
            soapParticle.Stop(true);
    }
}
