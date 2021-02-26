const sqlite = require('sqlite3').verbose();

var database = new sqlite.Database('./database/chatDB.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{
    
    if(err) throw err;

    console.log("Connected to database");

    var userID = "nos999";
    var password = "999";
    var name = "nos9";

    var sqlSelect = "SELECT * FROM UserData WHERE UserID = 'nos666' AND Password = '666'"; // Log in
    var sqlInsert = `INSERT INTO UserData (UserID, Password, Name) VALUES ("${userID}", "${password}", "${name}")`; // Register
    var sqlUpdate = `UPDATE UserData SET Money = 500 WHERE UserID = "${userID}"`;

    var sqlAddMoney = "SELECT Money FROM UserData WHERE UserID = 'nos555'";

    database.all(sqlUpdate, (err, rows)=>{
        if(err)
        {
            console.log(err);
        }
        else
        {
            /*if(rows.length > 0)
            {
                var currentMoney = rows[0].Money;
                currentMoney += 200;

                database.all("UPDATE UserData SET Money = '"+currentMoney+"' WHERE UserID = '"+userID+"'", (err, rows)=>{
                    if(err)
                    {
                        console.log("Add money fail");
                    }
                    else
                    {
                        var result = {
                            eventName: "Add Money",
                            data: currentMoney
                        }
                    }
                })
            }
            else
            {
                console.log("User ID not found");
            }*/

            console.log(rows);
        }
    })
});