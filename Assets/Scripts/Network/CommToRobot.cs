using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class CommToRobot : MonoBehaviour
{
    [SerializeField]
    private string _ipAddress = default;
    [SerializeField]
    private int _port = 0;

    private UDP_Sender _sender;


    private void Awake()
    {
        _sender = new UDP_Sender(_ipAddress, _port);
    }

    private void OnDestroy()
    {
        //close the udp sender (unblocking the port)
        _sender?.Close();
    }

    public void SendMsg(String msg)
    {
        SendMsg(0,msg);
    }

    public void SendMsg(int id, String msg)
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(id);
                writer.Write(msg);
            }
            _sender.SendUdp(m.ToArray());
        }
    }
    
    

    public void SendMsg(byte[] msg)
    {
        _sender.SendUdp(msg);
    }


}
