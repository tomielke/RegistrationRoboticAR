using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasPosition : MonoBehaviour
{
    [SerializeField] Vector3 relativePosition;
    [SerializeField] Transform parent; 

    // Start is called before the first frame update
    void Start()
    {
        if (parent == null)
            parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.position = parent.transform.position + transform.TransformDirection(relativePosition) ; 
    }
}
