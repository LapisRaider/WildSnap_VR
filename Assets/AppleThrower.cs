using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleThrower : MonoBehaviour
{
    public float initialVelocity;
    public GameObject apple;
    public GameObject aimIndicator;
    public int numIndicator; 
    public float aimTimeStep;

    private bool isAiming = false;
    private GameObject[] aimIndicators;

    void Start()
    {
        aimIndicators = new GameObject[numIndicator];
        for (int i = 0; i < numIndicator; i++)
        {
            aimIndicators[i] = Instantiate(aimIndicator);
            aimIndicators[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetButtonDown("Fire1"))
        {
            StartAiming();
        }

        if (isAiming)
        {
            DrawAimingLine();
        }
        
        if (Input.GetButtonUp("Fire1") && isAiming)
        {
            StopAiming();
            ThrowApple();
        }
    }

    void ThrowApple()
    {
        GameObject thrownApple = Instantiate(apple, transform);
        Rigidbody rb = thrownApple.GetComponent<Rigidbody>();
        Vector3 aimingVector = Camera.main.transform.forward;
        rb.velocity = aimingVector * initialVelocity;
    }

    void StartAiming()
    {
        isAiming = true;
        for (int i = 0; i < numIndicator; i++)
        {
            aimIndicators[i].SetActive(true);
        }
    }

    void StopAiming()
    {
        isAiming = false;
        for (int i = 0; i < numIndicator; i++)
        {
            aimIndicators[i].SetActive(false);
        }
    }

    void DrawAimingLine()
    {
        Vector3 aimingVector = Camera.main.transform.forward;
        Vector3 position = transform.position;
        Vector3 velocity = aimingVector * initialVelocity;
        float time = 0;

        for (int i = 0; i < numIndicator; i++)
        {
            time += aimTimeStep;
            // s = ut + 1/2 at^2
            float yDisplacement = velocity.y * time + 0.5f * Physics.gravity.y * time * time;
            float xDisplacement = velocity.x * time;
            float zDisplacement = velocity.z * time;
            aimIndicators[i].transform.position = position + new Vector3(xDisplacement, yDisplacement, zDisplacement);
        }
    }
}
