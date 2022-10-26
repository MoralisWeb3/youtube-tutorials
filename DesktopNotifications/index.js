const express = require('express')
const notifier = require('node-notifier');
const app = express()
const port = 3000

app.use(express.json())


app.post('/webhook', (req, res) => {

    const webhook = req.body;
    for (const erc20Transfer of webhook.erc20Transfers){
    
        const addrs = `${erc20Transfer.from.slice(0, 4)}...${erc20Transfer.from.slice(38)}`;
        const amount = Number(erc20Transfer.valueWithDecimals).toFixed(0);
        
        notifier.notify({
            title: 'NEW USDT Transfer',
            message: `${addrs} just sent \n$${amount}`,
          });
        
    }

    return res.status(200).json();

})


app.listen(port, () => {
    console.log(`Listening to streams`)
})