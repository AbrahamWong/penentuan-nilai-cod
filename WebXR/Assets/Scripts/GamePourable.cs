using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GamePourable : GameInteractables
{
    protected Renderer rend;
    protected string rendererFillReference;
    protected double normalXAngle = 0;
    protected double normalZAngle = 0;
    protected float capacity;
    protected float maxFill, minFill;
    protected Coroutine colorCoroutine;

    public bool isFull;

    new protected virtual void Start()
    {
        base.Start();
        isFull = weightContained >= capacity ? true : false;
        rendererFillReference = "_Fill";

        colorCoroutine = StartCoroutine(coroutineCheckParticle());
    }

    protected IEnumerator coroutineCheckParticle()
    {
        while (!simulationController.isExperimentDone())
        {
            foreach (string particle in particleContained)
            {
                if (particle.Equals("kmno4"))
                {
                    rend.material.SetColor("_LiquidColor", parseHexToColor(LiquidColors.KMNO4_100));
                    rend.material.SetColor("_SurfaceColor", parseHexToColor(SurfaceColors.KMNO4_100));
                    break;
                }
                else
                {
                    rend.material.SetColor("_LiquidColor", parseHexToColor(LiquidColors.NORMAL));
                    rend.material.SetColor("_SurfaceColor", parseHexToColor(SurfaceColors.NORMAL));

                }
            }

            yield return new WaitForSeconds(.1f);
        }

    }

    public virtual float EstimateFillInML()
    { 
        return rend == null ? 0 : ((rend.material.GetFloat(rendererFillReference) - minFill) / (maxFill - minFill)) * capacity;
    }

    public virtual float getFillMaterialPercentage()
    {
        float fillPercentage = weightContained / capacity;

        return fillPercentage * (maxFill - minFill) + minFill;
    }

    public virtual void ReduceFill(string type)
    {
        if (isFull) isFull = false;
        if (type.Equals("pour"))
        {
            weightContained -= LiquidTransferSpeed.pouringSpeed;
        }
        else if (type.Equals("suck"))
        {
            weightContained -= LiquidTransferSpeed.pipetteSpeed;
        }
        if (type.Equals("bpip") && weightContained > 0f) // Volume Pipette decreasing value
        {
            weightContained -= LiquidTransferSpeed.bigPipetteSpeed;
        }
        else if (type.Equals("burr"))
        {
            weightContained -= LiquidTransferSpeed.buretteSpeed;
        }

        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
    }

    public virtual void IncreaseFill(string type)
    {
        if (type.Equals("pour"))
        {
            weightContained += LiquidTransferSpeed.pouringSpeed;
        }
        else if (type.Equals("suck"))
        {
            weightContained += LiquidTransferSpeed.pipetteSpeed;
        }
        if (type.Equals("bpip") && weightContained < capacity)
        {
            weightContained += LiquidTransferSpeed.bigPipetteSpeed * (gameObject.GetComponent<VolumePipette>() != null ? 0.02f : 1);
        }
        else if (type.Equals("burr"))
        {
            weightContained += LiquidTransferSpeed.buretteSpeed;
        }

        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
        if (weightContained >= capacity) isFull = true;
    }
}
