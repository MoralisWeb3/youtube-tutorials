const fs = require("fs");
const path = require("path");

async function main() {
  const MyContract = await ethers.getContractFactory("SepoliaNFT");
  const myContract = await MyContract.deploy();
  console.log("Contract deployed to address:", myContract.address);


}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
