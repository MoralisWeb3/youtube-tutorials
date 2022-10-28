const express = require("express");
const app = express();
const port = 3000;


app.use(express.json());

app.post("/webhook", async (req, res) => {
  const { body } = req;
  
  console.log(body);

  return res.status(200).json();

});


  app.listen(port, () => {
    console.log(`Listening to streams`);
  });

