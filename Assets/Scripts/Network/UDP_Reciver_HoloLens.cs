using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Threading;


#if WINDOWS_UWP
using Windows.Storage.Streams;
#endif


namespace UDP
{
    public class UDP_Reciver_HoloLens : MonoBehaviour
    {
        private static string ListenPort = "9718";
        [HideInInspector]
        public bool HasNewMessage;

        private bool coroutineRuns = false;
        private bool startReceived = false;


#if WINDOWS_UWP
    Windows.Networking.Sockets.DatagramSocket _Socket = null;
    Windows.Networking.Sockets.DatagramSocket _SocketOut = null;
    Windows.Networking.Sockets.DatagramSocket FindBuddySocket;
#endif

        private Queue<byte[]> _receivedPackages = new Queue<byte[]>();
        public PackageEvent PackageReceiveEvent = new PackageEvent();

        void Start()
        {

            //#if WINDOWS_UWP
            //        Init_Listener();
            //#endif
        }

        void Awake()
        {

#if WINDOWS_UWP
        Init_Listener();
#endif
        }

        void Update()
        {
            while (_receivedPackages.Count > 0)
            {
                var package = _receivedPackages.Dequeue();
                PackageReceiveEvent.Invoke(package);
            }
        }



        public void OnDestroy()
        {
#if WINDOWS_UWP
        if (_Socket != null)
        {
            _Socket.Dispose();
            _Socket = null;
        }
#endif


        }


#if WINDOWS_UWP
    private void Init_Listener()
    {
        try
        {
            _Socket = new Windows.Networking.Sockets.DatagramSocket();
            _Socket.MessageReceived += ServerDatagramSocket_MessageReceived;
            _Socket.BindServiceNameAsync(ListenPort);
            Debug.Log("Listener active"); 
        }
        catch (Exception ex)
        {
            _Socket.Dispose();
            _Socket = null;

            Debug.LogError(ex.ToString());
            Debug.LogError(Windows.Networking.Sockets.SocketError.GetStatus(ex.HResult).ToString());
        }
    }

    public void Dispose()
    {
        if (_Socket != null)
        {
            _Socket.Dispose();
            _Socket = null;
        }
    }

#endif

#if WINDOWS_UWP
    private async void ServerDatagramSocket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender, Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
    {
        try{
            string request;
            using (DataReader dataReader = args.GetDataReader())
            {
                byte[] content = new byte[dataReader.UnconsumedBufferLength];
                dataReader.ReadBytes(content);
                _receivedPackages.Enqueue(content);             
            }
        }
        catch (Exception ex)
        {
            Debug.Log("package not received - ERROR");            
        }
    }
#endif

        public class PackageEvent : UnityEvent<byte[]>
        {
            public byte[] Data;
        }
    }
}





