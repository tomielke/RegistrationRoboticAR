using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class RANSACPivot
{
    public static void getRansacPivotCalibration(List<Matrix<float>> allOrientations, List<Vector3> allPositions, int nbIterations, float errorMargin, out Vector3 localPosition, out int nbConsensusSet)
    {
        int maxNbConsentiousPoints = 0;

        List<Matrix<float>> bestConsensusRotations = new List<Matrix<float>>();
        List<Vector3> bestConsensusPositions = new List<Vector3>();

        for (int iteration = 0; iteration < nbIterations; iteration++)
        {
            List<Vector3> posSet = new List<Vector3>();
            List<Matrix<float>> rotSet = new List<Matrix<float>>();

            //choose four random positions and orientations for pivot calibration. 
            int[] randIndex = new int[] {Random.Range(0, allOrientations.Count), Random.Range(0, allOrientations.Count), Random.Range(0, allOrientations.Count), Random.Range(0, allOrientations.Count) };

            foreach (int index in randIndex)
            {
                posSet.Add(allPositions[index]);
                rotSet.Add(allOrientations[index]);
            }
            CalibratePivot(rotSet, posSet, out Vector3 setTLoc, out Vector3 setTGlob);

            int nbConsensusPt = getNbConsensusSet(allOrientations, allPositions, setTLoc, setTGlob, errorMargin, out List<Matrix<float>> consensusRot, out List<Vector3> consensusPos);
            if (nbConsensusPt > maxNbConsentiousPoints)
            {
                bestConsensusPositions = new List<Vector3>(consensusPos);
                bestConsensusRotations = new List<Matrix<float>>(consensusRot);
                maxNbConsentiousPoints = nbConsensusPt;
            }
        }

        if (maxNbConsentiousPoints > 0)
        {
            CalibratePivot(bestConsensusRotations, bestConsensusPositions, out Vector3 currTLoc, out Vector3 currTGlob);
            localPosition = currTLoc;
            nbConsensusSet = maxNbConsentiousPoints;
        }
        else
        {
            Debug.Log("Calibration Not Possible.");
            localPosition = Vector3.zero;
            nbConsensusSet = 0;
        }
    }

    static void CalibratePivot(List<Matrix<float>> allOrientations, List<Vector3> allPositions, out Vector3 tLoc, out Vector3 tGlob)
    {
        int NbPoints = allOrientations.Count;
        Matrix<float> matR = Matrix<float>.Build.Dense((NbPoints - 1) * 3, 3);
        Vector<float> vecT = Vector<float>.Build.Dense((NbPoints - 1) * 3);

        Matrix<float> r1 = allOrientations[0];
        Vector3 p1 = allPositions[0];

        List<Matrix<float>> matAngles = new List<Matrix<float>> { r1 };

        //pivot calibration following the algebraic two step method. 
        for (int i = 0; i < NbPoints - 1; i++)
        {
            Matrix<float> r2 = allOrientations[i + 1];
            Vector3 p2 = allPositions[i + 1];

            for (int z = 0; z < 3; z++)
            {
                for (int s = 0; s < 3; s++)
                    matR[i * 3 + z, s] = r1[z, s] - r2[z, s];

                vecT[i * 3 + z] = p2[z] - p1[z];
            }

            r1 = r2;
            p1 = p2;

            matAngles.Add(r2);
        }

        Vector<float> res = matR.Solve(vecT);

        Vector<float> tglob = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0 });

        for (int i = 0; i < NbPoints; i++)
            tglob += matAngles[i] * res + Vector<float>.Build.DenseOfArray(new float[] { allPositions[i].x, allPositions[i].y, allPositions[i].z });

        tglob = tglob / NbPoints;

        tGlob = getVector3(tglob);
        tLoc = getVector3(res);
    }

    static float getPivotError(Vector3 tloc, Vector3 tglob, Vector3 pos, Matrix<float> rot)
    {
        Vector<float> error = rot * getVector(tloc) + getVector(pos) - getVector(tglob);
        return Mathf.Sqrt(error[0] * error[0] + error[1] * error[1] + error[2] * error[2]);
    }

    static int getNbConsensusSet(List<Matrix<float>> allOrientations, List<Vector3> allPositions, Vector3 tloc, Vector3 tglob, float errorMargin, out List<Matrix<float>> consensusSetOrientations, out List<Vector3> consensusSetPositions)
    {
        int nbConsensusSet = 0;
        consensusSetOrientations=new List<Matrix<float>>();
        consensusSetPositions=new List<Vector3>();

        for (int i = 0; i < allOrientations.Count; i++)
        {
            if (getPivotError(tloc, tglob, allPositions[i], allOrientations[i]) < errorMargin)
            {
                nbConsensusSet++;
                consensusSetOrientations.Add(allOrientations[i]);
                consensusSetPositions.Add(allPositions[i]);
            }
        }
        return nbConsensusSet;
    }

    static Vector<float> getVector(Vector3 a)
    {
        return Vector<float>.Build.DenseOfArray(new float[] { a.x, a.y, a.z });
    }

    static Vector3 getVector3(Vector<float> a)
    {
        return new Vector3(a[0], a[1], a[2]);
    }

}
