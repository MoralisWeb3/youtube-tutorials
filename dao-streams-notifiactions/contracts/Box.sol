//SPDX-License-Identifier: MIT

pragma solidity ^0.8.0;

import "@openzeppelin/contracts/access/Ownable.sol";

contract Box is Ownable {
    uint256 public value;

    event newValueStored(uint256 value);

    function store(uint256 newValue) public {
        value = newValue;
        emit newValueStored(value);
    }

    function getValue() public view returns (uint256) {
        return value;
    }
}
