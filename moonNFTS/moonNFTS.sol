// SPDX-License-Identifier: MIT
pragma solidity ^0.8.3;

import "github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/utils/Counters.sol";
import "github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/token/ERC721/extensions/ERC721URIStorage.sol";
import "github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/token/ERC721/ERC721.sol";
import "github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/utils/Strings.sol";

contract NFT is ERC721URIStorage {
    using Counters for Counters.Counter;
    Counters.Counter private _tokenIds;
    address public owner;


    constructor() ERC721("Moon NFTS", "Moon") {
        owner = msg.sender;
    }

    function createToken() public returns (uint) {
        _tokenIds.increment();
        uint256 newItemId = _tokenIds.current();
        string memory tokenURI = string(abi.encodePacked(
            "https://firebasestorage.googleapis.com/v0/b/moonnfts-d3cbe.appspot.com/o/metadata%2F", 
            Strings.toString(newItemId), 
            ".json?alt=media"
        ));

        _mint(msg.sender, newItemId);
        _setTokenURI(newItemId, tokenURI);
        return newItemId;
    }
}