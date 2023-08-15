using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RobotTransformData
{
    public int id;
    public Vector3 position;
    public Vector3 orientation;
    public float[] jointAngles = new float[7];

    public RobotTransformData()
    {
        
    }

    public RobotTransformData(int _id, Vector3 _pos, Vector3 _rot)
    {
        id = _id;
        position = _pos;
        orientation = _rot;
    }

    public RobotTransformData(int _id, float[] _jointAngles)
    {
        id = _id;
        jointAngles = _jointAngles;
    }

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(id);
                if (id != 3)
                {
                    writer.Write(position.x);
                    writer.Write(position.y);
                    writer.Write(position.z);

                    writer.Write(orientation.x);
                    writer.Write(orientation.y);
                    writer.Write(orientation.z);
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                        writer.Write(jointAngles[i]);
                }
            }
            return m.ToArray();
        }
    }

    public static RobotTransformData Deserialize(byte[] package)
    {
        RobotTransformData data = new RobotTransformData();

        using (MemoryStream m = new MemoryStream(package))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                data.id = reader.ReadInt32();

                if (data.id != 3)
                {
                    var Px = reader.ReadSingle();
                    var Py = reader.ReadSingle();
                    var Pz = reader.ReadSingle();
                    data.position = new Vector3(Px, Py, Pz);

                    var Rx = reader.ReadSingle();
                    var Ry = reader.ReadSingle();
                    var Rz = reader.ReadSingle();
                    data.orientation = new Vector3(Rx, Ry, Rz);
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                        data.jointAngles[i] = reader.ReadSingle();
                }
            }
            return data;
        }
    }

    /*
     * conversion between unity (left handed convention; [m]) to kuka.sunrise (right handed convention; [mm]).
     */
    public void RobotPosToUnity()
    {
        position *= 0.001f;
        position.z *= -1f;
    }

    public void UnityPosToRobot()
    {
        position *= 1000f;
        position.z *= -1f;
    }

    public void JointsToUnity()
    {
        for(int i=0; i<7; i++)
            if(i%2==1)
                jointAngles[i] *= -1f;
        jointAngles[3] *= -1f; //specific for KUKA iiwa due to different orientation of joints. 
    }
}

