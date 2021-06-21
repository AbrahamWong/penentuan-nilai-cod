using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GameInteractables : MonoBehaviour
{
    protected SimulationController simulationController;
    protected float weightContained = 0f;

    public float getWeightContained()
    {
        return weightContained;
    }

    public void setWeightContained(float weight)
    {
        weightContained = weight;
    }
}
