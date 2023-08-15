using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class UDP_Sender
{
    public int Port
    {
        get { return _port; }
    }
    int _port;

    public string Address
    {
        get { return _address; }
    }
    string _address;

    private IPEndPoint _remoteIpEndPoint;
    private Socket _sock;

    public UDP_Sender(string address, int port)
    {
        _port = port;
        _address = address;

        _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        var addresses = System.Net.Dns.GetHostAddresses(address);
        if (addresses.Length == 0)
        {
            throw new Exception("Unable to find IP address for " + address);
        }
        _remoteIpEndPoint = new IPEndPoint(addresses[0], port);
    }

    public void SendUdp(byte[] message)
    {
        _sock.SendTo(message, _remoteIpEndPoint);
    }

    public void Close()
    {
        _sock.Close();
    }
}

