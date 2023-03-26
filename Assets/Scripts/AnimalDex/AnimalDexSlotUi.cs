using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalDexSlotUi : MonoBehaviour
{
    [Header("UI Objects")]
    public Image m_animalImg;
    public Image m_animalImgUnknown;
    public TextMeshProUGUI m_animalNameText;

    [Header("UI settings")]
    public string NOT_SEEN_TEXT = "";

    [Header("Internal info")]
    private AnimalType m_animalType;
    public delegate void DexSlotClicked(AnimalType type);
    DexSlotClicked dexSlotClickedCallback;

    public void Init(AnimalType type, AnimalDexEntry dexEntry, DexSlotClicked clickCallback)
    {
        bool hasSeenAnimal = dexEntry.m_photos.Count > 0;

        m_animalType = type;
        m_animalNameText.text = hasSeenAnimal ? dexEntry.m_animalInfo.m_displayName : NOT_SEEN_TEXT;

        //set sprite
        m_animalImg.sprite = dexEntry.m_animalInfo.m_dexSprite;
        m_animalImgUnknown.sprite = dexEntry.m_animalInfo.m_dexSprite;
        m_animalImg.gameObject.SetActive(hasSeenAnimal);
        m_animalImgUnknown.gameObject.SetActive(!hasSeenAnimal);

        //if the photos are updated for this animal, make the sprite visible
        dexEntry.onAnimalNewlySeenCallback += SetSpriteVisible;

        dexSlotClickedCallback = clickCallback;
    }

    public void SetSpriteVisible()
    {
        m_animalImg.gameObject.SetActive(true);
        m_animalImgUnknown.gameObject.SetActive(false);
    }

    public void Clicked()
    {
        dexSlotClickedCallback?.Invoke(m_animalType);
    }
}
