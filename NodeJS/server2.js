const app = require('express')();
const server = require('http').Server(app);
const websocket = require('ws');
const wss = new websocket.Server({server});
const sqlite = require('sqlite3').verbose();

server.listen(process.env.PORT || 8080, ()=>{
    console.log("Server start at port "+server.address().port);
});

var wsList = [];
var roomList = [];

var name;
var userID;
var userPW;
/*
{
    roomName: ""
    wsList: []
}
*/

var database = new sqlite.Database('./database/chatDB.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{
    
    if(err) throw err;

    console.log("Connected to database");

});

wss.on("connection", (ws)=>{
    
    //Lobby
    console.log("client connected.");
    //Reception
    ws.on("message", (data)=>{
        console.log("send from client :"+ data);

        //========== Convert jsonStr into jsonObj =======

        //toJsonObj = JSON.parse(data);

        // I change to line below for prevent confusion
        var toJsonObj = { 
            roomName:"",
            data:""
        }
        toJsonObj = JSON.parse(data);
        //===============================================

        if(toJsonObj.eventName == "CreateRoom")//CreateRoom
        {
            //============= Find room with roomName from Client =========
            var isFoundRoom = false;
            for(var i = 0; i < roomList.length; i++)
            {
                if(roomList[i].roomName == toJsonObj.data)
                {
                    isFoundRoom = true;
                    break;
                }
            }
            //===========================================================

            if(isFoundRoom == true)// Found room
            {
                //Can't create room because roomName is exist.
                //========== Send callback message to Client ============

                //ws.send("CreateRoomFail"); 

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"CreateRoom",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("client create room fail.");
            }
            else
            {
                //============ Create room and Add to roomList ==========
                var newRoom = {
                    roomName: toJsonObj.data,
                    wsList: []
                }

                newRoom.wsList.push(ws);

                roomList.push(newRoom);
                //=======================================================

                //========== Send callback message to Client ============

                //ws.send("CreateRoomSuccess");

                //I need to send roomName into client too. I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"CreateRoom",
                    data:toJsonObj.data
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================
                console.log("client create room success.");
            }

            //console.log("client request CreateRoom ["+toJsonObj.data+"]");
            
        }
        else if(toJsonObj.eventName == "JoinRoom")//JoinRoom
        {
            //============= Home work ================
            // Implementation JoinRoom event when have request from client.
            var isFoundRoom = false;
            for(var i = 0; i < roomList.length; i++)
            {
                if(roomList[i].roomName == toJsonObj.data)
                {
                    isFoundRoom = true;
                    roomList[i].wsList.push(ws);
                    break;
                }
            }

            if(isFoundRoom == true)
            {
                var callbackMsg = {
                    eventName:"JoinRoom",
                    data:toJsonObj.data
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                console.log("client join room success.");
            }

            else
            {
                var callbackMsg = {
                    eventName:"JoinRoom",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);

                console.log("client join room fail.");
            }
            //================= Hint =================
            //roomList[i].wsList.push(ws);

            console.log("client request JoinRoom");
            //========================================
        }
        else if(toJsonObj.eventName == "LeaveRoom")//LeaveRoom
        {
            //============ Find client in room for remove client out of room ================
            var isLeaveSuccess = false;//Set false to default.
            for(var i = 0; i < roomList.length; i++)//Loop in roomList
            {
                for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
                {
                    if(ws == roomList[i].wsList[j])//If founded client.
                    {
                        roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                        if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                        {
                            roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                        }
                        isLeaveSuccess = true;
                        break;
                    }
                }
            }
            //===============================================================================

            if(isLeaveSuccess)
            {
                //========== Send callback message to Client ============

                //ws.send("LeaveRoomSuccess");

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    data:"success"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("leave room success");
            }
            else
            {
                //========== Send callback message to Client ============

                //ws.send("LeaveRoomFail");

                //I will change to json string like a client side. Please see below
                var callbackMsg = {
                    eventName:"LeaveRoom",
                    data:"fail"
                }
                var toJsonStr = JSON.stringify(callbackMsg);
                ws.send(toJsonStr);
                //=======================================================

                console.log("leave room fail");
            }
        }
        else if(toJsonObj.eventName == "RegisterName")
        {
            name = toJsonObj.data;
        }
        else if(toJsonObj.eventName == "RegisterID")
        {
            userID = toJsonObj.data;
        }
        else if(toJsonObj.eventName == "RegisterPW")
        {
            userPW = toJsonObj.data;
        }
        else if(toJsonObj.eventName == "Register")
        {
            var sqlInsert = `INSERT INTO UserData (UserID, Password, Name) VALUES ("${userID}", "${userPW}", "${name}")`;
            database.all(sqlInsert, (err, rows)=>{
                if(err)
                {
                    var callbackMsg = {
                        eventName:"Register",
                        data:"fail"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("Register fail");

                    console.log(err);
                }
                else
                {
                    var callbackMsg = {
                        eventName:"Register",
                        data:"success"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("Register success");

                    console.log(rows);
                }
            })
        }
        else if(toJsonObj.eventName == "LogInID")
        {
            userID = toJsonObj.data;
        }
        else if(toJsonObj.eventName == "LogInPW")
        {
            userPW = toJsonObj.data;
        }
        else if(toJsonObj.eventName == "LogIn")
        {
            var sqlSelect = `SELECT * FROM UserData WHERE UserID = "${userID}" AND Password = "${userPW}"`;
            database.all(sqlSelect, (err, rows)=>{
                if(err)
                {
                    var callbackMsg = {
                        eventName:"LogIn",
                        data:"fail"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("Log In fail");

                    console.log(err);
                }
                else
                {
                    var callbackMsg = {
                        eventName:"LogIn",
                        data:"success"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);

                    console.log("Log In success");

                    console.log(rows);
                }
            })
        }
    });


    /*wsList.push(ws);
    
    ws.on("message", (data)=>{
        console.log("send from client :"+ data);
        Boardcast(data);
    });*/
    
    ws.on("close", ()=>{
        console.log("client disconnected.");

        //============ Find client in room for remove client out of room ================
        for(var i = 0; i < roomList.length; i++)//Loop in roomList
        {
            for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
            {
                if(ws == roomList[i].wsList[j])//If founded client.
                {
                    roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                    if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                    {
                        roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                    }
                    break;
                }
            }
        }
        //===============================================================================
    });
});

function Boardcast(ws, message)
{
    var selectRoomIndex =  -1;
    for(var i = 0; i < roomList.length; i++)
    {
        for(var j = 0; j < roomList[i].wsList.length; j++)
        {
            if(ws == roomList[i].wsList[j])
            {
                selectRoomIndex = i;
                break;
            }
        }
    }
    for(var i = 0; i < roomList[selectRoomIndex].wsList.length; i++)
    {
        var callbackMsg = {
            eventName:"SendMessage",
            data:message
        }
        roomList[selectRoomIndex].wsList[i].send(JSON.stringify(callbackMsg));
    }
}

