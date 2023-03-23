using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalWander : MonoBehaviour
{
    [SerializeField] private float m_wanderRadius;
    Vector3 m_anchorPosition;
    public void Awake()
    {
        m_anchorPosition = transform.position;
    }

    public Vector3 GetRandomPointInCircle()
    {
        Vector2 randomCircle = Random.insideUnitCircle;
        return m_anchorPosition + new Vector3(randomCircle.x, 0, randomCircle.y) * m_wanderRadius;
    }
}
