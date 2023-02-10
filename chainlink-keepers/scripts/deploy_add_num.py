from brownie import AddNum, network, config
from scripts.helpful_scripts import get_account


def deploy_num():
    account = get_account()
    num = AddNum.deploy(
        {"from": account}, publish_source=config["networks"][network.show_active()].get("verify", False))
    print(f"Contract deployed to {num.address}")


def main():
    deploy_num()
