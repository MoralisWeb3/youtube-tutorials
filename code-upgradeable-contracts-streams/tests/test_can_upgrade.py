import pytest
from brownie import (
    BoxV1,
    BoxV2,
    UpgradeableProxy,
    ProxyAdmin,
    Contract,
    network,
    config,
    exceptions,
)
from scripts.helpful_scripts import get_account, encode_function_data, upgrade


def test_proxy_upgrades():
    account = get_account()
    box = BoxV1.deploy(
        {"from": account},
    )
    proxy_admin = ProxyAdmin.deploy(
        {"from": account},
    )
    box_encoded_initializer_function = encode_function_data()
    proxy = UpgradeableProxy.deploy(
        box.address,
        proxy_admin.address,
        box_encoded_initializer_function,
        {"from": account, "gas_limit": 1000000},
    )
    box_v2 = BoxV2.deploy(
        {"from": account},
    )
    proxy_box = Contract.from_abi("BoxV2", proxy.address, BoxV2.abi)
    # with pytest.raises(exceptions.VirtualMachineError):
    #     proxy_box.increment({"from": account})
    upgrade(account, proxy, box_v2, proxy_admin_contract=proxy_admin)
    print(f"Initial value {proxy_box.getValue() }")
    assert proxy_box.getValue() == 0
    proxy_box.increment({"from": account})
    print(f"Final value {proxy_box.getValue() }")
    assert proxy_box.getValue() == 1
