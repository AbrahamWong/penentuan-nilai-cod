using UnityEngine;

public class Pippette15 : GamePourable
{
    // [SerializeField] private bool hasFill = false;
    [SerializeField] private bool canUse = false;
    [SerializeField] private string tempParticle;
    private GamePourable pourableOnContact;

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

    // Falloff
    public int count = 0;
    private new void OnTriggerEnter(Collider other)
    {
        GameInteractables interactables = other.GetComponent<GameInteractables>();
        Debug.Log("Pipette: other.name => " + other.name + ", interactable isNull? " + (interactables == null));
        if (interactables == null) return;

        Debug.Log("Pipette: interactable => " + interactables.name);

        // Real function
        tempParticle = interactables.getParticleContained() != null && interactables.getParticleContained().Count > 0 
            ? interactables.getParticleContained()[0].ToString() : "";
        canUse = true;
        if (other.GetComponent<GamePourable>() != null) pourableOnContact = other.GetComponent<GamePourable>();

        // // Falloff func
        // GamePourable pourable = other.GetComponent<GamePourable>();
        // Debug.Log("Pipette: pourable => " + pourable.name);
        // if (pourable != null)
        // {
        //     setPourableOnContact(pourable);
        //     switch (count % 2)
        //     {
        //         case 0:
        //             StopTriggerAction();
        //             break;
        // 
        //         case 1:
        //             StartTriggerAction();
        //             break;
        //     }
        //     count++;
        // }
    }

    private new void OnTriggerExit(Collider other)
    {
        setPourableOnContact(null);
        canUse = false;
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
            particleContained.Add(tempParticle);
            tempParticle = "";
            simulationController.onSuckingWithPipette(this, pourableOnContact);
        }
    }
}
