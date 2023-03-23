// send_to_frontend.js

const fs = require("fs");
const path = require("path");
const { ethers, artifacts } = require("hardhat");

async function main() {
  const outputDir = path.join(__dirname, "..", "frontend", "src");

  if (!fs.existsSync(outputDir)) {
    fs.mkdirSync(outputDir, { recursive: true });
  }

  // Get network and chainId
  const network = await ethers.provider.getNetwork();
  const chainId = network.chainId;

  // Replace this with your contract's name
  const contractName = "SepoliaNFT";

  // Get the contract artifact
  const ContractArtifact = await artifacts.readArtifact(contractName);

  // Get the deployed contract address and ABI
  const contractAddress = ContractArtifact.deployedAddress;
  const contractABI = ContractArtifact.abi;

  // Prepare the JSON object
  const jsonConfig = {
    chainId,
    contractAddress,
    contractABI,
  };

  // Write the JSON object to a file
  const outputPath = path.join(outputDir, "config.json");
  fs.writeFileSync(outputPath, JSON.stringify(jsonConfig, null, 2));

  console.log("Configuration file generated at", outputPath);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });

//   const [deployer] = await ethers.getSigners();
