using UnityEngine;
using TMPro;

public class Burette : GamePourable
{
    private GamePourable pourableUnder;
    private bool triggerKnob;

    private float originalWeight, weight;

    // Start is called before the first frame update
    protected override void Start()
    {
        rend = transform.Find("MeshContainer/buret/buret_isi").GetComponent<Renderer>();

        normalXAngle = 1;
        normalZAngle = 1;
        maxFill = 0.15f;
        minFill = -0.246f;

        weightContained = 0;
        capacity = 50;      // 50ml
        
        base.Start();
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
        
        text.fontSize = 0.4f;
        text.alignment = TextAlignmentOptions.Bottom;
        text.text = "0 ml";

        // https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/
        RectTransform rt = transform.Find("Teks Sampel").GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0.2f, 0.1f);
        rt.localPosition = new Vector3(0, -0.04f, -0.14f);
        rt.localRotation = Quaternion.Euler(0, 90, 0);

        colorCoroutine = StartCoroutine(coroutineCheckParticle());
        Debug.Log("Burette: colorCoroutine == null? " + (colorCoroutine == null) + " but why?\nrend == null?");
    }

    private void Update()
    {
        if (triggerKnob)
        {
            if (weightContained > 0f)
            {
                simulationController.onTitrate(this, pourableUnder);
                pourableUnder.GetComponent<Erlenmeyer300>().startLerpingUp();
            }

            weight = weightContained;
            text.text = (Mathf.Round((originalWeight - weight) * 100) / 100).ToString() + " ml";
        }
    }

    // Proxy StartTriggerAction
    // private new void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Burette Trigger: test enter"); 
    //     StartTriggerAction();
    // }
    // private new void OnTriggerExit(Collider other) => StopTriggerAction();

    public override void StartTriggerAction()
    {
        base.StartTriggerAction();

        // start titration
        pourableUnder = simulationController.GetClosestPourables(transform);
        if (pourableUnder != null && pourableUnder.GetType() == typeof(Erlenmeyer300))
        {
            triggerKnob = true;
            originalWeight = weight = weightContained;
        }
    }

    public override void StopTriggerAction()
    {
        base.StopTriggerAction();

        // stop titration
        triggerKnob = false;
    }
}
