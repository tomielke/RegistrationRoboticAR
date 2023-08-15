using System.Collections.Generic;
using UnityEngine;

/*
 * The manual registration includes a basic robotic placeholder made up of simple shapes. 
 * To make this functional, it needs to be substituted with the preferred robot model. 
 * To guarantee accurate replication of the actual robot's motion, every robot joint must be represented by an individual GameObject, 
 * all of which are referred to in a List<GameObject> named 'joints'.
 */

public class ManualRegistration : MonoBehaviour
{
    [SerializeField] List<GameObject> joints;
    [SerializeField] CommToRobot sender;
    [SerializeField] GameObject robotKOS;
    [SerializeField] GameObject robot;

    List<Vector3> distances = new List<Vector3>();
   
    private void OnDisable()
    {
        if(sender != null)
            sender.SendMsg("SendJointAngles;Stop;");
        if(robotKOS != null)
            robotKOS.SetActive(false);
    }

    private void Start()
    {
        for (int i = 1; i < joints.Count; i++)
            distances.Add(joints[i].transform.localPosition - joints[i - 1].transform.localPosition);

        //activate the continous sending of robot joint angles. 
        sender.SendMsg("SendJointAngles;Start;");

        //position robot in front of user randomly rotated. 
        robot.transform.position = Camera.main.transform.position + Camera.main.transform.TransformDirection(new Vector3(0, -0.2f, 1));
        robot.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(new Vector3(90f, 0, 0));
        robot.transform.localRotation *= Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360f)));
    }

    public void finishRegistration(bool finish)
    {
        robotKOS.transform.position = joints[0].transform.position;
        robotKOS.transform.rotation = joints[0].transform.rotation;
        robot.SetActive(!finish);
        robotKOS.SetActive(finish);
    }

    void rotateJoints(List<float> _angles)
    {
        //rotate robot joints to match configuration of the physical robot. 
        for (int i = 0; i < joints.Count-1; i++)
        {
            if (i % 2 == 0)
                joints[i + 1].transform.rotation = joints[i].transform.rotation * Quaternion.Euler(new Vector3(0, 0, _angles[i]));
            else
                joints[i + 1].transform.rotation = joints[i].transform.rotation * Quaternion.Euler(new Vector3(0, _angles[i], 0));
            joints[i + 1].transform.position = joints[i].transform.position + joints[i].transform.rotation * distances[i];
        }
    }

    public void rotateJoints(float[] angles)
    {
        List<float> angleList = new List<float>();
        foreach(float a in angles)
            angleList.Add(a);
        rotateJoints(angleList);
    }
}
