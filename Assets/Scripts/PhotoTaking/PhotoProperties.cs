using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoProperties : MonoBehaviour
{
    public RawImage m_RawImage;
    private Animal_Behaviour animal;
    private float score;

    private Dictionary<AnimalState, float> poseScores = new Dictionary<AnimalState, float>() {
        {AnimalState.Walking, 1.0f},
        {AnimalState.Standing, 2.0f},
        {AnimalState.Running, 1.5f},
        {AnimalState.Attacking, 5.0f},
        {AnimalState.Dead, 0.1f} 
    };


    public void SetAnimal(GameObject animal, float rayHitProportion, float distance, float imageSize)
    {
        if (animal == null)
        {
            this.score = 0;
            return;
        }

        this.animal = animal.GetComponent<Animal_Behaviour>();
        float baseScore = 10.0f;
        AnimalState animalState = this.animal.GetAnimalState();

        // scale this based on animal properties i guess?
        // ideas: rarity, how much of the frame contains the animal, etc.
        this.score = rayHitProportion * distance * imageSize * baseScore;
    }

    public void SetImage(Texture2D image)
    {
        m_RawImage.texture = image;
    }
}
