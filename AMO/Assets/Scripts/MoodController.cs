using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoodController : MonoBehaviour
{
    public Image moodImage;

    private void Update()
    {
        Limitation limitation = HomeController.Instance.currentLimitation;
        if (limitation != null)
        {
            Sprite sprite = Main.Instance.GetSprite(((Main.MoodStage)limitation.mood).ToString());
            if (sprite) moodImage.sprite = sprite;
        }
    }
}
