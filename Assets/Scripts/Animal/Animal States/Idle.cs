using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : IState
{
    AnimalMotion m_animalMotion;
    private float m_idleTime;
    public Idle(AnimalMotion animalMotion)
    {
        m_animalMotion = animalMotion;
    }

    public override void OnEnter()
    {
        Debug.Log("idle");
        m_animalMotion.SetIdle();
        m_idleTime = Random.Range(10, 20);
    }

    public override void Tick()
    {
        m_idleTime -= Time.deltaTime;
    }

    public override bool StateEnded()
    {
        return m_idleTime < 0;
    }
}
