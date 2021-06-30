using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FumeRoom : MonoBehaviour
{
    private SimulationController simulationController;
    private GameObject fumeDoor;

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();

        fumeDoor = transform.Find("Buka Kaca Pembatas/Internal/JointContainer/Joint").gameObject;
        fumeRoomActivated(true);

        // Lakukan delay selama 1 detik.
        StartCoroutine(Delay(0.1f));
    }

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        fumeRoomActivated(false);       // ruang asam secara default terkunci, tidak aktif
    }

    public void FumeButtonTriggered(bool switchedOn)
    {
        fumeRoomActivated(switchedOn);
    }

    public void CheckFumeRoomPrerequisites ()
    {
        GameObject textPrerequisite = new GameObject();
        textPrerequisite.AddComponent<TextMeshPro>();

        // https://docs.microsoft.com/en-us/dotnet/api/system.string.format?view=net-5.0#System_String_Format_System_String_System_Object___
        TextMeshPro tmp = textPrerequisite.GetComponent<TextMeshPro>();
        tmp.text = string.Format("Maaf, untuk membuka lemari asam, anda perlu\n{0}{1}{2}{3}", 
            !simulationController.usingLabCoat          ? "- Menggunakan jas lab\n" : "",
            !simulationController.usingRespirator       ? "- Menggunakan masker\n" : "",
            !simulationController.usingGloves           ? "- Menggunakan sarung tangan\n" : "",
            !simulationController.usingGlasses          ? "- Menggunakan kacamata pelindung" : "");
        tmp.fontSize = 0.5f;

        textPrerequisite.transform.SetParent(transform);
        textPrerequisite.transform.localPosition = new Vector3(-0.32f, 0.17f, 0);

        // https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/
        RectTransform rt = textPrerequisite.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0.75f, 0.5f);
        rt.rotation = Quaternion.Euler(0, 90, 0);

        // Destroy after 1.5 seconds
        Destroy(textPrerequisite, 1.5f);
    }

    private void fumeRoomActivated(bool status)
    {
        switch (status)
        {
            case true:
                fumeDoor.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation |
                    RigidbodyConstraints.FreezePositionX |
                    RigidbodyConstraints.FreezePositionZ;
                break;

            case false:
                fumeDoor.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                break;
        }
    }
}
