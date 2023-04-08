using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToPlayer : IState
{
    private AnimalMotion m_animalMotion;
    private AnimalDetection m_animalDetection;
    private float m_attackingTime;
    private bool reached;
    private bool isAggressive;
    private bool isScared;
    private Vector3 endPos;

    public WalkToPlayer(AnimalMotion animalMotion, AnimalDetection animalDetection)
    {
        m_animalMotion = animalMotion;
        m_animalDetection = animalDetection;
        reached = false;
    }

    public override void OnEnter()
    {
        isAggressive = m_animalDetection.isAggressive;
        
        if (isAggressive) { 
            m_animalMotion.WalkToPoint(m_animalDetection.GetPlayerPosition());
        }
        else {
            endPos = m_animalDetection.GetAnimalPosition() + (m_animalDetection.GetAnimalPosition() - m_animalDetection.GetPlayerPosition())*m_animalDetection.fleeingCoef;
            endPos += new Vector3(0f,1f,0f);
            m_animalMotion.WalkToPoint(endPos);
        }
 
        m_attackingTime = 2f;
    }

    public override void Tick()
    {
        if (isAggressive ) { 
            if (m_animalDetection.PlayerInRange()) {
                m_animalMotion.SetDestination(m_animalDetection.GetPlayerPosition());
            }
            else {
                m_animalMotion.SetDestination(m_animalDetection.GetAnimalPosition());
            }

        }

        else {
            m_animalMotion.SetDestination(endPos);
        }
    }

    public override void OnExit()
    { 

    }
    public override bool StateEnded()
    {
        if (isAggressive) {
            if (m_animalMotion.ReachedDestination(1f) || reached){
                reached = true;
                if (m_animalDetection.PlayerInRange()){
                    m_animalMotion.StartWatchingPlayer();
                }
                if (m_attackingTime > 0){
                    m_attackingTime -= Time.deltaTime;
                }
                else {
                    
                    m_animalMotion.EndWatchingPlayer();
                    reached = false;
                    return true;
                }
            }
        }
        else {

            if (m_animalMotion.ReachedDestination(3f)) {
                m_animalMotion.SetIdle();
                return true;
            }
        }

        return false;
    }

}
