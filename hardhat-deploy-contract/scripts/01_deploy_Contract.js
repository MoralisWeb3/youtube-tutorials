async function main() {
  const MyContract = await ethers.getContractFactory("MyContract");
  const myContract = await MyContract.deploy("Hello World");
  console.log("Contract deployed to address:", myContract.address);

  const currentValue = await myContract.getMessage();
  console.log("Current value:", currentValue);

  const transactionResponse = await myContract.updateMessage("A new message");
  await transactionResponse.wait(1);

  const newValue = await myContract.getMessage();
  console.log("New value:", newValue);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
