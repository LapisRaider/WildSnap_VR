using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalPhotoUi : MonoBehaviour
{
    public TextMeshProUGUI m_stateText;
    public TextMeshProUGUI m_scroreText;

    public RawImage m_rawImage;

    public void UpdatePhotoUi(AnimalState animalState, AnimalPhotoInfo photoInfo)
    {
        m_rawImage.texture = photoInfo.m_photoTaken;
        m_scroreText.text = photoInfo.m_score.ToString();

        m_stateText.text = animalState.ToString();
    }
}
