using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : IState
{
    AnimalMotion m_animalMotion;
    public Idle(AnimalMotion animalMotion)
    {
        m_animalMotion = animalMotion;
    }

    public override void OnEnter()
    {
        m_animalMotion.SetIdle();
    }
}
