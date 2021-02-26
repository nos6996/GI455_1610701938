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
        public Text sendText, receiveText, inputText, createRoomName, joinRoomName, userNameRegis, userIDRegis, passwordRegis,
            rePasswordRegis, userIDLogIn, passwordLogIn, userNameTxt;
        private string iP, port, username;
        //private bool messageSender = false;
        public GameObject rootMessenger, lobby, createRoom, joinRoom, createRoomNotice, joinRoomNotice, leaveRoomNotice, login,
            register, registerFail, loginFail, userName;

        class MessageData
        {
            public string username;
            public string message;
        }

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
            //iP = PlayerPrefs.GetString("IP");
            //port = PlayerPrefs.GetString("Port");
            //username = PlayerPrefs.GetString("Username");
            //username = "nos";
            //ws = new WebSocket("ws://" + iP + ":" + port + "/");
            ws = new WebSocket("ws://127.0.0.1:8080/");

            ws.OnMessage += OnMessage;

            ws.Connect();

            login.SetActive(true);
            register.SetActive(false);
            lobby.SetActive(false);
            createRoom.SetActive(false);
            joinRoom.SetActive(false);
            rootMessenger.SetActive(false);
            createRoomNotice.SetActive(false);
            joinRoomNotice.SetActive(false);
            leaveRoomNotice.SetActive(false);
            registerFail.SetActive(false);
            loginFail.SetActive(false);
            userName.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateNotifyMessage();

            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                MessageData receiveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);
                if (receiveMessageData.username == username)
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

        public void ToLogin()
        {
            register.SetActive(false);
            login.SetActive(true);
        }

        public void ToRegister()
        {
            login.SetActive(false);
            register.SetActive(true);
        }

        public void ToLobby()
        {
            login.SetActive(false);
            lobby.SetActive(true);
        }

        public void CloseRegisterFail()
        {
            registerFail.SetActive(false);
        }

        public void CloseLoginFail()
        {
            loginFail.SetActive(false);
        }

        public void RegisterName(string userName)
        {
            userName = userNameRegis.text;

            if (userNameRegis.text == "" || userIDRegis.text == "" || passwordRegis.text == "" || rePasswordRegis.text == "" || passwordRegis.text != rePasswordRegis.text)
            {
                registerFail.SetActive(true);
            }

            else
            {
                SocketEvent socketEvent = new SocketEvent("RegisterName", userName);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }
        }

        public void RegisterID(string userID)
        {
            userID = userIDRegis.text;

            if (userNameRegis.text == "" || userIDRegis.text == "" || passwordRegis.text == "" || rePasswordRegis.text == "" || passwordRegis.text != rePasswordRegis.text)
            {
                registerFail.SetActive(true);
            }

            else
            {
                SocketEvent socketEvent = new SocketEvent("RegisterID", userID);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }
        }

        public void RegisterPW(string password)
        {
            password = passwordRegis.text;

            if (userNameRegis.text == "" || userIDRegis.text == "" || passwordRegis.text == "" || rePasswordRegis.text == "" || passwordRegis.text != rePasswordRegis.text)
            {
                registerFail.SetActive(true);
            }

            else
            {
                SocketEvent socketEvent = new SocketEvent("RegisterPW", password);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }
        }

        public void Register()
        {
            if (userNameRegis.text == "" || userIDRegis.text == "" || passwordRegis.text == "" || rePasswordRegis.text == "" || passwordRegis.text != rePasswordRegis.text)
            {
                registerFail.SetActive(true);
            }

            else
            {
                SocketEvent socketEvent = new SocketEvent("Register", "");

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }
        }

        public void LogInID(string userID)
        {
            userID = userIDLogIn.text;
            if(userIDLogIn.text == "" || passwordLogIn.text == "")
            {
                loginFail.SetActive(true);
            }

            else
            {
                SocketEvent socketEvent = new SocketEvent("LogInID", userID);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }
        }

        public void LogInPW(string password)
        {
            password = passwordLogIn.text;
            if (userIDLogIn.text == "" || passwordLogIn.text == "")
            {
                loginFail.SetActive(true);
            }

            else
            {
                SocketEvent socketEvent = new SocketEvent("LogInPW", password);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }
        }

        public void LogIn()
        {
            if (userIDLogIn.text == "" || passwordLogIn.text == "")
            {
                loginFail.SetActive(true);
            }

            else
            {
                SocketEvent socketEvent = new SocketEvent("LogIn", "");

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }
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
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            MessageData messageData = new MessageData();
            messageData.username = username;
            messageData.message = inputText.text;

            string toJsonStr = JsonUtility.ToJson(messageData);

            ws.Send(toJsonStr);
            inputText.text = "";
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
                else if (receiveMessageData.eventName == "Register")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                    if (receiveMessageData.data == "success")
                    {
                        ToLogin();
                    }
                    else if (receiveMessageData.data == "fail")
                    {
                        registerFail.SetActive(true);
                    }
                }
                //else if (receiveMessageData.eventName == "UserName")
                //{
                //    if (OnLeaveRoom != null)
                //        OnLeaveRoom(receiveMessageData);
                //    username = receiveMessageData.data;
                //    print(username);
                //}
                else if (receiveMessageData.eventName == "LogIn")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                    if (receiveMessageData.data == "success")
                    {
                        ToLobby();
                        //userNameTxt.text = username;
                        //userName.SetActive(true);
                    }
                    else if (receiveMessageData.data == "fail")
                    {
                        loginFail.SetActive(true);
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

