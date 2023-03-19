using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDogController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play("Bark");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
