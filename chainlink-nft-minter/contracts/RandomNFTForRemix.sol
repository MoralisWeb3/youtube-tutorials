//SPDX-License-Identifier: MIT

pragma solidity ^0.8.0;

// Important Imports

import "@openzeppelin/contracts/token/ERC721/extensions/ERC721URIStorage.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@chainlink/contracts/src/v0.8/VRFConsumerBaseV2.sol";
import "@chainlink/contracts/src/v0.8/interfaces/VRFCoordinatorV2Interface.sol";

// Error Handling

error RandomNFT_AllreadyInitialized();
error RandomNFT_NeedMoreFunds();
error RandomNFT_RangeOurOfBounds();
error RandomNFT_TransferFailed();

// Contract

contract RandomNFTForRemix is ERC721URIStorage, VRFConsumerBaseV2, Ownable {
    // Breed Types for NFTs
    enum Breed {
        shiny,
        natural,
        artificial
    }

    // Chainlink VRF Variables
    VRFCoordinatorV2Interface private immutable i_vrfCoordinator;
    uint64 private immutable i_suscriptionId;
    bytes32 private immutable i_gasLane;
    uint32 private immutable i_callbackGasLimit;
    uint16 private constant REQUEST_CONFIRMATIONS = 3;
    uint32 private constant NUM_WORDS = 1;

    // NFT Variables
    uint256 private immutable i_mintFee;
    uint256 private s_tokenCounter;
    uint256 internal constant MAX_CHANCE_VALUE = 100;
    string[] internal s_nftTokenUris;
    bool private s_initialized;

    // Helpers for Chainlink VRF
    mapping(uint256 => address) public s_requestIdToSender;

    // Events
    event NftRequested(uint256 indexed requestId, address requester);
    event NftMinted(Breed breed, address minter);

    // Variables for REMIX
    address public vrfCoordinatorV2 =
        0x2Ca8E0C643bDe4C2E08ab1fA0da3401AdAD7734D;
    bytes32 public gasLane =
        0x79d3d8832d904592c0bf9818b621522c988bb8b0c05cdc3b15aea1b6e8db0c15;
    uint256 public mintFee = 0.001 ether;
    uint32 public callbackGasLimit = 500000;
    string[3] public nftUris = ["", "", ""];

    // Constructor with all the paremeter needed for Chainlink VRF and UriStorage NFTs
    constructor(uint64 suscriptionId)
        VRFConsumerBaseV2(vrfCoordinatorV2)
        ERC721("Random IPFS NFT Moralis", "RMN")
    {
        i_vrfCoordinator = VRFCoordinatorV2Interface(vrfCoordinatorV2);
        i_gasLane = gasLane;
        i_suscriptionId = suscriptionId;
        i_mintFee = mintFee;
        i_callbackGasLimit = callbackGasLimit;
        _initializeContract(nftUris);
        s_tokenCounter = 0;
    }

    // Request NFT based on the requestID
    function requestNft() public payable returns (uint256 requestId) {
        if (msg.value < i_mintFee) {
            revert RandomNFT_NeedMoreFunds();
        }
        requestId = i_vrfCoordinator.requestRandomWords(
            i_gasLane,
            i_suscriptionId,
            REQUEST_CONFIRMATIONS,
            i_callbackGasLimit,
            NUM_WORDS
        );

        s_requestIdToSender[requestId] = msg.sender;
        emit NftRequested(requestId, msg.sender);
    }

    // Fulfill Chainlink Randomness Request

    function fulfillRandomWords(uint256 requestId, uint256[] memory randomWords)
        internal
        override
    {
        address nftOwner = s_requestIdToSender[requestId];
        uint256 newItemId = s_tokenCounter;
        s_tokenCounter = s_tokenCounter + 1;
        uint256 moddedRng = randomWords[0] % MAX_CHANCE_VALUE;
        Breed nftBreed = getBreedFromModdedRng(moddedRng);
        _safeMint(nftOwner, newItemId);
        _setTokenURI(newItemId, s_nftTokenUris[uint256(nftBreed)]);
        emit NftMinted(nftBreed, nftOwner);
    }

    // Get the Change to get a specific Breed
    function getChanceArray() public pure returns (uint256[3] memory) {
        return [20, 50, MAX_CHANCE_VALUE];
    }

    // Initialize Contract

    function _initializeContract(string[3] memory nftTokenUris) private {
        if (s_initialized) {
            revert RandomNFT_AllreadyInitialized();
        }
        s_nftTokenUris = nftTokenUris;
        s_initialized = true;
    }

    // Get Breed from Modded RNG
    function getBreedFromModdedRng(uint256 moddedRng)
        public
        pure
        returns (Breed)
    {
        uint256 totalSum = 0;
        uint256[3] memory changeArray = getChanceArray();
        for (uint256 i = 0; i < changeArray.length; i++) {
            if (moddedRng >= totalSum && moddedRng < changeArray[i]) {
                return Breed(i);
            }
            totalSum += changeArray[i];
        }
        revert RandomNFT_RangeOurOfBounds();
    }

    // Withdraw Funds
    function withdraw() public onlyOwner {
        uint256 balance = address(this).balance;
        (bool success, ) = payable(msg.sender).call{value: balance}("");
        if (!success) {
            revert RandomNFT_TransferFailed();
        }
    }

    // Getters
    function getMintFee() public view returns (uint256) {
        return i_mintFee;
    }

    function getNftTokenUris(uint256 index)
        public
        view
        returns (string memory)
    {
        return s_nftTokenUris[index];
    }

    function getInitialized() public view returns (bool) {
        return s_initialized;
    }

    function getTokenCounter() public view returns (uint256) {
        return s_tokenCounter;
    }
}
