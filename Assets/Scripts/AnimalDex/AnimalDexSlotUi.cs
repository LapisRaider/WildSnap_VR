using UnityEngine;
using UnityEngine.UI;

public class AnimalDexSlotUi : MonoBehaviour
{
    public Image m_animalImg;
    private AnimalType m_animalType;

    public delegate void DexSlotClicked(AnimalType type);
    DexSlotClicked dexSlotClickedCallback;

    public void Init(AnimalType type, AnimalDexEntry dexEntry, DexSlotClicked clickCallback)
    {
        m_animalImg.sprite = dexEntry.m_animalInfo.m_dexSprite;
        m_animalImg.color = dexEntry.m_photos.Count > 0 ? Color.white : Color.black;

        //if the photos are updated for this animal, make the sprite visible
        dexEntry.onAnimalPhotoUpdateCallback += SetSpriteVisible;

        dexSlotClickedCallback = clickCallback;
    }

    public void SetSpriteVisible()
    {
        m_animalImg.color = Color.white;
    }

    public void Clicked()
    {
        dexSlotClickedCallback?.Invoke(m_animalType);
    }
}
