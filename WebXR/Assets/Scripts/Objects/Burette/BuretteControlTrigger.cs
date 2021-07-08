using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuretteControlTrigger : GameInteractables
{
    private Burette burette;
    private GamePourable pourableUnder;
    private bool triggerKnob;

    private float originalWeight, weight;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        burette = GetComponentInParent<Burette>();
        GameObject textObject = transform.GetChild(0).gameObject;

        text.fontSize = 0.4f;
        text.alignment = TextAlignmentOptions.Center;
        text.text = "0 ml";

        // https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/
        RectTransform rt = textObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0.5f, 0.125f);
        rt.localPosition = new Vector3(0, 0.1f, 0);
        rt.localRotation = Quaternion.Euler(0, 90, 90);
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerKnob)
        {
            if (burette.getWeightContained() > 0f)
            {
                simulationController.onTitrate(burette, pourableUnder);
                pourableUnder.GetComponent<Erlenmeyer300>().startLerpingUp();
            }

            weight = burette.getWeightContained();
            text.text = (Mathf.Round((originalWeight - weight) * 100) / 100).ToString() + " ml";
        }
    }

    // Proxy StartTriggerAction
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Burette Trigger: test enter"); 
        StartTriggerAction();
    }
    private void OnTriggerExit(Collider other) => StopTriggerAction();

    public override void StartTriggerAction()
    {
        base.StartTriggerAction();

        // start titration
        pourableUnder = simulationController.GetClosestPourables(transform);
        if (pourableUnder.GetType() == typeof(Erlenmeyer300))
        {
            triggerKnob = true;
            originalWeight = weight = burette.getWeightContained();
        }
    }

    public override void StopTriggerAction()
    {
        base.StopTriggerAction();

        // stop titration
        triggerKnob = false;
    }
}
