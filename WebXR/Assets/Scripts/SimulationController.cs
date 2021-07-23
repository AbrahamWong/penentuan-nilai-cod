using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebXR;
using TMPro;

public class SimulationController : MonoBehaviour
{
    [SerializeField] private GameObject[] interactables;
    public bool usingLabCoat { get; set; }
    public bool usingGloves { get; set; }
    public bool usingRespirator { get; set; }
    public bool usingGlasses { get; set; }

    [SerializeField] private GameObject[] indicators;
    [SerializeField] private bool[] indicatorsStatus;

    private WebXRController leftController = null;  private TextMeshPro leftTest;
    private WebXRController rightController = null; private TextMeshPro rightTest;

    public WebXRController getLeftController() => leftController;
    public WebXRController getRightController() => rightController;

    public Dictionary<string, float> PrerequisiteIngredientDictionary { get; } = new Dictionary<string, float>();

    public AudioClip[] clips;

    void Start()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
        indicators = Linqer.sortGameObjectArray(GameObject.FindGameObjectsWithTag("IndicatorTexts"));

        foreach (GameObject indicator in indicators) indicator.SetActive(false);

        // https://stackoverflow.com/questions/20565894/setting-entire-bool-to-false/20566137
        // deklarasi array akan membuat array of bool dengan default value false
        indicatorsStatus = new bool[indicators.Length];

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("VRController");
        foreach (GameObject controller in gameObjects)
        {
            if (controller.name.Equals("handL"))
            {
                leftController = controller.GetComponent<WebXRController>();
                leftTest = leftController.GetComponentInChildren<TextMeshPro>();
            }
            if (controller.name.Equals("handR"))
            {
                rightController = controller.GetComponent<WebXRController>();
                rightTest = rightController.GetComponentInChildren<TextMeshPro>();
            }
        }

        StartCoroutine(coroutineControllerAction(leftController, rightController));
        StartCoroutine(checkExperimentStatus());
        StartCoroutine(updateText());

        PrerequisiteIngredientDictionary.Add("h2c2o4", 0.225f);
        PrerequisiteIngredientDictionary.Add("kmno4", 0.158f);
        PrerequisiteIngredientDictionary.Add("h2so4", 111.11f);
    }

    public void PlayAudioByName (string audioName)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = findClipInSimulator(audioName);
        audioSource.Play();

        Destroy(audioSource, 1f);
    }

    private AudioClip findClipInSimulator(string name)
    {
        AudioClip c = null;
        foreach (AudioClip clip in clips)
        {
            if (clip.name.Equals(name)) c = clip;
        }

        return c;
    }

    public void setPrerequisiteStatus (int prerequisiteNumber, bool status)
    {
        indicatorsStatus[prerequisiteNumber] = status;
        indicators[prerequisiteNumber].SetActive(status);
    }

    public bool getPrerequisiteStatus(int prerequisiteNumber) => indicatorsStatus[prerequisiteNumber];

    public bool isExperimentDone()
    {
        bool done = true;
        foreach (var indicatorStatus in indicatorsStatus)
        {
            if (!indicatorStatus) { done = false; break; }
        }

        return done;
    }

    IEnumerator checkExperimentStatus()
    {
        while (!isExperimentDone()) yield return null;
        yield return new WaitForSeconds(15f);
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

    private bool isObjectWithNoText(GameInteractables interactable)
    {
        if (interactable == null
                || interactable.GetType() == typeof(LabSafetyTrigger)
                || interactable.GetType() == typeof(Funnel)
                || interactable.GetType() == typeof(Burette)
                || interactable.GetType() == typeof(ElectricHeaterTrigger)
                || interactable.GetType() == typeof(WatchGlass)
                || interactable.GetType() == typeof(VolumePipette)) return true;
        else return false;
    }

    public IEnumerator updateText()
    {
        yield return new WaitForEndOfFrame();
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
            if (isObjectWithNoText(interactable)) continue;

            interactable.setInteractableText(
                "Massa ditampung = " + (Mathf.Round(interactable.getWeightContained() * 100) / 100).ToString() +
                "\nTemperatur = " + interactable.getTemperature().ToString() +
                "\nMenampung " + interactable.getParticleInString()
            );
        }
        yield return null;
    }

    public IEnumerator updateInteractableText(GameInteractables interactable)
    {
        if (isObjectWithNoText(interactable)) yield break;

        interactable.setInteractableText(
                "Massa ditampung = " + (Mathf.Round(interactable.getWeightContained() * 100) / 100).ToString() +
                "\nTemperatur = " + interactable.getTemperature().ToString() +
                "\nMenampung " + interactable.getParticleInString()
        );
    }

    
    private char controllerSideTriggered = ' ';
    private GamePourable pourableTriggered = null;
    IEnumerator coroutineControllerAction(WebXRController controllerL, WebXRController controllerR)
    {
        while (!isExperimentDone())
        {
            leftTest.text = "Trigger = " + leftController.GetAxis(WebXRController.AxisTypes.Trigger) 
                + "\nGrip = " + leftController.GetAxis(WebXRController.AxisTypes.Grip);
            rightTest.text = "Trigger = " + rightController.GetAxis(WebXRController.AxisTypes.Trigger)
                + "\nGrip = " + rightController.GetAxis(WebXRController.AxisTypes.Grip);

            // Test hanya bisa di VR hardware
            // if (leftController.GetButtonDown(WebXRController.ButtonTypes.Trigger))
            if (controllerL.GetAxis(WebXRController.AxisTypes.Trigger) >= 0.85 && controllerSideTriggered.Equals(' '))
            {
                GamePourable p = GetClosestPourables(controllerL.transform);
                if (p != null)
                {
                    p.StartTriggerAction();
                    pourableTriggered = p;
                    controllerSideTriggered = 'L';
                }
            }
            else if (controllerR.GetAxis(WebXRController.AxisTypes.Trigger) >= 0.85 && controllerSideTriggered.Equals(' '))
            {
                GamePourable p = GetClosestPourables(controllerR.transform);
                if (p != null) 
                {
                    p.StartTriggerAction();
                    pourableTriggered = p;
                    controllerSideTriggered = 'R';
                }
            }

            // Test hanya bisa di VR hardware
            if (controllerL.GetAxis(WebXRController.AxisTypes.Trigger) <= 0.1 && controllerSideTriggered.Equals('L') ||
                controllerR.GetAxis(WebXRController.AxisTypes.Trigger) <= 0.1 && controllerSideTriggered.Equals('R'))
            {
                if (pourableTriggered != null)
                {
                    pourableTriggered.StopTriggerAction();
                    controllerSideTriggered = ' ';
                }
            }

            // Untuk mengecek temperatur benda dan apakah sarung tangan telah digunakan
            if (controllerL.GetAxis(WebXRController.AxisTypes.Grip) >= 0.85)
            {
                GameInteractables leftInteractable = GetClosestInteractable(controllerL.transform);

                if (leftInteractable != null && leftInteractable.getTemperature() > 41 && !usingGloves)
                {
                    controllerL.GetComponent<CustomControllerInteraction>().Drop();
                    PlayAudioByName("wrong_answer");
                    // leftInteractable.gameObject.GetComponent<FixedJoint>().connectedBody = null;
                }
            }

            if (controllerR.GetAxis(WebXRController.AxisTypes.Grip) >= 0.85)
            {
                GameInteractables rightInteractable = GetClosestInteractable(controllerR.transform);
                if (rightInteractable != null && rightInteractable.getTemperature() > 41 && !usingGloves)
                {
                    controllerR.GetComponent<CustomControllerInteraction>().Drop();
                    PlayAudioByName("wrong_answer");
                    // rightInteractable.gameObject.GetComponent<FixedJoint>().connectedBody = null;
                }
            }

            if (controllerR.GetButton(WebXRController.ButtonTypes.ButtonA))
            {
                PlayAudioByName("ok");
            }

            yield return null;
        }
    }

    public void OnPouringInteractable(GamePourable pouring, GamePourable poured)
    {
        if (pouring == null || poured == null || poured.isFull || poured.GetComponent<Pippette15>() != null) return;

        if (poured.getWeightContained() < 0.01f) poured.setTemperature(pouring.getTemperature());
        pouring.ReduceFill("pour");
        poured.IncreaseFill("pour");
        changeReceiverParticleContained(pouring, poured);
    }

    public void onSuckingWithPipette(GamePourable pipette, GamePourable container)
    {
        if (pipette == null || container == null || pipette.isFull) return;

        if (pipette.getWeightContained() < 0.1f) pipette.setTemperature(container.getTemperature());
        pipette.IncreaseFill("suck");
        container.ReduceFill("suck");
        changeReceiverParticleContained(container, pipette);
    }

    public void onPouringWithPipette(GamePourable pipette, GamePourable container)
    {
        if (pipette == null || container == null || container.isFull) return;

        if (container.getWeightContained() < 0.1f) container.setTemperature(pipette.getTemperature());
        pipette.ReduceFill("suck");
        container.IncreaseFill("suck");
        changeReceiverParticleContained(pipette, container);
    }

    public void onPouringWithBigPipette(GamePourable pipette, GamePourable container, bool trigger)
    {
        if (pipette == null || container == null) return;
        if (container.isFull) return;

        pipette.ReduceFill("bpip");
        container.IncreaseFill("bpip");
        changeReceiverParticleContained(pipette, container);
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

        foreach (string particleSender in particlesSender.ToArray())
        {
            bool same = false;
            if (particleSender.Equals("")) continue;
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
                // else
                // {
                //     Debug.Log("Particle: >>particleReceiver = " + particleReceiver + "...");
                // }

            }

            if(!same) particlesReceiver.Add(particleSender);
        }

        if (Mathf.Approximately(sender.getWeightContained(), 0))
        {
            sender.getParticleContained().Clear();
            StartCoroutine(updateInteractableText(sender));
            StartCoroutine(updateInteractableText(receiver));
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
        float distance = 0.4f;
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
                // if (Mathf.Sign(closestX) == Mathf.Sign(transform.rotation.x) && Mathf.Sign(closestZ) == Mathf.Sign(transform.rotation.z))

                Debug.Log("SimulationController: name = " + pourable.name + " with currentDistance = " + currentDistance + 
                    "\ntempXRot = " + transform.rotation.x + ", tempEulerXRot = " + transform.eulerAngles.x + " and closestZ is in " + closestZ + 
                    "\ntempZRot = " + transform.rotation.z + ", tempEulerZRot = " + transform.eulerAngles.z + " and closestX is in " + closestX);

                distance = currentDistance;
                closestPourable = pourable.GetComponent<GamePourable>();

                // Karena transform.rotation.x dan z nilainya cukup sensitif dan menghitung perubahan sudut nya terhadap perubahan pada
                // sumbu lain, cek relevansi sudut terhadap rotasi yang utama.
                // if (Mathf.Abs(transform.rotation.x) < 0.01f && Mathf.Sign(closestX) == Mathf.Sign(transform.rotation.z) 
                //     || Mathf.Abs(transform.rotation.z) < 0.01f && Mathf.Sign(closestZ) == Mathf.Sign(transform.rotation.x))
                // {
                //     distance = currentDistance;
                //     closestPourable = pourable.GetComponent<GamePourable>();
                // }
                // 
                // else continue;
            }
            else {; }

            Debug.Log("SimulationController: distance is now " + distance);
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
