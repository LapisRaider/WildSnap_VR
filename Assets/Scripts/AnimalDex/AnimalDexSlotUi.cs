using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalDexSlotUi : MonoBehaviour
{
    [Header("UI Objects")]
    public Image m_animalImg;
    public TextMeshProUGUI m_animalNameText;

    [Header("UI settings")]
    public Color NOT_SEEN_COLOR = Color.black;
    public Color SEEN_COLOR = Color.white;
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
        m_animalImg.color = hasSeenAnimal ? SEEN_COLOR : NOT_SEEN_COLOR;

        //if the photos are updated for this animal, make the sprite visible
        dexEntry.onAnimalPhotoUpdateCallback += SetSpriteVisible;

        dexSlotClickedCallback = clickCallback;
    }

    public void SetSpriteVisible()
    {
        m_animalImg.color = SEEN_COLOR;
    }

    public void Clicked()
    {
        dexSlotClickedCallback?.Invoke(m_animalType);
    }
}
