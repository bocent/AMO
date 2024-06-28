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
    private Vector3 lastPos;

    public void Show(Clean clean)
    {
        Debug.LogWarning("Soap");
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
            if (UserData.GetRequirementList().Contains((int)Main.RequirementType.NEED_FIX_UP))
            {
                PopupManager.Instance.ShowPopupMessage("err", "Tidak Dapat Membersihkan AMO", "AMO perlu diperbaiki terlebih dahulu", new ButtonInfo { content = "OK" });
                ActionProgress.Instance.Hide();
                Character.Instance.currentCharacter.PlayCleanUpAnimation(false);
                Hide();
            }
            else
            {
                Touch();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            soapImage.enabled = false;
            StopSoapParticle();
            Character.Instance.currentCharacter.PlayCleanUpAnimation(false);
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
            if (hitInfo.collider != null && lastPos != transform.position)
            {
                float value = clean.Cleaning();
                if (value >= 1)
                {
                    StopSoapParticle();
                    Character.Instance.currentCharacter.PlayCleanUpAnimation(false);
                }
                else
                {
                    PlaySoapParticle();
                    Character.Instance.currentCharacter.PlayCleanUpAnimation(true);
                }
            }
            else
            {
                Character.Instance.currentCharacter.PlayCleanUpAnimation(false);
            }
            lastPos = transform.position;
        }
        else
        {
            StopSoapParticle();
            Character.Instance.currentCharacter.PlayCleanUpAnimation(false);
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

        Character.Instance.currentCharacter.PlayCleanUpAnimation(false);
    }
}
