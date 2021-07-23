using System.Collections;
using UnityEngine;

abstract public class GamePourable : GameInteractables
{
    protected Renderer rend;
    protected string rendererFillReference;
    protected double normalXAngle = 0, normalZAngle = 0;
    protected float capacity;
    protected float maxFill, minFill;
    protected Coroutine colorCoroutine;

    public bool isFull;

    [SerializeField] protected float offsetWeight = 0;
    public void setOffsetWeight(float w) => offsetWeight = w;
    public void addOffsetWeight(float w) => offsetWeight += w;
    public float getOffsetWeight() => offsetWeight;

    new protected virtual void Start()
    {
        base.Start();
        isFull = weightContained >= capacity ? true : false;
        rendererFillReference = "_Fill";

        colorCoroutine = StartCoroutine(coroutineCheckParticle());
    }


    protected GamePourable pouredObject;
    protected bool okToPour = false;
    protected virtual void OnTriggerEnter(Collider other)
    {
        pouredObject = other.GetComponent<GamePourable>();
        if (pouredObject != null) okToPour = true;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        pouredObject = null; okToPour = false;
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

            yield return new WaitForSeconds(.05f);
        }

    }

    public virtual float EstimateFillInML() => rend == null ? 
        0 : 
        ((rend.material.GetFloat(rendererFillReference) - minFill) / (maxFill - minFill)) * capacity;

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
            weightContained -= LiquidTransferSpeed.bigPipetteSpeed * 0.25f;
        }
        else if (type.Equals("burr"))
        {
            weightContained -= LiquidTransferSpeed.buretteSpeed;
        }
        else if (type.Equals("empty"))
        {
            weightContained = 0;
            offsetWeight = 0;
        }

        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
        StartCoroutine(simulationController.updateInteractableText(this));
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
            weightContained += LiquidTransferSpeed.bigPipetteSpeed * (gameObject.GetComponent<VolumePipette>() != null ? 0.02f : 0.25f);
            if (gameObject.GetComponent<VolumePipette>() == null) offsetWeight += LiquidTransferSpeed.bigPipetteSpeed * 0.25f;
        }
        if (type.Equals("bpit") && weightContained < capacity)
        {
            weightContained += LiquidTransferSpeed.bigPipetteSpeed * 0.5f;
        }
        else if (type.Equals("burr"))
        {
            weightContained += LiquidTransferSpeed.buretteSpeed;
        }

        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
        if (weightContained >= capacity) isFull = true;
        StartCoroutine(simulationController.updateInteractableText(this));
    }
}
