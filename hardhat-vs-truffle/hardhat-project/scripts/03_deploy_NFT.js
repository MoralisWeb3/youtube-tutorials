const fs = require("fs");
const path = require("path");

async function main() {
  const MyContract = await ethers.getContractFactory("SepoliaNFT");
  const myContract = await MyContract.deploy();
  console.log("Contract deployed to address:", myContract.address);

  // Create the JSON object with the deployment address
  const contractConfig = {
    contractAddress: myContract.address,
  };

  // Define the output folder and file name
  const outputDir = path.join(__dirname, "..", "frontend", "src");
  const outputFile = path.join(outputDir, "contract-config.json");

  // Ensure the output folder exists
  if (!fs.existsSync(outputDir)) {
    fs.mkdirSync(outputDir, { recursive: true });
  }

  // Write the JSON object to the output file
  fs.writeFileSync(outputFile, JSON.stringify(contractConfig, null, 2));

  console.log("Contract configuration saved to:", outputFile);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
