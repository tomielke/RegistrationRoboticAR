using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine.UI; 


public class PivotCalibration : MonoBehaviour
{       
    [SerializeField] GameObject marker;
    [SerializeField] GameObject TCP;
    [SerializeField] Image processIndicator;

    [SerializeField] int nbCalibrationPoints = 100;
    [SerializeField] float errorMargin = 0.0025f;
    [SerializeField] int nbIterations = 70;

    [SerializeField] GameObject pointBasedRegistration;
    [SerializeField] GameObject buttons;

    [SerializeField] CommToRobot sender;

    List<Vector3> position = new List<Vector3>();
    List<Matrix<float>> rotation = new List<Matrix<float>>();
    bool isVisible;

    void Start()
    {
        StartCoroutine(recordCalibrationPoints());
    }

    private void OnDisable()
    {
        if (sender != null)
            sender.SendMsg("StopRotationRoutine;");

        position.Clear();
        rotation.Clear();

        processIndicator.enabled = false;
        TCP.SetActive(false);
    }

    IEnumerator recordCalibrationPoints()
    {
        yield return new WaitUntil(() => isVisible);
        sender.SendMsg("StartRotationRoutine;");

        processIndicator.enabled = true; 
        processIndicator.fillAmount= 0;   

        position.Add(marker.transform.position);
        rotation.Add(RotationConverter.getMatrix(marker.transform.rotation));

        //record position of orientation if marker distance to previous point is >1cm or rotation is >1°
        while(position.Count < nbCalibrationPoints)
        {
            yield return new WaitUntil(() => (isVisible && (position[position.Count - 1] - marker.transform.position).magnitude > 0.01 || 
            Quaternion.Angle(RotationConverter.getQuaternion(rotation[rotation.Count - 1]), marker.transform.rotation) > 1));

            position.Add(marker.transform.position);
            rotation.Add(RotationConverter.getMatrix(marker.transform.rotation));
            processIndicator.fillAmount = ((float)position.Count / nbCalibrationPoints);
        }

        sender.SendMsg("StopRotationRoutine;");
        processIndicator.enabled = false;

        RANSACPivot.getRansacPivotCalibration(rotation, position, nbIterations, errorMargin, out Vector3 localPos, out int Nb);

        //visualize calibrated TCP. 
        TCP.transform.position = marker.transform.rotation * localPos + marker.transform.position;
        TCP.SetActive(true);

        //start point based registration. 
        pointBasedRegistration.SetActive(true);
        buttons.SetActive(true);
    }

    public void changeVisibilityStatus(bool visible) { isVisible = visible; }
}



