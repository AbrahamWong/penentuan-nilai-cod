using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    const int MAX_VALUE = 42_069;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPouringInteractable(GameObject pouring, GameObject poured)
    {
        if (pouring == null || poured == null) return;

        Debug.Log(pouring.name + " menuangkan cairan kepada " + poured.name);
    }

    public GameObject GetClosestInteractable(GameObject gameObject)
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        Transform interactingTransform = gameObject.transform;

        Vector3 distance = new Vector3(MAX_VALUE, MAX_VALUE, MAX_VALUE);
        GameObject closest = new GameObject();

        foreach (var interactable in interactables)
        {
            Vector3 currentDistance = interactingTransform.position - interactable.transform.position;
            if (currentDistance.Equals(new Vector3(0, 0, 0)))
                continue;
            else if (Vector3.Min(currentDistance, distance) == currentDistance)
            {
                distance = currentDistance;
                closest = interactable;
            }
            else {; }
        }

        return closest;
    }
}
