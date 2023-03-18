using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoProperties : MonoBehaviour
{
    public RawImage m_RawImage;
    private GameObject animal;
    private float score;


    public void SetAnimal(GameObject animal, float rayHitProportion)
    {
        this.animal = animal;
        if (animal == null)
        {
            this.score = 0;
            return;
        }

        // scale this based on animal properties i guess?
        // ideas: rarity, how much of the frame contains the animal, etc.
        this.score = rayHitProportion;
    }

    public void SetImage(Texture2D image)
    {
        m_RawImage.texture = image;
    }
}
