using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebXR;

public class SimulationController : MonoBehaviour
{
    [SerializeField] private GameObject[] interactables;

    // gedebug
    [SerializeField] private List<GamePourable> pourables;
    public bool usingLabCoat { get; set; }
    public bool usingGloves { get; set; }
    public bool usingRespirator { get; set; }
    public bool usingGlasses { get; set; }

    [SerializeField] private GameObject[] indicators;
    [SerializeField] private bool[] indicatorsStatus;

    private WebXRController leftController;
    private WebXRController rightController;

    void Start()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");

        foreach (GameObject interactable in interactables)
        {
            if (interactable.GetComponent<GamePourable>() != null) pourables.Add(interactable.GetComponent<GamePourable>());
        }

        indicators = GameObject.FindGameObjectsWithTag("IndicatorTexts");
        foreach (GameObject indicator in indicators) indicator.SetActive(false);

        // https://stackoverflow.com/questions/20565894/setting-entire-bool-to-false/20566137
        // deklarasi array akan membuat array of bool dengan default value false
        indicatorsStatus = new bool[indicators.Length];

        StartCoroutine(updateText());

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("VRController");
        leftController = gameObjects[0].name == "handL" ? gameObjects[0].GetComponent<WebXRController>() : null;
        rightController = gameObjects[1].name == "handR" ? gameObjects[1].GetComponent<WebXRController>() : null;

    }


    private void Update()
    {
        // Test hanya bisa di VR hardware
        if (leftController.GetButtonDown(WebXRController.ButtonTypes.Trigger))
        {
            GamePourable p = GetClosestPourables(leftController.transform);
            if (p != null) p.StartTriggerAction();
        } 
        else if (rightController.GetButtonDown(WebXRController.ButtonTypes.Trigger))
        {
            GamePourable p = GetClosestPourables(rightController.transform);
            if (p != null) p.StartTriggerAction();
        }

        // Test hanya bisa di VR hardware
        if (leftController.GetButtonUp(WebXRController.ButtonTypes.Trigger))
        {
            GamePourable p = GetClosestPourables(leftController.transform);
            if (p != null) p.StopTriggerAction();
        } 
        else if (rightController.GetButtonUp(WebXRController.ButtonTypes.Trigger))
        {
            GamePourable p = GetClosestPourables(rightController.transform);
            if (p != null) p.StopTriggerAction();
        }

        // Test hanya bisa di VR hardware
        if (leftController.GetButtonDown(WebXRController.ButtonTypes.Grip)
            || rightController.GetButtonDown(WebXRController.ButtonTypes.Grip))
        {
            GameInteractables leftInteractable = GetClosestInteractable(leftController.transform);
            GameInteractables rightInteractable = GetClosestInteractable(rightController.transform);

            if (leftInteractable != null && leftInteractable.getTemperature() > 41 && !usingGloves)
            {
                leftInteractable.gameObject.GetComponent<FixedJoint>().connectedBody = null;
            }
            else if (rightInteractable != null && rightInteractable.getTemperature() > 41 && !usingGloves)
            {
                rightInteractable.gameObject.GetComponent<FixedJoint>().connectedBody = null;
            }
        }
    }

    public void setPrerequisiteStatus (int prerequisiteNumber, bool status)
    {
        indicatorsStatus[prerequisiteNumber] = status;
        indicators[prerequisiteNumber].SetActive(status);
        Debug.Log("Prerequisite number " + prerequisiteNumber + " is now " + status);
    }

    public bool getPrerequisiteStatus(int prerequisiteNumber) => indicatorsStatus[prerequisiteNumber];

    public bool isExperimentDone()
    {
        // https://stackoverflow.com/questions/34214167/if-all-elements-in-bool-array-are-true
        bool done = true;
        foreach (var indicatorStatus in indicatorsStatus)
        {
            if (!indicatorStatus) { done = false; break; }
        }

        return done;
    }

    IEnumerator delayBeforeQuit()
    {
        yield return new WaitForSeconds(4f);
        Debug.Log("Simulation Controller: App called to Quit");
        // Application.Quit();

        // https://answers.unity.com/questions/899037/applicationquit-not-working-1.html
        #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    IEnumerator updateText()
    {
        yield return new WaitForSeconds(.1f);
        while (!isExperimentDone())
        {
            foreach (GameObject item in interactables)
            {
                if (item == null) 
                {
                    // https://answers.unity.com/questions/1074164/remove-element-from-array.html
                    int index = System.Array.IndexOf(interactables, item);
                    
                    for (int a = index; a < interactables.Length - 1; a++)
                    {
                        // moving elements downwards, to fill the gap at [index]
                        interactables[a] = interactables[a + 1];
                    }
                    
                    // finally, let's decrement Array's size by one
                    System.Array.Resize(ref interactables, interactables.Length - 1);
                    yield return null;
                    continue;
                }

                GameInteractables interactable = item.GetComponent<GameInteractables>();
                // Debug.Log("Simulation Controller: debugName=" + (interactable == null? "true" : interactable.name));
                if (interactable == null
                    || interactable.GetType() == typeof(LabSafetyTrigger)
                    || interactable.GetType() == typeof(BuretteControlTrigger)
                    || interactable.GetType() == typeof(Funnel)
                    || interactable.GetType() == typeof(Burette)
                    || interactable.GetType() == typeof(ElectricHeaterTrigger)
                    || interactable.GetType() == typeof(WatchGlass)
                    || interactable.GetType() == typeof(VolumePipette)) continue;

                interactable.setInteractableText(
                    "Massa ditampung = " + (Mathf.Round(interactable.getWeightContained() * 100) / 100).ToString() +
                    "\nTemperatur = " + interactable.getTemperature().ToString() +
                    "\nMenampung " + interactable.getParticleInString()
                );
            }

            yield return null;
        }

        Debug.Log("Simulation Controller: Experiment done");
        StartCoroutine(delayBeforeQuit());
    }

    public void OnPouringInteractable(GamePourable pouring, GamePourable poured)
    {
        if (pouring == null || poured == null || poured.isFull || poured.GetComponent<Pippette15>() != null) return;

        changeReceiverParticleContained(pouring, poured);
        if (poured.getWeightContained() < 0.1f) poured.setTemperature(pouring.getTemperature());
        pouring.ReduceFill("pour");
        poured.IncreaseFill("pour");

    }

    public void onSuckingWithPipette(GamePourable pipette, GamePourable container)
    {
        if (pipette == null || container == null || pipette.isFull) return;

        changeReceiverParticleContained(container, pipette);
        if (pipette.getWeightContained() < 0.1f) pipette.setTemperature(container.getTemperature());
        pipette.IncreaseFill("suck");
        container.ReduceFill("suck");
    }

    public void onPouringWithPipette(GamePourable pipette, GamePourable container)
    {
        if (pipette == null || container == null || container.isFull) return;

        changeReceiverParticleContained(pipette, container);
        if (container.getWeightContained() < 0.1f) container.setTemperature(pipette.getTemperature());
        pipette.ReduceFill("suck");
        container.IncreaseFill("suck");
    }

    public void onPouringWithBigPipette(GamePourable pipette, GamePourable container)
    {
        if (pipette == null || container == null) return;
        if (container.isFull) return;

        changeReceiverParticleContained(pipette, container);
        pipette.ReduceFill("bpip");
        container.IncreaseFill("bpip");
    }

    public void onTitrate(GamePourable burette, GamePourable erlenmeyer)
    {
        if (burette == null || erlenmeyer == null) return;
        if (erlenmeyer.isFull || erlenmeyer.GetType() != typeof(Erlenmeyer300)) return;

        burette.ReduceFill("burr");
        erlenmeyer.IncreaseFill("burr");
    }

    public void RefillPourable(GamePourable pourable)
    {
        if (pourable.isFull) return;
        pourable.IncreaseFill("pour");
    }

    private void changeReceiverParticleContained(GamePourable sender, GamePourable receiver)
    {
        ArrayList particlesSender = sender.getParticleContained();
        ArrayList particlesReceiver = receiver.getParticleContained();

        Debug.Log("Particle: s = " + (particlesSender == null).ToString() + ", r = " + (particlesReceiver == null).ToString());
        foreach (string particleSender in particlesSender.ToArray())
        {
            bool same = false;
            if (particleSender.Equals("")) continue;
            Debug.Log("Particle: > particleSender = " + particleSender + "...");
            foreach (string particleReceiver in particlesReceiver.ToArray())
            {
                if (particleReceiver.Equals(particleSender))
                {
                    same = true;
                    continue;
                }
                else if (particleReceiver.Equals(""))
                {
                    particlesReceiver.Remove(particleReceiver);
                }
                else
                {
                    Debug.Log("Particle: >>particleReceiver = " + particleReceiver + "...");
                }

            }

            if(!same) particlesReceiver.Add(particleSender);
        }
    }

    public GameInteractables GetClosestInteractable(Transform senderTransform)
    {
        float distance = 0.6f;
        GameInteractables closest = null;

        foreach (var interactable in interactables)
        {
            if (interactable.GetComponent<GameInteractables>() == null) continue;

            float currentDistance = Vector3.Distance(senderTransform.position, interactable.transform.position);
            float closestX = (senderTransform.position.x - interactable.transform.position.x);
            float closestY = (senderTransform.position.y - interactable.transform.position.y);
            float closestZ = (senderTransform.position.z - interactable.transform.position.z);
            if (currentDistance == 0f)
                continue;
            else if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = interactable.GetComponent<GameInteractables>();
            }
            else {; }
        }

        return closest;
    }

    public GamePourable GetClosestPourables(Transform transform)
    {
        float distance = 0.6f;
        GamePourable closestPourable = null;

        foreach (var pourable in interactables)
        {
            if (pourable == null || pourable.GetComponent<GamePourable>() == null) continue;
            float currentDistance = Vector3.Distance(transform.position, pourable.transform.position);
            float closestX = (transform.position.x - pourable.transform.position.x);
            float closestY = (transform.position.y - pourable.transform.position.y);
            float closestZ = (transform.position.z - pourable.transform.position.z);

            if (currentDistance == 0f)
                continue;
            else if (currentDistance < distance)
            {
                // https://stackoverflow.com/questions/4099366/how-do-i-check-if-a-number-is-positive-or-negative-in-c
                // https://docs.microsoft.com/en-us/dotnet/api/system.math.sign?redirectedfrom=MSDN&view=net-5.0#overloads
                if (Mathf.Sign(closestX) == Mathf.Sign(transform.rotation.x) && Mathf.Sign(closestZ) == Mathf.Sign(transform.rotation.z))
                {
                    distance = currentDistance;
                    closestPourable = pourable.GetComponent<GamePourable>();
                }
                else continue;
            }
            else {; }
        }

        return closestPourable;
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
