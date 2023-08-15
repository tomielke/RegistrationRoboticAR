using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class PointbasedRegistration : MonoBehaviour
{
    public CommToRobot udpConnection;

    public enum registrationMode { boardRegistration, midairRegistration, none };
    [SerializeField] registrationMode Mode = registrationMode.none;

    int nbCalibrationPoints = 4;
    public bool robotPositionReached = false;

    public List<Vector3> robotPoints = new List<Vector3>();
    List<Vector3> markerPoints = new List<Vector3>();

    [SerializeField] Material activeMaterial;
    [SerializeField] Material recordedMaterial;

    [SerializeField] List<GameObject> registrationPoints = new List<GameObject>();
    [SerializeField] GameObject TCP;
    [SerializeField] GameObject board; 
    [SerializeField] GameObject robotKOS;

    [SerializeField] Image processIndicator;

    [SerializeField] GameObject buttonConfirm;
    [SerializeField] GameObject buttonDelete; 

    int startPoint = 0; 

    private void OnEnable()
    {
        startCalibration();
    }

    public void startCalibration()
    {
        resetRegistration();
        StartCoroutine(recordCalibrationPoints());
    }

    IEnumerator recordCalibrationPoints()
    {
        for (int currPoint = startPoint; currPoint < nbCalibrationPoints; currPoint++)
        {
            buttonConfirm.GetComponent<ButtonStatusController>().SetStatus(true);

            //highlight current registration point in board registration. 
            if (Mode == registrationMode.boardRegistration)
            {
                registrationPoints[currPoint].SetActive(true);
                registrationPoints[currPoint].GetComponent<Renderer>().material = activeMaterial;
            }

            //wait until confirm button is pressed. 
            yield return new WaitUntil(() => robotPositionReached);
            robotPositionReached = false;
            buttonConfirm.GetComponent<ButtonStatusController>().SetStatus(false);

            //request current position from robot. 
            udpConnection.SendMsg("SendPose;" + ((int)Mode+1));

            //visualize recorded registration point in mid-air registration. 
            if(Mode == registrationMode.midairRegistration)
            {
                registrationPoints[currPoint].transform.localPosition = registrationPoints[currPoint].transform.parent.transform.InverseTransformPoint(TCP.transform.position);
                registrationPoints[currPoint].SetActive(true);              
            }

            //wait for current position in robotKOS. 
            yield return new WaitUntil(() => robotPoints.Count > currPoint);

            //update progress indicator, registration point visualization and store current position in deviceKOS.
            processIndicator.fillAmount = (float)(robotPoints.Count) / (float)nbCalibrationPoints;
            markerPoints.Add(registrationPoints[currPoint].transform.localPosition);
            
            registrationPoints[currPoint].GetComponent<Renderer>().material = recordedMaterial;

            if (currPoint == 0)
                buttonDelete.GetComponent<ButtonStatusController>().SetStatus(true);
        }
        getPointBasedRegistration(robotPoints, markerPoints);
    }

    void getPointBasedRegistration(List<Vector3> _markerPoints, List<Vector3> _robotPoints)
    {
        KabschRegistrationAlgorithm.getPointBasedRegistration(_markerPoints, _robotPoints, out Quaternion rotation, out Vector3 translation);

        robotKOS.transform.position += robotKOS.transform.TransformDirection(translation);
        robotKOS.transform.rotation *= rotation;

        robotKOS.SetActive(true);
    }

    public void removeLastPoint()
    {
        StopCoroutine(recordCalibrationPoints());

        if(robotPoints.Count == nbCalibrationPoints)
            resetRobotKOSstartPos();

        for (int i=robotPoints.Count-1; i<registrationPoints.Count; i++)
            registrationPoints[i].SetActive(false);

        robotPoints.RemoveAt(robotPoints.Count-1);
        markerPoints.RemoveAt(markerPoints.Count-1);
        processIndicator.fillAmount = (float)(robotPoints.Count) / (float)nbCalibrationPoints;

        startPoint = markerPoints.Count;
        if (startPoint == 0) 
            buttonDelete.GetComponent<ButtonStatusController>().SetStatus(false);
        
        StartCoroutine(recordCalibrationPoints());
    }

    void resetRegistration()
    {
        startPoint = 0;
        foreach (GameObject obj in registrationPoints)
        {
            obj.SetActive(false);
            obj.GetComponent<Renderer>().material = recordedMaterial;
        }
        robotPoints.Clear();
        markerPoints.Clear();
        processIndicator.fillAmount = 0;

        buttonDelete.GetComponent<ButtonStatusController>().SetStatus(false);
        resetRobotKOSstartPos();
    }

    void resetRobotKOSstartPos()
    {
        robotKOS.SetActive(false);
        robotKOS.transform.position = registrationPoints[0].transform.parent.transform.position;
        robotKOS.transform.rotation = registrationPoints[0].transform.parent.transform.rotation;
    }

    public void setRobotPositionReached(bool isReached)
    {
        robotPositionReached = isReached;
    }


}
