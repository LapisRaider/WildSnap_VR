using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleManager : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        transform.position += velocity * Time.deltaTime;
    }

    public void SetVelocity(float x, float y, float z) {
        velocity = new Vector3(x, y, z);
    }
}
