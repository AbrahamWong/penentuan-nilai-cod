using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volumetric500 : GamePourable
{
    // Fill dari beaker (500ml) memiliki rentang nilai 0.04 (500ml) s/d -0.06 (kosong)
    // Fill dari labu ukur (500ml) memiliki rentang nilai 0 (500ml) s/d -0.11 (kosong)
    [SerializeField] private float fillInMililiter;

    void Start()
    {
        // https://answers.unity.com/questions/63317/access-a-child-from-the-parent-or-other-gameobject.html
        // https://answers.unity.com/questions/851056/how-can-i-find-object-childs-child-or-child-in-chi.html
        rend = gameObject.transform.Find("MeshContainer/volumetric_500/Filling").GetComponent<Renderer>();

        normalXAngle = 0.6959127;
        normalZAngle = 0.6959127;
        maxFill = 0f;
        minFill = -0.11f;

        weightContained = EstimateFillInML();
    }

    // Update is called once per frame
    void Update()
    {
        checkParticleInside();

        if (Mathf.Abs(transform.rotation.normalized.x) > normalXAngle || Mathf.Abs(transform.rotation.normalized.z) > normalZAngle)
        {
            if (rend.material.GetFloat("_VolumetricFill") <= minFill) return;
            simulationController.OnPouringInteractable(this, simulationController.GetClosestPourables(this));
        }
        fillInMililiter = EstimateFillInML();
    }

    public override float EstimateFillInML()
    {
        return ((rend.material.GetFloat("_VolumetricFill") - minFill) / (maxFill - minFill)) * 500;
    }

    public override void IncreaseFill(string type)
    {

        if (type.Equals("pour"))
        {
            weightContained += 50;
            rend.material.SetFloat("_VolumetricFill", getFillMaterialPercentage());
        }
        else if (type.Equals("suck"))
        {
            weightContained += 10;
            rend.material.SetFloat("_VolumetricFill", getFillMaterialPercentage());
        }
        else if (type.Equals("bpip"))
        {
            weightContained += 0.01f;
            rend.material.SetFloat("_VolumetricFill", getFillMaterialPercentage());
        }
    }

    public override void ReduceFill(string type)
    {
        if (type.Equals("pour"))
        {
            weightContained -= 50;
            rend.material.SetFloat("_VolumetricFill", getFillMaterialPercentage());
        }
        else if (type.Equals("suck"))
        {
            weightContained -= 10;
            rend.material.SetFloat("_VolumetricFill", getFillMaterialPercentage());
        }
    }

    public float getFillMaterialPercentage()
    {
        float fillPercentage = weightContained / 500;

        Debug.Log(fillPercentage * (maxFill - minFill) + minFill);
        return fillPercentage * (maxFill - minFill) + minFill;
    }

    private void checkParticleInside()
    {
        rend.material.SetColor("_LiquidColor", parseHexToColor(particleContained != "kmno4" ? LiquidColors.NORMAL : LiquidColors.KMNO4_100));
        rend.material.SetColor("_SurfaceColor", parseHexToColor(particleContained != "kmno4" ? SurfaceColors.NORMAL : SurfaceColors.KMNO4_100));
    }
}
