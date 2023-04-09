using UnityEngine;
using System.Collections.Generic;
using System;

public class FlockUnit : MonoBehaviour
{
    [Tooltip("So it doesnt collide with other units behind")]
    public float m_fovAngle = 1.0f;
    public float m_smoothDamp = 1.0f;

    [HideInInspector] public float m_speed = 1.0f;
    [HideInInspector] public Flock m_assignedFlock = null;
    [HideInInspector] public BoxCollider m_boundary = null;
    [HideInInspector] public float m_boundaryDetectionDist = 3.0f;
    [HideInInspector] public float m_boundaryAvoidanceWeight = 10.0f;

    private List<FlockUnit> m_cohesionNeighbours = new List<FlockUnit>();
    private Vector3 m_currVel;

    public void MoveUnit()
    {
        FindNeighbours();

        Vector3 cohesionVec = CalculateCohesionVector();
        if (!m_boundary.bounds.Contains(transform.position + transform.forward * m_boundaryDetectionDist))
        {
            cohesionVec += -transform.forward * m_boundaryAvoidanceWeight;
        }

        Vector3 moveVec = Vector3.SmoothDamp(transform.forward, cohesionVec, ref m_currVel, m_smoothDamp);
        moveVec = moveVec.normalized * m_speed;

        Vector3 newPos = transform.position + moveVec * Time.deltaTime;


        transform.forward = moveVec;
        transform.position = newPos;
    }

    private void FindNeighbours()
    {
        m_cohesionNeighbours.Clear();
        FlockUnit[] allNeighbors = m_assignedFlock.m_flockUnits;
        for (int i = 0; i < allNeighbors.Length; ++i)
        {
            FlockUnit curr = allNeighbors[i];
            if (curr == this)
                continue;

            float currNeighborDistanceSqr = Vector3.SqrMagnitude(curr.transform.position - transform.position);
            if (currNeighborDistanceSqr <= m_assignedFlock.m_cohesionDist)
            {
                m_cohesionNeighbours.Add(curr);
            }
        }
    }

    private Vector3 CalculateCohesionVector()
    {
        Vector3 cohesionVec = Vector3.zero;
        if (m_cohesionNeighbours.Count == 0)
            return cohesionVec;

        int neighboursInFov = 0;
        for (int i =0; i < m_cohesionNeighbours.Count; ++i)
        {
            if (CheckInFov(m_cohesionNeighbours[i].transform.position))
            {
                ++neighboursInFov;
                cohesionVec += m_cohesionNeighbours[i].transform.position;
            }
        }

        if (neighboursInFov == 0)
            return cohesionVec;

        cohesionVec = cohesionVec / neighboursInFov;
        cohesionVec -= transform.position;

        return cohesionVec.normalized;
    }

    private bool CheckInFov(Vector3 otherPos)
    {
        return Vector3.Angle(transform.forward, otherPos - transform.position) <= m_fovAngle;
    }
}
