using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStateMachine : MonoBehaviour
{
    private StateMachine m_stateMachine;
    private List<Transform> m_appleTransforms;
    private Transform m_playerTransform;
    [SerializeField] float detectionRadius;
    // Start is called before the first frame update
    void Awake()
    {
        m_stateMachine = new StateMachine();
        GameObject[] apples = GameObject.FindGameObjectsWithTag("Apple");
        foreach (GameObject apple in apples)
        {
            m_appleTransforms.Add(apple.transform);
        }
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        void At(IState from, IState to, Func<bool> condition)
        {
            m_stateMachine.AddTransition(from, to, condition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_stateMachine.Tick();
    }

    private bool PlayerInRange()
    {
        return Vector3.SqrMagnitude(transform.position - m_playerTransform.position) < detectionRadius * detectionRadius;
    }

    private Transform appleInRange()
    {
        foreach (Transform apple in m_appleTransforms)
        {
            if (Vector3.SqrMagnitude(transform.position - apple.position) < detectionRadius * detectionRadius)
            {
                return apple;
            }
        }

        return null;
    }
}
