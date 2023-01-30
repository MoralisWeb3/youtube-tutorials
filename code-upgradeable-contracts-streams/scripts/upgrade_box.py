from brownie import BoxV2, UpgradeableProxy, ProxyAdmin, config, network, Contract
from scripts.helpful_scripts import get_account, upgrade


def main():
    account = get_account()
    print(f"Deploying to {network.show_active()}")
    box_v2 = BoxV2.deploy(
        {"from": account}, publish_source=config["networks"][network.show_active()].get("verify", False))
    proxy = UpgradeableProxy[-1]
    proxy_admin = ProxyAdmin[-1]
    upgrade_tx = upgrade(account, proxy, box_v2,
                         proxy_admin_contract=proxy_admin)
    upgrade_tx.wait(1)
    print("Proxy upgraded to V2")
    proxy_box = Contract.from_abi("BoxV2", proxy.address, BoxV2.abi)
    print(f"Starting value of proxy box is {proxy_box.getValue()}")
    tx = proxy_box.increment({"from": account})
    tx.wait(1)
    print(f"Ending value of proxy box is {proxy_box.getValue()}")
