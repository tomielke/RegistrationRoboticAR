using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using UnityEngine;


public class ManageObjectManipulationHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> joints;
    [SerializeField] GameObject allJoints;

    /*
     * adds components to each robot joints that allows for the transformation and imposes constrains.
     */
    void Start()
    {
        foreach (GameObject joint in joints)
        {
            joint.AddComponent<ObjectManipulator>();
            joint.GetComponent<ObjectManipulator>().HostTransform = transform;
            joint.GetComponent<ObjectManipulator>().TwoHandedManipulationType = (Microsoft.MixedReality.Toolkit.Utilities.TransformFlags)3;
            joint.GetComponent<ObjectManipulator>().OnManipulationStarted.AddListener(delegate { setParentToGrabPoint(joint.GetComponent<ObjectManipulator>()); });
            joint.AddComponent<NearInteractionGrabbable>();
            joint.GetComponent<ConstraintManager>().AutoConstraintSelection = false;

            FixedRotationToWorldConstraint rotationConstraint = joint.AddComponent<FixedRotationToWorldConstraint>();
            MoveAxisConstraint translatioConstraint = joint.AddComponent<MoveAxisConstraint>();

            translatioConstraint.ConstraintOnMovement = (Microsoft.MixedReality.Toolkit.Utilities.AxisFlags)7; //constrain every axis

            joint.GetComponent<ConstraintManager>().AddConstraintToManualSelection(rotationConstraint);
            joint.GetComponent<ConstraintManager>().AddConstraintToManualSelection(translatioConstraint);
        }
    }

    void setParentToGrabPoint(ObjectManipulator obj)
    {
        foreach (var source in CoreServices.InputSystem.DetectedInputSources)
        {
            if (source.SourceType == InputSourceType.Hand)
            {
                foreach (var p in source.Pointers)
                {
                    if (p.Result != null)
                    {
                        var endPoint = p.Result.Details.Point;

                        //set parent position to grab position (for rotation around with grab position as pivot)
                        allJoints.transform.parent = null; 
                        transform.position = endPoint;
                        allJoints.transform.parent = transform; 
                    }
                }
            }
        }
    }

    public void setTranslationConstraint(bool active)
    {
        if (active)
            foreach (GameObject a in joints)
                a.GetComponent<ConstraintManager>().AddConstraintToManualSelection(a.GetComponent<MoveAxisConstraint>());
        else
            foreach (GameObject a in joints)
                a.GetComponent<ConstraintManager>().RemoveConstraintFromManualSelection(a.GetComponent<MoveAxisConstraint>());
    }

    public void setRotationConstraint(bool active)
    {
        if (active)
            foreach (GameObject a in joints)
                a.GetComponent<ConstraintManager>().AddConstraintToManualSelection(a.GetComponent<FixedRotationToWorldConstraint>());
        else
            foreach (GameObject a in joints)
                a.GetComponent<ConstraintManager>().RemoveConstraintFromManualSelection(a.GetComponent<FixedRotationToWorldConstraint>());
    }
}
