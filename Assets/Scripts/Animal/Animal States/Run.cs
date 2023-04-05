using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : IState
{
    AnimalMotion m_animalMotion;
    public Run(AnimalMotion animalMotion)
    {
        m_animalMotion = animalMotion;
    }

}
