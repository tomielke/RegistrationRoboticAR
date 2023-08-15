using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

public class KabschRegistrationAlgorithm 
{ 
    //finds transformation for dataset A to be aligned with B.
    public static void getPointBasedRegistration(List<Vector3> A, List<Vector3> B, out Quaternion rotation, out Vector3 translation)
    {
        List<Vector3> correctedA = new List<Vector3>();
        List<Vector3> correctedB = new List<Vector3>();


        //following kabsch algorithm as described in http://nghiaho.com/?page_id=671
        for (int i= 0; i < A.Count; i++) {
            correctedA.Add(A[i] - getAverage(A));
            correctedB.Add(B[i] - getAverage(B));
        }

        Matrix<float> matA = Matrix<float>.Build.Dense(A.Count, 3);
        Matrix<float> matB = Matrix<float>.Build.Dense(B.Count, 3);

        for (int i = 0; i < A.Count; i++){
            for(int j=0; j<3; j++)
            {
                matA[i,j] = correctedA[i][j];
                matB[i,j] = correctedB[i][j];
            }
        }

        Matrix<float> matH = matA.Transpose() * matB;
        Svd<float> svdH = matH.Svd();

        Matrix<float> svdV = svdH.VT.Transpose();
        Matrix<float> matRot = svdV * svdH.U.Transpose();

        if (Mathf.Sign(matRot.Determinant()) < 0)
        {        
            Matrix<float>  matD = Matrix<float>.Build.DenseDiagonal(3, 3, 1);
            matD.SetDiagonal(new float[] { 1, 1, -1 });
            matRot = svdV * matD * svdH.U.Transpose();
        }

        rotation = RotationConverter.getQuaternion(matRot);
        translation = getAverage(B) - rotation*getAverage(A); 
    }

    public static Vector3 getAverage(List<Vector3> list)
    {
        Vector3 average = Vector3.zero;
        foreach(Vector3 vector in list) 
            average += vector;
        return average / list.Count;       
    }
}
