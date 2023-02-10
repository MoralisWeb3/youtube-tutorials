import fetch from "node-fetch";

const options = {
  method: "GET",
  headers: {
    accept: "application/json",
    "X-API-Key": "YOUR-API-KEY-HERE",
  },
};

fetch(
  "https://testnet-aptos-api.moralis.io/transactions/by_hash/0x3f3a083fbba0a6c1aa7f21664cdc5041666d86ca7b5e620e0130678697149359",
  options
)
  .then((response) => response.json())
  .then((response) => console.log(response))
  .catch((err) => console.error(err));
