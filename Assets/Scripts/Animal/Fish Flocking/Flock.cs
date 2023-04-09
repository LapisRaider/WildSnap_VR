using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [Header("Spawn setup")]
    public FlockUnit m_flockPrefab;
    public int m_flockSize = 10;
    public Vector3 m_spawnBounds = new Vector3(1.0f, 1.0f, 1.0f);

    [Header("Unit Settings")]
    public Vector2 m_minMaxSpeed;
    public float m_boundaryDetectionDist = 3.0f;
    public float m_boundaryAvoidanceWeight = 10.0f;
    private BoxCollider m_bounds;


    [Header("Detection Distance")]
    public float m_cohesionDist;


    [HideInInspector] public FlockUnit[] m_flockUnits = null;

    // Start is called before the first frame update
    void Awake()
    {
        m_bounds = GetComponent<BoxCollider>();
        GenerateFlock();
    }

    void GenerateFlock()
    {
        m_flockUnits = new FlockUnit[m_flockSize];
        for (int i = 0; i < m_flockSize; ++i)
        {
            Vector3 randomOffset = Random.insideUnitSphere;
            randomOffset = new Vector3(randomOffset.x * m_spawnBounds.x, randomOffset.y * m_spawnBounds.y, randomOffset.z * m_spawnBounds.z);

            Vector3 spawnPos = randomOffset + transform.position;
            var rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
            m_flockUnits[i] = Instantiate(m_flockPrefab, spawnPos, rotation);
            m_flockUnits[i].transform.parent = transform;
            m_flockUnits[i].m_assignedFlock = this;
            m_flockUnits[i].m_speed = Random.Range(m_minMaxSpeed.x, m_minMaxSpeed.y);
            m_flockUnits[i].m_boundary = m_bounds;
            m_flockUnits[i].m_boundaryDetectionDist = m_boundaryDetectionDist;
            m_flockUnits[i].m_boundaryAvoidanceWeight = m_boundaryAvoidanceWeight;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_flockSize; ++i)
        {
            m_flockUnits[i].MoveUnit();
        }
    }
}
