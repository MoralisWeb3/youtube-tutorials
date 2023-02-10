//SPDX-License-Identifier: MIT

pragma solidity ^0.8.0;

contract AddNum {
    event NumberAdded(uint256 number);

    uint256 public number;

    function add() public {
        number += 1;
        emit NumberAdded(number);
    }

    function getNumber() public view returns (uint256) {
        return number;
    }
}
