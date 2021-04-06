const userAddress = "0x29781d9fca70165cbc952b8c558d528b85541f0b";
const results = await Parse.Cloud.run("getTransactions", {userAddress})

console.log(results);
