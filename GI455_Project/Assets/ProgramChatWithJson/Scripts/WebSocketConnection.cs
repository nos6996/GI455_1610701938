using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnection : MonoBehaviour
    {
        class MessageData
        {
            public string username;
            public string message;
        }

        struct SocketEvent
        {
            public string eventName;
            public string roomName;

            public SocketEvent(string eventName, string roomName)
            {
                this.eventName = eventName;
                this.roomName = roomName;
            }
        }

        public GameObject rootConnection;
        public GameObject rootMessenger;

        public InputField inputUsername;
        public InputField inputText;
        public Text sendText;
        public Text receiveText;
        
        private WebSocket ws;

        private string tempMessageString;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:25500/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            SocketEvent socketEvent = new SocketEvent("CreateRoom", "TestRoom01");

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);

            rootConnection.SetActive(false);
            rootMessenger.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            MessageData messageData = new MessageData();
            messageData.username = inputUsername.text;
            messageData.message = inputText.text;

            string toJsonStr = JsonUtility.ToJson(messageData);

            ws.Send(toJsonStr);
            inputText.text = "";
        }

        private void OnDestroy()
        {
            if (ws != null)
                ws.Close();
        }

        private void Update()
        {
            //if (tempMessageString != "" && tempMessageString != null)
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                MessageData receiveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);
                if(receiveMessageData.username == inputUsername.text)
                {
                    sendText.text += "<color=yellow>" + receiveMessageData.username + "</color>" + " : " + receiveMessageData.message + "\n";
                    receiveText.text += "\n";
                }
                else
                {
                    receiveText.text += "<color=yellow>" + receiveMessageData.username + "</color>" + " : " + receiveMessageData.message + "\n";
                    sendText.text += "\n";
                }
                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            tempMessageString = messageEventArgs.Data;
            Debug.Log(messageEventArgs.Data);
        }
    }
}


