using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vuforia; 

public class PinMarker : MonoBehaviour
{
    public List<GameObject> ObjectsToKeepActive;
    public GameObject Parent; 

    public void pinMarker(bool pin)
    {
        if (pin)
        {
            foreach(GameObject obj in ObjectsToKeepActive)
                obj.transform.parent = Parent.transform.parent;
            Parent.SetActive(false);
        }
        else
        {
            foreach (GameObject obj in ObjectsToKeepActive)
            {
                obj.transform.parent = Parent.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
            }
                
            Parent.SetActive(true);
        }          
    }



}
