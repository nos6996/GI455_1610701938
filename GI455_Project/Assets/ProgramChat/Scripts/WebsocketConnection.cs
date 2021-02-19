using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        //private WebSocket websocket;
        public Text myMessage, otherMessage, sendMessage, createRoomName, joinRoomName;
        private string iP, port, username;
        //private bool messageSender = false;
        public GameObject rootMessenger, lobby, createRoom, joinRoom, createRoomNotice, joinRoomNotice, leaveRoomNotice;

        public struct SocketEvent
        {
            public string eventName;
            public string data;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }

        private WebSocket ws;

        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;

        // Start is called before the first frame update
        void Start()
        {
            iP = PlayerPrefs.GetString("IP");
            port = PlayerPrefs.GetString("Port");
            username = PlayerPrefs.GetString("Username");
            //username = "nos";
            ws = new WebSocket("ws://" + iP + ":" + port + "/");
            //ws = new WebSocket("ws://127.0.0.1:8080/");

            ws.OnMessage += OnMessage;

            ws.Connect();

            lobby.SetActive(true);
            createRoom.SetActive(false);
            joinRoom.SetActive(false);
            rootMessenger.SetActive(false);
            createRoomNotice.SetActive(false);
            joinRoomNotice.SetActive(false);
            leaveRoomNotice.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateNotifyMessage();
        }

        public void ToCreateRoom()
        {
            lobby.SetActive(false);
            createRoom.SetActive(true);
        }

        public void ToJoinRoom()
        {
            lobby.SetActive(false);
            joinRoom.SetActive(true);
        }

        public void CreateRoom(string roomName)
        {
            roomName = createRoomName.text;
            SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

        public void JoinRoom(string roomName)
        {
            roomName = joinRoomName.text;
            SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }

        public void SendMessage(string message)
        {

        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                    if(receiveMessageData.data == "fail")
                    {
                        createRoomNotice.SetActive(true);
                    }
                    else
                    {
                        createRoom.SetActive(false);
                        rootMessenger.SetActive(true);
                    }
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);
                    if (receiveMessageData.data == "fail")
                    {
                        joinRoomNotice.SetActive(true);
                    }
                    else
                    {
                        joinRoom.SetActive(false);
                        rootMessenger.SetActive(true);
                    }
                }
                else if (receiveMessageData.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                    if (receiveMessageData.data == "success")
                    {
                        lobby.SetActive(true);
                        createRoom.SetActive(false);
                        joinRoom.SetActive(false);
                        rootMessenger.SetActive(false);
                    }
                    else if (receiveMessageData.data == "fail")
                    {
                        leaveRoomNotice.SetActive(true);
                    }
                }

                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);

            tempMessageString = messageEventArgs.Data;
        }

        public void CreateRoomNotice()
        {
            createRoomNotice.SetActive(false);
        }

        public void JoinRoomNotice()
        {
            joinRoomNotice.SetActive(false);
        }

        public void LeaveRoomNotice()
        {
            leaveRoomNotice.SetActive(false);
        }

        public void CreateRoomBack()
        {
            lobby.SetActive(true);
            createRoom.SetActive(false);
        }

        public void JoinRoomBack()
        {
            lobby.SetActive(true);
            joinRoom.SetActive(false);
        }
    }
}

