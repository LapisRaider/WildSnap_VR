using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : IState
{
    AnimalMotion m_animalMotion;
    public Sleep(AnimalMotion animalMotion)
    {
        m_animalMotion = animalMotion;
    }

    public override void OnEnter()
    {
        m_animalMotion.SetSleep();
    }
}
