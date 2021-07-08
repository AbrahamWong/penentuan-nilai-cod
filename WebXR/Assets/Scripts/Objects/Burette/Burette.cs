using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burette : GamePourable
{
    [SerializeField] private bool readyTitration = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        rend = gameObject.transform.Find("buret_isi").GetComponent<Renderer>();

        normalXAngle = 1;
        normalZAngle = 1;
        maxFill = 0.5f;
        minFill = 0.104f;
        capacity = 50;      // 50ml

        base.Start();
        weightContained = 0;
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());

        text.text = "";
    }
}
