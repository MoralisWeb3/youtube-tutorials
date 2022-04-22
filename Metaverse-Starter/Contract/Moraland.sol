// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;
import "@openzeppelin/contracts/token/ERC721/ERC721.sol";
import "@openzeppelin/contracts/utils/Counters.sol";
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721URIStorage.sol";

contract Moraland is ERC721URIStorage{
    constructor() ERC721("Moraland", "MLND") {}

    event Assigned(uint256 indexed tokenId, address indexed assignee, bytes bytesId);

    function assign(string calldata tokenURI, bytes calldata bytesId) public {
        uint256 _tokenId = abi.decode(bytesId, (uint256));
        _mint(msg.sender, _tokenId);
        _setTokenURI(_tokenId, tokenURI);
        emit Assigned(_tokenId, msg.sender, bytesId);
    }
    
    
    function exist(bytes calldata bytesId) public view returns (bool){
        uint256 _tokenId = abi.decode(bytesId, (uint256));
        return _exists(_tokenId);
    }
}