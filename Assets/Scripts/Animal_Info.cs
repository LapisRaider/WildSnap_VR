using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class Animal_Info : ScriptableObject
{
    public AnimalType m_type;
    public List<AnimalState> m_availableStates = new List<AnimalState>();

    [Header("Dex display information")]
    public Sprite m_dexSprite;
    public string m_displayName;
    [TextArea] public string m_description;
}
