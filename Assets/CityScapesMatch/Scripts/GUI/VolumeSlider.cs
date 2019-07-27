using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {

    public Image [] full;


    public void SetVolume(float volume)
    {
        int fullCount = (int) (volume * 10.0f);
        for (int i = 0; i < full.Length; i++)
        {
            full[i].enabled = (fullCount >= (i + 1));
        }
    }
}
