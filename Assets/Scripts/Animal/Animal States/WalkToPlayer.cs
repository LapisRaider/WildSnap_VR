using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToPlayer : IState
{
    private AnimalMotion m_animalMotion;
    private AnimalDetection m_animalDetection;
    private float m_eatingTime;
    private bool reached;

    public WalkToPlayer(AnimalMotion animalMotion, AnimalDetection animalDetection)
    {
        m_animalMotion = animalMotion;
        m_animalDetection = animalDetection;
        reached = false;
    }

    public override void OnEnter()
    {
        Debug.Log("player");
        m_animalMotion.WalkToPoint(m_animalDetection.GetPlayerPosition());
 
        m_eatingTime = Random.Range(5, 10);
    }

    public override void Tick()
    {
        m_animalMotion.SetDestination(m_animalDetection.GetPlayerPosition());
    }

    public override void OnExit()
    { 

    }
    public override bool StateEnded()
    {
        if (m_animalMotion.ReachedPlayer() || reached){
            reached = true;
            m_animalMotion.StartEating();
            if (m_eatingTime > 0){
                m_eatingTime -= Time.deltaTime;
            }
            else {
                m_animalMotion.EndEating();
                reached = false;
                return true;
            }

        }
        return false;
    }

}
