using System;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class Animal_Info : ScriptableObject
{
    public AnimalType m_type;

    [Header("Dex display information")]
    public Sprite m_dexSprite;
    public string m_displayName;
    public string m_description;
}
