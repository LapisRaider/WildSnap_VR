using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalEntryUi : MonoBehaviour
{
    [Header("Animal info section")]
    public TextMeshProUGUI m_description;
    public TextMeshProUGUI m_displayName;
    public Image m_dexSprite;

    [Header("Seen Ui")]
    public Color NOT_SEEN_COLOR = Color.black;
    public string NOT_SEEN_TEXT = "???";

    [Header("Photo section")]
    public GameObject m_imagePrefab;
    public Transform m_imageParent;
    private Dictionary<AnimalState, AnimalPhotoUi> m_animalPhotosUi = new Dictionary<AnimalState, AnimalPhotoUi>();

    private AnimalDexEntry m_currAnimalDexEntry = null;

    public void Awake()
    {
        foreach (var i in Enum.GetValues(typeof(AnimalState)))
        {
            GameObject image = Instantiate(m_imagePrefab, m_imageParent);
            m_animalPhotosUi.Add((AnimalState)i, image.GetComponent<AnimalPhotoUi>());
            image.SetActive(false);
        }
    }

    public void InitInfo(AnimalDexEntry dexEntry)
    {
        //remove delegates from prev animal dex entry
        if (m_currAnimalDexEntry != null)
        {
            m_currAnimalDexEntry.onAnimalNewlySeenCallback -= AnimalSeenUpdate;
            m_currAnimalDexEntry.onAnimalPhotoUpdateCallback -= UpdatePhoto;
        }

        m_currAnimalDexEntry = dexEntry;

        Animal_Info animalInfo = dexEntry.m_animalInfo;
        bool hasSeenAnimal = dexEntry.m_photos.Count > 0;

        //init data of animal ui
        m_dexSprite.sprite = animalInfo.m_dexSprite;
        m_description.text = hasSeenAnimal ? animalInfo.m_description : NOT_SEEN_TEXT;
        m_displayName.text = hasSeenAnimal ? animalInfo.m_displayName : NOT_SEEN_TEXT;
        m_dexSprite.color = hasSeenAnimal ? Color.white : NOT_SEEN_COLOR;

        //hide all photos first
        foreach (var photoUi in m_animalPhotosUi)
        {
            photoUi.Value.gameObject.SetActive(false);
        }

        //init photos for available states
        foreach (AnimalState availableState in animalInfo.m_availableStates)
        {
            AnimalPhotoUi photoUi = m_animalPhotosUi[availableState];

            if (dexEntry.m_photos.ContainsKey(availableState))
                photoUi.InitInfo(availableState, dexEntry.m_photos[availableState]);
            else
                photoUi.InitInfo(availableState, null);

            photoUi.gameObject.SetActive(true);
        }

        if (!hasSeenAnimal)
            dexEntry.onAnimalNewlySeenCallback += AnimalSeenUpdate;

        m_currAnimalDexEntry.onAnimalPhotoUpdateCallback += UpdatePhoto;
    }

    public void AnimalSeenUpdate()
    {
        m_description.text = m_currAnimalDexEntry.m_animalInfo.m_description;
        m_displayName.text = m_currAnimalDexEntry.m_animalInfo.m_displayName;
        m_dexSprite.color = Color.white;
    }

    public void UpdatePhoto(AnimalState animalState)
    {
        AnimalPhotoUi photoUi = m_animalPhotosUi[animalState];
        photoUi.InitInfo(animalState, m_currAnimalDexEntry.m_photos[animalState]);
    }
}
