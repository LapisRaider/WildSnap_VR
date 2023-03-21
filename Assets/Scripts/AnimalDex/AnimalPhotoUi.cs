using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalPhotoUi : MonoBehaviour
{
    [Header("Ui objects")]
    public TextMeshProUGUI m_stateText;
    public TextMeshProUGUI m_scroreText;
    public RawImage m_rawImage;

    [Header("Ui settings")]
    public Color NO_PHOTO_COLOR = Color.black;

    public void InitInfo(AnimalState animalState, AnimalPhotoInfo photoInfo)
    {
        if (photoInfo == null)
        {
            m_rawImage.texture = null;
            m_rawImage.color = NO_PHOTO_COLOR;

            m_scroreText.gameObject.SetActive(false);
        }
        else
        {
            m_rawImage.texture = photoInfo.m_photoTaken;
            m_rawImage.color = Color.white;

            m_scroreText.text = photoInfo.m_score.ToString();
            m_scroreText.gameObject.SetActive(true);
        }

        m_stateText.text = animalState.ToString();
    }
}
