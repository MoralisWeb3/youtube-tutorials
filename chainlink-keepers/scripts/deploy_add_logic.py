from brownie import AddNumLogic, network, config
from scripts.helpful_scripts import get_account


def deploy_num_logic():
    account = get_account()
    num = AddNumLogic.deploy(1,
                             {"from": account}, publish_source=config["networks"][network.show_active()].get("verify", False))
    print(f"Contract deployed to {num.address}")


def main():
    deploy_num_logic()
