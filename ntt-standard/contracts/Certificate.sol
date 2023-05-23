//SPDX-License-Identifier: MIT

pragma solidity ^0.8.0;

import "@openzeppelin/contracts/access/Ownable.sol";
import "./ERC4671.sol";

contract Certificate is ERC4671, Ownable {
    constructor(
        string memory _courseName,
        string memory _tokenName
    ) ERC4671(_courseName, _tokenName) {}

    // mapping
    mapping(address => uint256) public studentToId;

    // event
    event TokenMinted(address indexed _owner, uint256 indexed _tokenId);

    // Mint a token for a specific address
    function createCertificate(
        address _student,
        string memory _tokenUri
    ) public onlyOwner {
        _mint(_student);
        studentToId[_student] = emittedCount() - 1;
        setTokenURI(studentToId[_student], _tokenUri);
        emit TokenMinted(_student, studentToId[_student]);
    }
}
