//SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract BoxV2 {
    uint256 private value;

    event valueChanged(uint256 _value);

    function setValue(uint256 _value) public {
        value = _value;
        emit valueChanged(_value);
    }

    function getValue() public view returns (uint256) {
        return value;
    }

    function increment() public {
        value = value + 1;
        emit valueChanged(value);
    }
}
