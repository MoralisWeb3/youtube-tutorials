const hre = require("hardhat");

async function main() {
 
  const hashing = require('keccak256');
  var hashedPassword = hashing('Mellon');

  // We get the contract to deploy
  const MoriaGates = await hre.ethers.getContractFactory("MoriaGates");
  // We deploy the contract. IMPORTANT --> deploy() parameter needs to be what the contract constructor needs
  const moriaGates = await MoriaGates.deploy(hashedPassword);

  await moriaGates.deployed();
  console.log("Contract address: ", moriaGates.address);
  console.log("Hashed password: ", hashedPassword);

  await moriaGates.deployTransaction.wait(5);

  // We verify the contract
  await hre.run("verify:verify", {
    address: moriaGates.address,
    constructorArguments: [
      hashedPassword,
    ],
  });
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
