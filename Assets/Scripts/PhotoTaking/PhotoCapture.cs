using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCapture : MonoBehaviour
{
    private Camera m_photoTakingCamera;

    // Start is called before the first frame update
    void Awake()
    {
        m_photoTakingCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //take photo
        }
    }
}
