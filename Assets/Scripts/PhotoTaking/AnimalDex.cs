using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalDex : MonoBehaviour
{
    public List<Animal_Info> m_animalInfo = new List<Animal_Info>();

    private Dictionary<AnimalType, AnimalDexEntry> m_dexEntries = new Dictionary<AnimalType, AnimalDexEntry>();

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Animal_Info animalInfo in m_animalInfo)
        {
            m_dexEntries.Add(animalInfo.m_type, new AnimalDexEntry(animalInfo));
        }
    }

    /**
     * Checks whether photo can be added into it's respective animal dex entry
     */
    public bool AddPhotoToDexEntry(AnimalType animalType, AnimalState animalState, int photoScore, RawImage photo)
    {
        AnimalDexEntry currEntry = m_dexEntries[animalType];
        Dictionary<AnimalState, AnimalPhotoInfo> currPhotos = currEntry.m_photos;

        //check if animal state exists, if don't exist just insert
        if (currPhotos.ContainsKey(animalState))
        {
            AnimalPhotoInfo prevPhoto = currPhotos[animalState];

            //prev photo score is higher or same, don't update photo
            if (prevPhoto.m_score >= photoScore)
                return false;

            prevPhoto.m_photoTaken = photo;
            prevPhoto.m_score = photoScore;
        }
        else
        {
            currPhotos.Add(animalState, new AnimalPhotoInfo(photo, photoScore));
        }

        return true;
    }
}

[Serializable]
public class AnimalDexEntry
{
    public Animal_Info m_animalInfo = null;
    public Dictionary<AnimalState, AnimalPhotoInfo> m_photos = new Dictionary<AnimalState, AnimalPhotoInfo>();

    public AnimalDexEntry(Animal_Info animalInfo)
    {
        m_animalInfo = animalInfo;
    }
}

[Serializable]
public class AnimalPhotoInfo
{
    public RawImage m_photoTaken;
    public int m_score = 0;

    public AnimalPhotoInfo(RawImage photo, int score)
    {
        m_photoTaken = photo;
        m_score = score;
    }
}