using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStateMachine : MonoBehaviour
{
    private StateMachine m_stateMachine;
    private AnimalMotion m_animalMotion;
    private AnimalWander m_animalWander;
    private AnimalDetection m_animalDetection;

    private Idle idle;
    // Start is called before the first frame update
    void Awake()
    {
        m_stateMachine = new StateMachine();

        void At(IState from, IState to, Func<bool> condition)
        {
            m_stateMachine.AddTransition(from, to, condition);
        }
        m_animalMotion = GetComponent<AnimalMotion>();
        m_animalWander = GetComponent<AnimalWander>();
        m_animalDetection = GetComponent<AnimalDetection>();
        //Initialize Animal States
        idle = new Idle(m_animalMotion);
        Walk walk = new Walk(m_animalMotion, m_animalWander);
        Sleep sleep = new Sleep(m_animalMotion);
        EatApple eatApple = new EatApple(m_animalMotion, m_animalDetection);

        At(idle, walk, idle.StateEnded);
        At(walk, idle, walk.StateEnded);
        At(idle, eatApple, m_animalDetection.AppleInRange);
        At(walk, eatApple, m_animalDetection.AppleInRange);
        At(eatApple, idle, eatApple.StateEnded);
    }

    private void Start()
    {
        m_stateMachine.SetState(idle);
    }
    // Update is called once per frame
    void Update()
    {
        m_stateMachine.Tick();
    }

}
