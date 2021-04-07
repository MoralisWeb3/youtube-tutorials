const userAddress = "0x29781d9fca70165cbc952b8c558d528b85541f0b";
const pageSize = 2;
pageNum = 2;
const results = await Parse.Cloud.run("getTransactions", {userAddress, pageSize, pageNum});

console.log(results);
