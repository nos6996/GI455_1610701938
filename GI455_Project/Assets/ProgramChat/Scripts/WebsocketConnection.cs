using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        private WebSocket websocket;
        public Text myMessage, otherMessage, sendMessage;
        private string iP, port;
        private bool messageSender = false;

        // Start is called before the first frame update
        void Start()
        {
            iP = PlayerPrefs.GetString("IP");
            port = PlayerPrefs.GetString("Port");
            websocket = new WebSocket("ws://" + iP + ":" + port + "/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                websocket.Send("Number : " + Random.Range(0, 99999));
            }
        }

        public void OnDestroy()
        {
            if(websocket != null)
            {
                websocket.Close();
            }
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            if(messageSender == true)
            {
                myMessage.alignment = TextAnchor.LowerRight;
                otherMessage.text += "\n";
                myMessage.text += "\n" + messageEventArgs.Data;
                messageSender = false;
            }
            else
            {
                otherMessage.alignment = TextAnchor.LowerLeft;
                myMessage.text += "\n";
                otherMessage.text += "\n" + messageEventArgs.Data;
            }
        }

        public void Send()
        {
            messageSender = true;
            websocket.Send(sendMessage.text);
        }
    }
}

