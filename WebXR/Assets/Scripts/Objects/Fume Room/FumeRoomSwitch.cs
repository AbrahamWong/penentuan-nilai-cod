using UnityEngine;

public class FumeRoomSwitch : MonoBehaviour
{
    private FumeRoom fumeRoom;
    private GameObject switchMesh;
    private Material mat;

    private Color emissionColor = new Color();
    [SerializeField] private bool switchedOn = false;

    private SimulationController simulationController;

    void Start()
    {
        fumeRoom = transform.parent.parent.gameObject.GetComponent<FumeRoom>();
        switchMesh = transform.Find("MeshContainer/Saklar").gameObject;
        
        mat = switchMesh.GetComponent<MeshRenderer>().material;
        emissionColor = mat.GetColor("_EmissionColor");

        fumeRoomActivated(switchedOn);
        
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (simulationController.usingLabCoat && 
            simulationController.usingRespirator && 
            simulationController.usingGlasses && 
            simulationController.usingGloves)
        {
            switchedOn = true;

            fumeRoom.FumeButtonTriggered(switchedOn);
            fumeRoomActivated(switchedOn);
        }
        else
        {
            fumeRoom.CheckFumeRoomPrerequisites();
        }

    }

    private void fumeRoomActivated(bool status)
    {
        // https://forum.unity.com/threads/disable-and-enable-emission-property-of-a-material-via-script.445016/

        switch (status)
        {
            case true:
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.AnyEmissive;
                mat.SetColor("_EmissionColor", emissionColor);
                break;

            case false:
                mat.DisableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                mat.SetColor("_EmissionColor", Color.black);
                break;
        }
    }
}
