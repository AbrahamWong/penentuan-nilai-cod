using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GamePourable : GameInteractables
{
    protected Renderer rend;
    protected double normalXAngle = 0;
    protected double normalZAngle = 0;
    protected float maxFill, minFill;
    [SerializeField] protected float liquidSpeed = 0.0003f;

    abstract public float EstimateFillInML();
    abstract public void ReduceFill(string type);
    abstract public void IncreaseFill(string type);


}
