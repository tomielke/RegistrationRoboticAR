using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class RotationConverter
{
    public static Vector3 getEulerAngles(Matrix<float> rot)
    {
        //there are two possible solutions, but only one needed. 
        //implementation according to http://eecs.qmul.ac.uk/~gslabaugh/publications/euler.pdf
        Vector3 euler1, euler2; 

        if (Mathf.Abs(rot[2, 0]) != 1)
        {
            euler1.y = -Mathf.Asin(rot[2, 0]);
            euler2.y = Mathf.PI - euler1.y;

            euler1.x = Mathf.Atan2(rot[2, 1] / Mathf.Cos(euler1.y), rot[2, 2] / Mathf.Cos(euler1.y));
            euler2.x = Mathf.Atan2(rot[2, 1] / Mathf.Cos(euler2.y), rot[2, 2] / Mathf.Cos(euler2.y));

            euler1.z = Mathf.Atan2(rot[1, 0] / Mathf.Cos(euler1.y), rot[0, 0] / Mathf.Cos(euler1.y));
            euler2.z = Mathf.Atan2(rot[1, 0] / Mathf.Cos(euler2.y), rot[0, 0] / Mathf.Cos(euler2.y));
        }
        else
        {
            euler1.z = 0; //arbitrary 
            if (rot[2,0] == -1)
            {
                euler1.x = Mathf.PI / 2;
                euler1.y = euler1.z + Mathf.Atan2(rot[0, 1], rot[0, 2]);
            }
            else
            {
                euler1.x = Mathf.PI / 2;
                euler1.y = euler1.z + Mathf.Atan2(-rot[0, 1], -rot[0, 2]);
            }
            euler2 = Vector3.zero; 
        }

        //for consistency: direct conversion to kuka angles, therfore left handed to right handed cos.
        euler1 = new Vector3(-euler1.x, -euler1.y, euler1.z);

        return euler1; 
       
    }

    //get euler angles (Z-Y-X) from quaternion. 
    public static Vector3 getEulerAngles(Quaternion q)
    {
        Vector3 angles = Vector3.zero;

        //roll (x-axis rotation)
        float sinr_cosp = 2 * (q.w * q.x + q.y * q.z);
        float cosr_cosp = 1 - 2 * (q.x * q.x + q.y * q.y);
        angles.x = Mathf.Atan2(sinr_cosp, cosr_cosp);

        // pitch (y-axis rotation)
        float sinp = 2 * (q.w * q.y - q.z * q.x);
        if (Mathf.Abs(sinp) >= 1)
            angles.y = Mathf.Sign(sinp) * (Mathf.PI / 2);
        else
            angles.y = Mathf.Asin(sinp);

        // yaw (z-axis rotation)
        float siny_cosp = 2 * (q.w * q.z + q.x * q.y);
        float cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
        angles.z = Mathf.Atan2(siny_cosp, cosy_cosp);

        return new Vector3(-angles.x, -angles.y, angles.z);

    }

    //get quaternion from kuka.sunrise angels (rotation convention Z-Y-X).
    public Quaternion getQuaternion(Vector3 euler)
    {
        return Quaternion.AngleAxis(Mathf.Rad2Deg * euler.z, Vector3.forward)
            * Quaternion.AngleAxis(-Mathf.Rad2Deg * euler.y, Vector3.up)
            * Quaternion.AngleAxis(-Mathf.Rad2Deg * euler.x, Vector3.right);
    }

    //get quaternion from kuka.sunrise angles (rotation convention Z-Y-X).
    public static Quaternion getQuaternion(Matrix<float> m)
    {
        float tr = m[0, 0] + m[1, 1] + m[2, 2];
        Quaternion q = new Quaternion();
        if (tr > 0f)
        {
            float s = Mathf.Sqrt(1f + tr) * 2f;
            q.w = 0.25f * s;
            q.x = (m[2, 1] - m[1, 2]) / s;
            q.y = (m[0, 2] - m[2, 0]) / s;
            q.z = (m[1, 0] - m[0, 1]) / s;
        }
        else if ((m[0, 0] > m[1, 1]) && (m[0, 0] > m[2, 2]))
        {
            float s = Mathf.Sqrt(1f + m[0, 0] - m[1, 1] - m[2, 2]) * 2f;
            q.w = (m[2, 1] - m[1, 2]) / s;
            q.x = 0.25f * s;
            q.y = (m[0, 1] + m[1, 0]) / s;
            q.z = (m[0, 2] + m[2, 0]) / s;
        }
        else if (m[1, 1] > m[2, 2])
        {
            float s = Mathf.Sqrt(1f + m[1, 1] - m[0, 0] - m[2, 2]) * 2f;
            q.w = (m[0, 2] - m[2, 0]) / s;
            q.x = (m[0, 1] + m[1, 0]) / s;
            q.y = 0.25f * s;
            q.z = (m[1, 2] + m[2, 1]) / s;
        }
        else
        {
            float s = Mathf.Sqrt(1f + m[2, 2] - m[0, 0] - m[1, 1]) * 2f;
            q.w = (m[1, 0] - m[0, 1]) / s;
            q.x = (m[0, 2] + m[2, 0]) / s;
            q.y = (m[1, 2] + m[2, 1]) / s;
            q.z = 0.25f * s;
        }
        return q;
    }

    //get rotation matrix from kuka.sunrise euler angles (rotation convention: Z-Y-X).
    public static Matrix<float> getMatrix(Vector3 eulerAngles)
    {
        float psi = eulerAngles.z;
        float theta = -eulerAngles.y;
        float phi = -eulerAngles.x; 

        float[,] rot = new float[3, 3] {
            {Mathf.Cos(theta)*Mathf.Cos(psi), Mathf.Sin(phi)*Mathf.Sin(theta)*Mathf.Cos(psi)-Mathf.Cos(phi)*Mathf.Sin(psi), Mathf.Cos(phi)*Mathf.Sin(theta)*Mathf.Cos(psi)+Mathf.Sin(phi)*Mathf.Sin(psi) },
            {Mathf.Cos(theta)*Mathf.Sin(psi), Mathf.Sin(phi)*Mathf.Sin(theta)*Mathf.Sin(psi)+Mathf.Cos(phi)*Mathf.Cos(psi), Mathf.Cos(phi)*Mathf.Sin(theta)*Mathf.Sin(psi)-Mathf.Sin(phi)*Mathf.Cos(psi)  },
            {-Mathf.Sin(theta), Mathf.Sin(phi)*Mathf.Cos(theta), Mathf.Cos(phi)*Mathf.Cos(theta) }
        };

        return Matrix<float>.Build.DenseOfArray(rot);
    }

    //get rotation matrix from quaternions. 
    public static Matrix<float> getMatrix(Quaternion q)
    {
        float[,] rot = new float[3, 3] {
           {1-2*q.y*q.y-2*q.z*q.z, 2*q.x*q.y-2*q.z*q.w, 2*q.x*q.z+2*q.y*q.w },
           {2*q.x*q.y+2*q.z*q.w, 1-2*q.x*q.x-2*q.z*q.z, 2*q.y*q.z-2*q.x*q.w },
           {2*q.x*q.z-2*q.y*q.w, 2*q.y*q.z+2*q.x*q.w, 1-2*q.x*q.x-2*q.y*q.y }
        };

        return Matrix<float>.Build.DenseOfArray(rot);
    }


}
