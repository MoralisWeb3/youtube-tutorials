const Moralis = require("moralis").default;
const Chains = require("@moralisweb3/evm-utils");
const EvmChain = Chains.EvmChain;

const transferAbi = [
    {
        "anonymous": false,
        "inputs": [
          {
            "indexed": true,
            "internalType": "address",
            "name": "from",
            "type": "address"
          },
          {
            "indexed": true,
            "internalType": "address",
            "name": "to",
            "type": "address"
          },
          {
            "indexed": true,
            "internalType": "uint256",
            "name": "tokenId",
            "type": "uint256"
          }
        ],
        "name": "Transfer",
        "type": "event"
      }
]

const options = {
    chains: [EvmChain.ETHEREUM],
    description: "All ETH NFT Trasfers",
    tag: "nft",
    allAddresses: true,
    includeContractLogs: true,
    abi: transferAbi,
    topic0: ["Transfer(address,address,uint256)"],
    webhookUrl: "xxx/webhook"
}

Moralis.start({
    apiKey: "xxx",
}).then(async () => {
    const stream = await Moralis.Streams.add(options);
    console.log(stream); 
  });