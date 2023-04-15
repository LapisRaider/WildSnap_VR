using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public GameObject eatingParticles;

    void Start()
    {
        eatingParticles.SetActive(false);
    }

    public void Eat()
    {
        eatingParticles.SetActive(true);
    }
}
