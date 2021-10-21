let fs = require("fs");
let axios = require("axios");

let ipfsArray = [];
let promises = [];

for (let i = 0; i < 100; i++) {
    let paddedHex = ("0000000000000000000000000000000000000000000000000000000000000000" + i.toString(16)).substr("-64");
    ipfsArray.push({
        path: `metadata/${paddedHex}.json`,
        content: {
            image: `ipfs://QmX1xfbKoUEHtnoiVY9uEtH3X6YuJx2yzPg6ggp3zrFUzz/images/${paddedHex}.png`,
            name: `My Youtube test NFT #${i}`,
            description: "Awesome NFT for my youtube video"
        }
    })
}
axios.post("https://deep-index.moralis.io/api/v2/ipfs/uploadFolder", 
    ipfsArray,
    {
        headers: {
            "X-API-KEY": 'wnFbJ8XSeI8Ym4uOBsBHQfoYSOXF31OxAxs6ohXchHo7D7qvoJ66Qetxll5jrA46',
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

//'https://ipfs.moralis.io:2053/ipfs/QmX1xfbKoUEHtnoiVY9uEtH3X6YuJx2yzPg6ggp3zrFUzz/images/0000000000000000000000000000000000000000000000000000000000000063.png'