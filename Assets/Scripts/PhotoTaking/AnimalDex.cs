using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimalDex : SingletonBase<AnimalDex>
{
    public List<Animal_Info> m_animalInfo = new List<Animal_Info>();

    private Dictionary<AnimalType, AnimalDexEntry> m_dexEntries = new Dictionary<AnimalType, AnimalDexEntry>();

    public override void Awake()
    {
        foreach (Animal_Info animalInfo in m_animalInfo)
        {
            m_dexEntries.Add(animalInfo.m_type, new AnimalDexEntry(animalInfo));
        }
    }

    /**
     * Checks whether photo can be added into it's respective animal dex entry
     */
    public bool AddPhotoToDexEntry(AnimalType animalType, AnimalState animalState, int photoScore, Texture2D photo)
    {
        AnimalDexEntry currEntry = m_dexEntries[animalType];
        return currEntry.UpdatePhotos(animalState, photoScore, photo);
    }

    public Dictionary<AnimalType, AnimalDexEntry> GetDexEntries()
    {
        return m_dexEntries;
    }
}

[Serializable]
public class AnimalDexEntry
{
    public delegate void OnAnimalPhotoUpdate();
    public OnAnimalPhotoUpdate onAnimalPhotoUpdateCallback;

    public Animal_Info m_animalInfo = null;
    public Dictionary<AnimalState, AnimalPhotoInfo> m_photos = new Dictionary<AnimalState, AnimalPhotoInfo>();

    public AnimalDexEntry(Animal_Info animalInfo)
    {
        m_animalInfo = animalInfo;
    }

    public bool UpdatePhotos(AnimalState animalState, int photoScore, Texture2D photo)
    {
        //check if animal state exists, if don't exist just insert
        if (m_photos.ContainsKey(animalState))
        {
            AnimalPhotoInfo prevPhoto = m_photos[animalState];

            //prev photo score is higher or same, don't update photo
            if (prevPhoto.m_score >= photoScore)
                return false;

            prevPhoto.m_photoTaken = photo;
            prevPhoto.m_score = photoScore;
        }
        else
        {
            m_photos.Add(animalState, new AnimalPhotoInfo(photo, photoScore));
        }

        if (onAnimalPhotoUpdateCallback != null)
            onAnimalPhotoUpdateCallback.Invoke();
        return true;
    }
}

[Serializable]
public class AnimalPhotoInfo
{
    public Texture2D m_photoTaken;
    public int m_score = 0;

    public AnimalPhotoInfo(Texture2D photo, int score)
    {
        m_photoTaken = photo;
        m_score = score;
    }
}