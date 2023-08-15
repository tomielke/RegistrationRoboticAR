using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UDP;
using UnityEngine;

public class CommFromDesktop : MonoBehaviour
{
    [SerializeField] private UDP_Reciver_HoloLens _receiver = default;
    [SerializeField] PointbasedRegistration boardRegistration;
    [SerializeField] PointbasedRegistration midairRegistration;
    [SerializeField] ManualRegistration manualRegistration;
 
    // Start is called before the first frame update
    void Start()
    {
        _receiver.PackageReceiveEvent.AddListener(PackageReceiveListener);
    }


    private void PackageReceiveListener(byte[] arg0)
    {
        RobotTransformData d = RobotTransformData.Deserialize(arg0);
        d.RobotPosToUnity();

        switch (d.id)
        {
            case 1:
                boardRegistration.robotPoints.Add(d.position);
                break; 
            case 2:
                midairRegistration.robotPoints.Add(d.position);
                break;
            case 3:
                d.JointsToUnity();
                manualRegistration.rotateJoints(d.jointAngles);
                break;
        }
    }
}
