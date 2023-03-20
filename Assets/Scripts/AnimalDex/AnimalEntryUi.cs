using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalEntryUi : MonoBehaviour
{
    public TextMeshProUGUI m_description;
    public TextMeshProUGUI m_displayName;

    public Image m_dexSprite;

    public List<AnimalPhotoUi> m_animalPhotosUi = new List<AnimalPhotoUi>();

    public void UpdateInfo(AnimalDexEntry dexEntry)
    {
        Animal_Info animalInfo = dexEntry.m_animalInfo;

        m_description.text = animalInfo.m_description;
        m_displayName.text = animalInfo.m_displayName;
        m_dexSprite.sprite = animalInfo.m_dexSprite;

        foreach (AnimalPhotoUi photoUi in m_animalPhotosUi)
        {
            photoUi.gameObject.SetActive(false);
        }

        //TODO:: TEMP CODES, SHOULD JUST DISPLAY MISSING STATES
        int currPhotoUiIndex = 0;
        foreach (var photo in dexEntry.m_photos)
        {
            m_animalPhotosUi[currPhotoUiIndex].UpdatePhotoUi(photo.Key, photo.Value);
            m_animalPhotosUi[currPhotoUiIndex].gameObject.SetActive(true);

            ++currPhotoUiIndex;
        }

        //the remaining photos make it inactive since they're not taken yet
        for (int i = currPhotoUiIndex + 1; i < m_animalPhotosUi.Count; ++i)
        {
            m_animalPhotosUi[i].gameObject.SetActive(false);
        }
    }
}
