using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalPhotoUi : MonoBehaviour
{
    public GameObject m_photoUnknown;
    public GameObject m_photoKnown;
    public RawImage m_rawImage;
    public TextMeshProUGUI m_stateText;
    public TextMeshProUGUI m_scoreText;

    [Header("Score")]
    public GameObject m_gold;
    public GameObject m_silver;
    public GameObject m_bronze;
    public int m_minScoreGold;
    public int m_minScoreSilver;


    public void InitInfo(AnimalState animalState, AnimalPhotoInfo photoInfo)
    {
        m_photoUnknown.SetActive(photoInfo == null);
        m_photoKnown.SetActive(photoInfo != null);

        if (photoInfo != null)
        {
            m_rawImage.texture = photoInfo.m_photoTaken;
            m_stateText.text = animalState.ToString();
            m_scoreText.text = photoInfo.m_score.ToString();

            m_gold.SetActive(false);
            m_silver.SetActive(false);
            m_bronze.SetActive(false);

            if (photoInfo.m_score >= m_minScoreGold)
            {
                m_gold.SetActive(true);
            }
            else if (photoInfo.m_score >= m_minScoreSilver)
            {
                m_silver.SetActive(true);
            }
            else
            {
                m_bronze.SetActive(true);
            }
        }
    }
}
