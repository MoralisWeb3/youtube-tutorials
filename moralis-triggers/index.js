

const express = require("express");

const app = express();
const port = 3000;

app.use(express.json());


app.post("/webhook", (req, res) => {
    const webhook = req.body;

    const transfers = webhook["erc20Transfers"];

    for (const transfer of transfers) {
        const { from, to, value, triggers } = transfer;
        const senderAccount = from;
        const receiverAccount = to;
        const amount = value;
        let formattedVal = "$" + amount.slice(0, -6) + "." + amount.slice(-6);

        const leftBalance = triggers[0]['value'];
        let formattedNum = "$" + leftBalance.slice(0, -6) + "." + leftBalance.slice(-6);


        console.log(`================================================`)
        console.log(`Account: ${senderAccount}`);
        console.log(`Sent to: ${receiverAccount}`);
        console.log(`Sent: ${formattedVal}`);
        console.log(`Left Balance: ${formattedNum}`);
        console.log(`================================================`)

    }
    return res.status(200).json();
});

app.listen(port, () => {
    console.log(`Listening to streams`);
});
