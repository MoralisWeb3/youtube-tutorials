var config = {
    type: Phaser.CANVAS,
    width: 1920,
    height: 1080,
    scene: {
        preload: preload,
        create: create,
        update: update
    },
    fps: {
        target: 60,
        forceSetTimeOut: true
    }
};

const game = new Phaser.Game(config);
const EVENT_COOLOFF = 400; // 1 second cool of between events
var event_cooloffs = [];

var sprites = {}
var destinations = {}
var spriteSpeed = 5;
var stepSize = 50;

var context;

function preload ()
{
    this.load.image('map', 'images/map.jpg');
    this.load.image('skin1', 'images/blob/1.png');
    this.load.image('skin2', 'images/blob/2.png');
    this.load.image('skin3', 'images/blob/3.png');
    this.load.image('skin4', 'images/blob/4.png');
    this.load.image('skin5', 'images/blob/5.png');
    this.load.image('skin6', 'images/blob/6.png');

}

function ping(){
    findDataForUser(Moralis.User.current())
    .then((result) => {
        if(result.length == 0){
            console.log("No user data object found...")
        }
        else{
            var playerData = result[0];
            playerData.set("latestPing",new Date().getTime());
            playerData.save();
            
            
        }
    },(error)=>{
        console.log(error);
    })

    setTimeout(ping,2000);
}

function spawnUser(user){

    findDataForUser(user)
    .then((result) => {
        if(result.length == 0){
            console.log("No user data object found...")
        }
        else{
            var playerData = result[0];
            const skinName = "skin"+playerData.get("skin");
            const x = playerData.get("locX");
            const y = playerData.get("locY");

            console.log("SPAWNING ",x, y, skinName)
            console.log(user);
            console.log(playerData)

            sprites[user.id] = context.add.sprite(x, y, skinName).setScale(1,1);
            
            if(user.id == Moralis.User.current().id){
                playerData.set("latestSpawn",new Date().getTime());
                playerData.save();
            }
            
        }
    },(error)=>{
        console.log(error);
    })
    
    // start recurring events
    ping()
    saveState()
}

async function create ()
{
    console.log("create");
    context = this;
    // place background
    this.add.image(0, 0, 'map').setOrigin(0, 0).setScale(1.5,1.5);

    //place player
    if(Moralis.User.current()){
        spawnUser(Moralis.User.current())
    }


    let query = new Moralis.Query("PlayerData");
    query.exists("locX");
    query.exists("locY");
    let subscription = await query.subscribe();
    

    subscription.on('update', (object) => {

        if(object.get("user").id == Moralis.User.current().id)
            return;

        console.log('user moved');
        console.log(object.get("user").id);
        console.log("new location: " + object.get("locX") + " " + object.get("locY"));

        if(!sprites[object.get("user").id])
        {
            spawnUser(object.get("user"))
        }

        destinations[object.get("user").id] = {x:object.get("locX"),y:object.get("locY")}
        console.log(destinations[object.get("user").id])    

    });

  /* if(!sprites[object.user.id]){
            spawnUser(object.id);
        }

        if(sprites[object.user.id] && object.user.id !=Moralis.User.current().id){
            const skinName = "skin1";
            const x = object.attributes.locX;
            const y = object.attributes.locY;
    
            destinations[object.user.id] = {x:x,y:y};
        }*/
}


function update ()
{


    if(!Moralis.User.current())return;
    const user = Moralis.User.current();
    userId = user.id;
    cursors = this.input.keyboard.createCursorKeys();

    if (cursors.left.isDown)
    {
        emitMove("left", Moralis.User.current().id)
    }
    else if (cursors.right.isDown)
    {
        emitMove("right", Moralis.User.current().id)
    }
    else if (cursors.up.isDown)
    {
        emitMove("up", Moralis.User.current().id)
    }
    else if (cursors.down.isDown)
    {
        emitMove("down", Moralis.User.current().id)
    }




 
    Object.keys(sprites).forEach(function(userId) {
        if(sprites[userId] && destinations[userId]){

            if(sprites[userId].x<destinations[userId].x){
                sprites[userId].resting = false;
                sprites[userId].setPosition(sprites[userId].x+spriteSpeed,sprites[userId].y);
            }

            else if(sprites[userId].x>destinations[userId].x){
                sprites[userId].resting = false;
                sprites[userId].setPosition(sprites[userId].x-spriteSpeed,sprites[userId].y);
            }

            else if(sprites[userId].y<destinations[userId].y){
                sprites[userId].resting = false;
                sprites[userId].setPosition(sprites[userId].x,sprites[userId].y+spriteSpeed);
            }

            else if(sprites[userId].y>destinations[userId].y){
                sprites[userId].resting = false;
                sprites[userId].setPosition(sprites[userId].x,sprites[userId].y-spriteSpeed);
            }
            else
                sprites[userId].resting = true;

        }
    });
}


function canEmitMove(id,direction){

    id = id+direction;

    // if first event - allow it and set cool off
    if(!event_cooloffs[id]){
        event_cooloffs[id] = Date.now()+EVENT_COOLOFF;
        return true;
    }
    //not enough time passed
    else if(event_cooloffs[id]>Date.now()){
        return false;
    }
    else{
        event_cooloffs[id] = Date.now()+EVENT_COOLOFF;
        return true;
    }
}



function emitMove(direction, playerId){

    if(canEmitMove(playerId,direction)){
        console.log(direction, playerId);

        var newX=sprites[Moralis.User.current().id].x,
            newY=sprites[Moralis.User.current().id].y;

        if(direction == "up")
            newY-=stepSize;
        else if(direction == "down")
            newY+=stepSize;
        else if(direction == "left")
            newX-=stepSize;
        else if(direction == "right")
            newX+=stepSize;
            

        destinations[Moralis.User.current().id] = {x:newX,y:newY};
        
       /* 
        */


        findDataForUser(Moralis.User.current())
        .then((result) => {
            if(result.length == 0){
                console.log("No user data object found...")
            }
            else{
                var playerData = result[0];
                playerData.set("locX",newX);
                playerData.set("locY",newY);
                playerData.save();
            }
        },(error)=>{
            console.log(error);
        })

    }


    
    
}

function findDataForUser(user) {
    var data = Moralis.Object.extend("PlayerData");
    const query = new Moralis.Query(data);
    query.equalTo("user", user);
    return query.find();
}


function saveState(){

    var stateToSave = {};
    stateToSave.playerData = [];

    var data = Moralis.Object.extend("PlayerData");
    const query = new Moralis.Query(data);
    query.find()
    .then(async(response) =>  {
        response.forEach(function(r){
            console.log(r.get("user").id)
            console.log(r.get("locX"))
            console.log(r.get("locY"))
            console.log(r.get("skin"))

            stateToSave.playerData.push({
                                user_id:r.get("user").id,
                                locX:r.get("locX"),
                                locY:r.get("locY"),
                                skin:r.get("skin")
                            })
        });


        const file = new Moralis.File("state.json", {base64 : btoa(JSON.stringify(stateToSave))});
        await file.saveIPFS()
        console.log("STATE SAVED")
        console.log(file.ipfs())
    })

    setTimeout(saveState,10000)
}
