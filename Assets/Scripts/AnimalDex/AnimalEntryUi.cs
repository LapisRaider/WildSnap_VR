using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalEntryUi : MonoBehaviour
{
    [Header("UI")]
    public GameObject m_InfoSection;
    public GameObject m_InfoSectionUnknown;
    public string m_habitatPrefix = "<b>Habitat:</b> ";
    public string m_dietPrefix = "<b>Diet:</b> ";

    [Header("Info Section")]
    public TextMeshProUGUI m_displayName;
    public TextMeshProUGUI m_scientificName;
    public TextMeshProUGUI m_habitat;
    public TextMeshProUGUI m_diet;
    public TextMeshProUGUI m_description;
    public Image m_dexSprite;
    public Image m_dexSpriteUnknown;

    [Header("Photo Section")]
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

        //init animal info
        m_displayName.text = animalInfo.m_displayName;
        m_scientificName.text = animalInfo.m_scientificName;
        m_dexSprite.sprite = animalInfo.m_dexSprite;
        m_dexSpriteUnknown.sprite = animalInfo.m_dexSprite;
        m_habitat.text = m_habitatPrefix + animalInfo.m_habitat;
        m_diet.text = m_dietPrefix + animalInfo.m_diet;
        m_description.text = animalInfo.m_description;

        m_InfoSection.SetActive(hasSeenAnimal);
        m_InfoSectionUnknown.SetActive(!hasSeenAnimal);

        //hide all photos first
        foreach (var photoUi in m_animalPhotosUi)
        {
            photoUi.Value.gameObject.SetActive(false);
        }

        //init photos for available states
        foreach (AnimalAvailableStateScore availableStateAndScore in animalInfo.m_availableStateAndScore)
        {
            AnimalState state = availableStateAndScore.m_state;

            AnimalPhotoUi photoUi = m_animalPhotosUi[state];

            if (dexEntry.m_photos.ContainsKey(state))
            {
                photoUi.InitInfo(state, dexEntry.m_photos[state]);
            }
            else
            {
                photoUi.InitInfo(state, null);
            }

            photoUi.gameObject.SetActive(true);
        }

        if (!hasSeenAnimal)
            dexEntry.onAnimalNewlySeenCallback += AnimalSeenUpdate;

        m_currAnimalDexEntry.onAnimalPhotoUpdateCallback += UpdatePhoto;
    }

    public void AnimalSeenUpdate()
    {
        m_InfoSection.SetActive(true);
        m_InfoSectionUnknown.SetActive(false);
    }

    public void UpdatePhoto(AnimalState animalState)
    {
        AnimalPhotoUi photoUi = m_animalPhotosUi[animalState];
        photoUi.InitInfo(animalState, m_currAnimalDexEntry.m_photos[animalState]);
    }
}
