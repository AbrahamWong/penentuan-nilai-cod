using System.Collections;
using System.Linq;
using UnityEngine;

public static class Linqer
{
    // https://answers.unity.com/questions/828984/gameobject-findgameobjectswithtag-in-order-up-to-d.html
    public static GameObject[] sortGameObjectArray(GameObject[] unsortedArray)
    {
        GameObject[] sortedArray = unsortedArray.OrderBy(x => x.name).ToArray();
        return sortedArray;
    }

}