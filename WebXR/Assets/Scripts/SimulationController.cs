using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    private GameObject[] interactables;

    // Start is called before the first frame update
    void Start()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPouringInteractable(GameInteractables pouring, GameInteractables poured)
    {
        if (pouring == null || poured == null) return;

        // Debug.Log(pouring.name + " menuangkan cairan kepada " + poured.name);
        pouring.ReduceFill("pour");
        poured.IncreaseFill("pour");

    }

    public void onSuckingWithPipetteInteractable(GameInteractables pipette, GameInteractables container)
    {
        if (pipette == null || container == null) return;

        // Debug.Log(pipette.name + " mengambil cairan dari " + container.name);
        pipette.IncreaseFill("suck");
        container.ReduceFill("suck");
    }

    public void onPouringWithPipetteInteractable(GameInteractables pipette, GameInteractables container)
    {
        if (pipette == null || container == null) return;

        // Debug.Log(pipette.name + " menuangkan cairan dari pipet ke " + container.name);
        pipette.ReduceFill("suck");
        container.IncreaseFill("suck");
    }

    public GameInteractables GetClosestInteractable(GameInteractables gameInteractables)
    {
        Transform interactingTransform = gameInteractables.transform;

        float distance = 0.6f;

        // Tidak bisa menginisiasi dengan new GameInteractables karena GameInteractables         
        // implements MonoBehavior, dan sepertinya segala sesuatu yang implement 
        // MonoBehavior tidak boleh dibuat menggunakan new Class().
        GameObject gameObject = new GameObject();
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
        Destroy(gameObject);
        return closest;
    }
}
