let fs = require("fs");
let axios = require("axios");

let ipfsArray = [];
let promises = [];

for (let i = 0; i < 100; i++) {
    let paddedHex = ("0000000000000000000000000000000000000000000000000000000000000000" + i.toString(16)).substr("-64");
    
    promises.push(new Promise( (res, rej) => {
        fs.readFile(`${__dirname}/export/${paddedHex}.png`, (err, data) => {
            if(err) rej();
            ipfsArray.push({
                path: `images/${paddedHex}.png`,
                content: data.toString("base64")
            })
            res();
        })
    }))
}
Promise.all(promises).then( () => {
    axios.post("https://deep-index.moralis.io/api/v2/ipfs/uploadFolder", 
        ipfsArray,
        {
            headers: {
                "X-API-KEY": '',
                "Content-Type": "application/json",
                "accept": "application/json"
            }
        }
    ).then( (res) => {
        console.log(res.data);
    })
    .catch ( (error) => {
        console.log(error)
    })
})
