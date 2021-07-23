using System.Collections;
using UnityEngine;

public class Funnel : GameInteractables
{
    [SerializeField] private GameInteractables attachedTo;
    private Rigidbody rb;
    [SerializeField] private bool funnelLock = false, isAllowedToGrip = false;

    public GameInteractables getAttachedIntearctable() => attachedTo;

    protected override void Start()
    {
        base.Start();
        text.text = "";
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (attachedTo != null)
            transform.rotation = attachedTo.transform.rotation;
    }

    private void OnTriggerEnter(Collider other) 
        => getFunnelTrigger(other);
    

    public void getFunnelTrigger (Collider other)
    {
        // Berarti interaksi dengan objek yang bukan GameInteractables
        // Misal: meja.
        if (other.gameObject.GetComponent<GameInteractables>() == null)
        {
            if (other.name.Equals("InputModel"))
            {
                WebXR.WebXRController controller = other.transform.parent.GetComponent<WebXR.WebXRController>();
                isAllowedToGrip = controller.GetAxis(WebXR.WebXRController.AxisTypes.Grip) <= 0.2 ? true : false;
            }
            return;
        }

        // Cek collider yang mana yang menyentuh corong
        switch (other.GetComponent<GameInteractables>().GetType().ToString())
        {
            case "WatchGlass":
                WatchGlass wg = simulationController.getWatchGlass();

                ArrayList particle = new ArrayList();
                particle.Add(wg.particleInside);
                attachedTo.setParticleContained(particle);

                attachedTo.GetComponent<GamePourable>().addOffsetWeight(wg.getWeightContained());
                StartCoroutine(simulationController.updateInteractableText(attachedTo));
                wg.emptyChemicals();
                break;

            case "Volumetric500":
                if (funnelLock) break;

                GameObject volumetric500 = other.gameObject;

                attachedTo = volumetric500.GetComponent<GameInteractables>();
                transform.position = volumetric500.transform.position + new Vector3(0, 0.2f, 0);
                transform.rotation = new Quaternion(0, 0, 0, 1);
                hingeFromObject(volumetric500);

                funnelLock = true;
                break;

            case "Burette":
                if (funnelLock) break;

                GameObject burette = other.gameObject;

                attachedTo = burette.GetComponent<GameInteractables>();
                transform.position = burette.transform.position + new Vector3(0, 0.3f, 0);
                hingeToObject(burette);

                funnelLock = true;
                break;

            default:
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.GetComponent<WebXR.WebXRController>() == null) return;
        
        WebXR.WebXRController controller = other.transform.parent.GetComponent<WebXR.WebXRController>();
        if (controller != null && controller.GetAxis(WebXR.WebXRController.AxisTypes.Grip) >= 0.85 
            && attachedTo != null && isAllowedToGrip)
        {
            unhingeFromObject(attachedTo.gameObject);
            attachedTo = null;
            funnelLock = false;
        }
    }

    // https://forum.unity.com/threads/stick-object-to-moving-object.127988/
    private void hingeToObject (GameObject hinge)
    {
        HingeJoint hj = gameObject.AddComponent<HingeJoint>();
        hj.connectedBody = hinge.GetComponent<Rigidbody>();
        rb.mass = 0.00001f;
        gameObject.GetComponent<Collider>().material.bounciness = 0;
        rb.freezeRotation = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void hingeFromObject (GameObject hinge)
    {
        HingeJoint hj = hinge.AddComponent<HingeJoint>();
        hj.connectedBody = rb;

        FixedJoint fj = gameObject.AddComponent<FixedJoint>();
        fj.connectedBody = hinge.GetComponent<Rigidbody>();

        rb.mass = 0.00001f;
        gameObject.GetComponent<Collider>().material.bounciness = 0;
        rb.freezeRotation = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void unhingeFromObject(GameObject hinge)
    {
        Destroy(hinge.GetComponent<HingeJoint>());
        Destroy(GetComponent<HingeJoint>());
        Destroy(GetComponent<FixedJoint>());
        rb.mass = 1;
        rb.freezeRotation = false;
    }
}
