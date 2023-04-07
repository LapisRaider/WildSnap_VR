using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class Animal_Info : ScriptableObject
{
    public AnimalType m_type;
    [Tooltip("List of available states the animal can be in for players to take photos, and it's appropriate score.")]
    public List<AnimalAvailableStateScore> m_availableStateAndScore = new List<AnimalAvailableStateScore>();

    [Tooltip("Map name of states in animal's animator to it's appropriate states")]
    public List<AnimalAnimationStates> m_animStateData = new List<AnimalAnimationStates>();

    [Header("Dex display information")]
    public Sprite m_dexSprite;
    public string m_displayName;
    public string m_scientificName;
    public string m_habitat;
    public string m_diet;
    [TextArea] public string m_description;
}

[Serializable]
public class AnimalAnimationStates
{
    public string m_animStateName = "";
    public AnimalState m_state = AnimalState.Idling;
}

[Serializable]
public class AnimalAvailableStateScore
{
    public AnimalState m_state = AnimalState.Idling;
    public float m_score = 1;
}