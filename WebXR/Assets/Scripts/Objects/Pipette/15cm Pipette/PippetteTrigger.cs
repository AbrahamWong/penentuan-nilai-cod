using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PippetteTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponentInParent<Pippette15>().CallTriggerEnterFromChild(other);
    }
}
