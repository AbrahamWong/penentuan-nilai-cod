using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GameInteractables : MonoBehaviour
{
    protected SimulationController simulationController;
    [SerializeField]
    protected Renderer rend;
    protected double normalXAngle = 0;
    protected double normalZAngle = 0;

    protected bool isFull, isEmpty = false;
    protected float maxFill, minFill;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    abstract public float EstimateFillInML();
    abstract public void ReduceFill(string type);
    abstract public void IncreaseFill(string type);
}
