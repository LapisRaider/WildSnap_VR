using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : IState
{
    private AnimalMotion m_animalMotion;
    private AnimalWander m_animalWander;
    private Vector3 wanderPosition;
    private float time = 5f;
    private Vector3 previousPos;
    public Walk(AnimalMotion animalMotion, AnimalWander animalWander)
    {
        m_animalMotion = animalMotion;
        m_animalWander = animalWander;
    }

    public override void OnEnter()
    {
        time = 4f;
        wanderPosition = m_animalWander.GetRandomPointInCircle();
        m_animalMotion.WalkToPoint(wanderPosition);
    }

    public override bool StateEnded()
    {
        time -= Time.deltaTime;
        return (time < 0f || m_animalMotion.ReachedDestination(1f));
    }
}
