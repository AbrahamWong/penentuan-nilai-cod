using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    private GameObject[] interactables;
    public bool usingLabCoat { get; set; }
    public bool usingGloves { get; set; }
    public bool usingRespirator { get; set; }
    public bool usingGlasses { get; set; }

    private GameObject[] indicators;
    private bool[] indicatorsStatus;

    void Start()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");

        indicators = GameObject.FindGameObjectsWithTag("IndicatorTexts");
        // https://stackoverflow.com/questions/20565894/setting-entire-bool-to-false/20566137
        // deklarasi array akan membuat array of bool dengan default value false
        indicatorsStatus = new bool[5];
    }

    public void setPrerequisiteStatus (int prerequisiteNumber, bool status)
    {
        indicatorsStatus[prerequisiteNumber] = status;
        indicators[prerequisiteNumber].SetActive(status);
    }

    public void OnPouringInteractable(GamePourable pouring, GamePourable poured)
    {
        if (pouring == null || poured == null) return;
        pouring.ReduceFill("pour");
        poured.IncreaseFill("pour");

    }

    public void onSuckingWithPipette(GamePourable pipette, GamePourable container)
    {
        if (pipette == null || container == null) return;
        pipette.IncreaseFill("suck");
        container.ReduceFill("suck");
    }

    public void onPouringWithPipette(GamePourable pipette, GamePourable container)
    {
        if (pipette == null || container == null) return;
        pipette.ReduceFill("suck");
        container.IncreaseFill("suck");
    }

    public void onPouringWithBigPipette(GamePourable pipette, GamePourable container)
    {
        if (pipette == null || container == null) return;
        pipette.ReduceFill("bpip");
        container.IncreaseFill("bpip");
    }

    public GameInteractables GetClosestInteractable(GameInteractables gameInteractables)
    {
        Transform interactingTransform = gameInteractables.transform;
        float distance = 0.6f;
        GameInteractables closest = null;

        foreach (var interactable in interactables)
        {
            float currentDistance = Vector3.Distance(interactingTransform.position, interactable.transform.position);
            float closestX = (interactingTransform.position.x - interactable.transform.position.x);
            float closestY = (interactingTransform.position.y - interactable.transform.position.y);
            float closestZ = (interactingTransform.position.z - interactable.transform.position.z);
            if (currentDistance == 0f)
                continue;
            else if (currentDistance < distance)
            {
                distance = currentDistance;
                Debug.Log("Close: " + interactable + "; " + distance + "\nDistance in xyz: " + closestX + " " + closestY + " " + closestZ);
                closest = interactable.GetComponent<GameInteractables>();
            }
            else {; }
        }

        Debug.Log("Closest: " + closest + " with distance " + distance);
        return closest;
    }

    public GamePourable GetClosestPourables(GameInteractables gameInteractables)
    {
        GamePourable closestPourable;
        Transform interactingTransform = gameInteractables.transform;

        float distance = 0.6f;
        closestPourable = null;

        foreach (var pourable in interactables)
        {
            float currentDistance = Vector3.Distance(interactingTransform.position, pourable.transform.position);
            float closestX = (interactingTransform.position.x - pourable.transform.position.x);
            float closestY = (interactingTransform.position.y - pourable.transform.position.y);
            float closestZ = (interactingTransform.position.z - pourable.transform.position.z);
            if (currentDistance == 0f)
                continue;
            else if (currentDistance < distance && pourable.GetComponent<GamePourable>() != null)
            {
                distance = currentDistance;
                Debug.Log("CloseP: " + pourable + "; " + distance + "\nDistanceP in xyz: " + closestX + " " + closestY + " " + closestZ);
                closestPourable = pourable.GetComponent<GamePourable>();
            }
            else {; }
        }

        Debug.Log("ClosestP: " + closestPourable + " with distanceP " + distance);
        return closestPourable;
    }

    public GamePourable GetClosestPourables(Transform transform)
    {
        float distance = 0.6f;
        GamePourable closestPourable = null;

        foreach (var pourable in interactables)
        {
            float currentDistance = Vector3.Distance(transform.position, pourable.transform.position);
            float closestX = (transform.position.x - pourable.transform.position.x);
            float closestY = (transform.position.y - pourable.transform.position.y);
            float closestZ = (transform.position.z - pourable.transform.position.z);
            if (currentDistance == 0f)
                continue;
            else if (currentDistance < distance && pourable.GetComponent<GamePourable>() != null)
            {
                distance = currentDistance;
                Debug.Log("ClosePT: " + pourable + "; " + distance + "\nDistancePT in xyz: " + closestX + " " + closestY + " " + closestZ);
                closestPourable = pourable.GetComponent<GamePourable>();
            }
            else {; }
        }

        Debug.Log("ClosestPT: " + closestPourable + " with distancePT " + distance);
        return closestPourable;
    }

    public void RefillPourable(GamePourable pourable)
    {
        pourable.IncreaseFill("pour");
    }

    protected WatchGlass glass;
    public WatchGlass getWatchGlass()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

        foreach (var interactable in interactables)
        {
            if (interactable.name == "Lab Watch Glass")
            {
                glass = interactable.GetComponent<WatchGlass>();

                return glass;
            }
        }

        return null;
    }
}
