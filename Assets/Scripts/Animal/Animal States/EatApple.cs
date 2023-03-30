using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatApple : IState
{
    private AnimalMotion m_animalMotion;
    private AnimalDetection m_animalDetection;
    private float time;
    private float m_eatingTime;
    private bool reached;

    public EatApple(AnimalMotion animalMotion, AnimalDetection animalDetection)
    {
        m_animalMotion = animalMotion;
        m_animalDetection = animalDetection;
        reached = false;
    }

    public override void OnEnter()
    {
        Debug.Log("apple");
        m_animalMotion.WalkToApple(m_animalDetection.GetApplePosition());
        time = 1.0f;
        m_eatingTime = Random.Range(5, 10);
    }

    public override void Tick()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            m_animalMotion.SetDestination(m_animalDetection.GetApplePosition());
            time += 1.0f;
        }
    }

    public override void OnExit()
    { 
        m_animalDetection.DestroyApple();
    }
    public override bool StateEnded()
    {
        if (m_animalMotion.ReachedDestination(3f) || reached){
            reached = true;
            m_animalMotion.StartEating();
            if (m_eatingTime > 0 && m_animalDetection.appleExists()){
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
