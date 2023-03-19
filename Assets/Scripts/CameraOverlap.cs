using UnityEngine;

public class CameraOverlap : MonoBehaviour
{
    public string tagToSearch = "Animal"; // the tag of the game object to search for
    public float maxDistance = 100f; // the maximum distance of the raycast
    public float coneAngle = 30f; // the angle of the cone in degrees
    private RaycastHit hit;

    void Update()
    {
        Vector3 origin = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 direction = rotation * Vector3.forward;
        float halfAngle = coneAngle / 2f;
        float coneRadius = Mathf.Tan(halfAngle * Mathf.Deg2Rad) * maxDistance;

        Collider[] hits = Physics.OverlapSphere(origin, coneRadius);
        float closestDistance = Mathf.Infinity;
        GameObject closestObject = null;

        foreach (Collider hitCollider in hits)
        {
            if (hitCollider.CompareTag(tagToSearch))
            {
                Vector3 hitPoint = hitCollider.ClosestPoint(origin);
                Vector3 hitDirection = (hitPoint - origin).normalized;
                float angle = Vector3.Angle(direction, hitDirection);
                if (angle < halfAngle)
                {
                    float distance = Vector3.Distance(origin, hitPoint);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestObject = hitCollider.gameObject;
                    }
                }
            }
        }

        if (closestObject != null)
        {
            // do something with the closest game object with the specified tag
            Debug.Log("Closest object with tag " + tagToSearch + " is " + closestObject.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 direction = rotation * Vector3.forward;
        float halfAngle = coneAngle / 2f;
        float coneRadius = Mathf.Tan(halfAngle * Mathf.Deg2Rad) * maxDistance;
        Gizmos.DrawWireSphere(origin, coneRadius);
        Gizmos.DrawLine(origin, origin + direction * maxDistance);
        Gizmos.DrawLine(origin, origin + Quaternion.Euler(0f, halfAngle, 0f) * direction * maxDistance);
        Gizmos.DrawLine(origin, origin + Quaternion.Euler(0f, -halfAngle, 0f) * direction * maxDistance);
    }
}