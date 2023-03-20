using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PhotoCalculator
{
    //TODO:: PUT THIS SOMEWHERE ELSE
    public Dictionary<AnimalState, float> poseScores = new Dictionary<AnimalState, float>() {
        {AnimalState.Walking, 1.0f},
        {AnimalState.Standing, 2.0f},
        {AnimalState.Running, 1.5f},
        {AnimalState.Attacking, 5.0f},
        {AnimalState.Dead, 0.1f} 
    };
}
