using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebXR;
using Zinnia.Action;

// Change from MonoBehavior to BooleanAction from Zinnia.Action
public class BooleanTrigger : BooleanAction
{
    public WebXRController controller;

    // Update is called once per frame
    void Update()
    {
        Receive(controller.GetButton(WebXRController.ButtonTypes.Trigger));
    }
}
