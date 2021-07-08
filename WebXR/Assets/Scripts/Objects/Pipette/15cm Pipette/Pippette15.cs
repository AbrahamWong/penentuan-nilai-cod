using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pippette15 : GamePourable
{
    [SerializeField] private float fillInMililiter;
    // [SerializeField] private bool hasFill = false;
    [SerializeField] private bool canUse = false;
    [SerializeField] private string tempParticle;
    private GamePourable pourableOnContact;

    public void setUsability(bool status) => canUse = status;
    public bool getUsability() => canUse;

    public void setTempParticle(string particle) => tempParticle = particle;
    public string getTempParticle() => tempParticle;
    public void setPourableOnContact(GamePourable pourable) => pourableOnContact = pourable;

    
    // Start is called before the first frame update
    protected override void Start()
    {
        rend = gameObject.transform.Find("MeshContainer/pipet_15_cm/Isi").GetComponent<Renderer>();

        maxFill = 0.15f;
        minFill = -0.15f;
        capacity = 5;  // 5ml

        base.Start();
        rend.material.SetFloat(rendererFillReference, minFill);
        weightContained = EstimateFillInML();
    }

    private void Update()
    {
        fillInMililiter = EstimateFillInML();
    }

    public override void StartTriggerAction()
    {
        base.StartTriggerAction();
        if (isFull && canUse)
        {
            simulationController.onPouringWithPipette(this, pourableOnContact);
            particleContained.Clear();
        }
    }

    public override void StopTriggerAction()
    {
        base.StopTriggerAction();
        if (!isFull && canUse)
        {
            simulationController.onSuckingWithPipette(this, pourableOnContact);
            particleContained.Add(tempParticle);
            tempParticle = "";
        }
    }
}
