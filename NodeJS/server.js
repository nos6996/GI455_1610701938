var websocket = require("ws");

var websocketServer = new websocket.Server({port:25500}, ()=>{
    console.log("server is running");
});

var wsList = [];
var roomList = [];

websocketServer.on("connection", (ws, rq)=>{
    //Lobby
    {
        console.log("client connected.");
        //Reception
        ws.on("message", (data)=>{
            console.log("send from client : " + data);
            
            var toJsonObj = JSON.parse(data);

            if([toJsonObj].eventName == "CreateRoom")//CreateRoom
            {
                var IsFoundRoom = false;

                for(var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj.roomName)
                    {
                        IsFoundRoom = true;
                        break;
                    }
                }

                if(IsFoundRoom == true)
                {
                    //Callback to client : create room fail
                    ws.send("Create Room Fail")

                    console.log("Create Room Fail");
                }
                else
                {
                    //Callback to client : create room success
                    var newRoom = {
                        roomName: [toJsonObj].roomName,
                        wsList: []
                    }
                    newRoom.wsList.push(ws);
    
                    roomList.push(newRoom);

                    ws.send("Create Room Success");

                    console.log("Create Room Success");
                }

                console.log("client request CreateRoom : " + newRoom.roomName);
            }
            else if([toJsonObj].eventName == "JoinRoom")//JoinRoom
            {
                console.log("client request JoinRoom : " + [toJsonObj].roomName);
            }
        });
    }
    
    //console.log("client connected."); 
   
   //wsList.push(ws);

   //ws.on("message", (data)=>{
       //console.log("send from client : " + data);
       //Boardcast(data);
   //});

   ws.on("close", ()=>{
       wsList = ArrayRemove(wsList, ws);
       console.log("client disconected.");
   });
});

function Boardcast(data)
{
    for(var i = 0; i < wsList.length; i++)
    {
        wsList[i].send(data);
    }
}

function ArrayRemove(arr, value)
{
    return arr.filter((element)=>{
        return element != value;
    });
}