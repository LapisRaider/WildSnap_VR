using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : IState
{
    private AnimalMotion m_animalMotion;
    private AnimalWander m_animalWander;
    private Vector3 wanderPosition;
    public Walk(AnimalMotion animalMotion, AnimalWander animalWander)
    {
        m_animalMotion = animalMotion;
        m_animalWander = animalWander;
    }

    public override void OnEnter()
    {
        wanderPosition = m_animalWander.GetRandomPointInCircle();
        m_animalMotion.WalkToPoint(wanderPosition);
    }

    public override bool StateEnded()
    {
        return m_animalMotion.ReachedDestination();
    }
}
