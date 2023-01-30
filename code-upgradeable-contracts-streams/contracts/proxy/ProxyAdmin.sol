//SPDX-License-Identifier: MIT

pragma solidity ^0.8.0;

import "./UpgradeableProxy.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract ProxyAdmin is Ownable {
    // New upgrade made on contract.
    event Upgraded(address indexed implementation);

    function getProxyImplementation(UpgradeableProxy proxy)
        public
        view
        virtual
        returns (address)
    {
        // bytes4(keccak256("implementation()")) == 0x5c60da1b
        (bool success, bytes memory returndata) = address(proxy).staticcall(
            hex"5c60da1b"
        );
        require(success);
        return abi.decode(returndata, (address));
    }

    function getProxyAdmin(UpgradeableProxy proxy)
        public
        view
        virtual
        returns (address)
    {
        // bytes4(keccak256("admin()")) == 0xf851a440
        (bool success, bytes memory returndata) = address(proxy).staticcall(
            hex"f851a440"
        );
        require(success);
        return abi.decode(returndata, (address));
    }

    function changeProxyAdmin(UpgradeableProxy proxy, address newAdmin)
        public
        virtual
        onlyOwner
    {
        proxy.changeAdmin(newAdmin);
    }

    function upgrade(UpgradeableProxy proxy, address implementation)
        public
        virtual
        onlyOwner
    {
        proxy.upgradeTo(implementation);
        emit Upgraded(implementation);
    }

    function upgradeAndCall(
        UpgradeableProxy proxy,
        address implementation,
        bytes memory data
    ) public payable virtual onlyOwner {
        proxy.upgradeToAndCall{value: msg.value}(implementation, data);
        emit Upgraded(implementation);
    }
}
