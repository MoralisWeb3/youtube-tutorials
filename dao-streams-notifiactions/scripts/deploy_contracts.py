from brownie import Box, GovernanceToken, MyDao, config, network
from scripts.helpful_scripts import get_account
from web3 import Web3

initial_supply = Web3.toWei(1000, "ether")


def deploy_contracts():
    account = get_account()
    box = Box.deploy({"from": account}, publish_source=config['networks'][network.show_active()].get(
        'verify', False))
    print(box.address)
    governance_token = GovernanceToken.deploy(
        initial_supply, {"from": account}, publish_source=config['networks'][network.show_active()].get(
            'verify', False))
    print(governance_token.address)
    tx = governance_token.transfer(
        account, Web3.toWei(10, "ether"), {"from": account})
    tx.wait(1)

    my_dao = MyDao.deploy(governance_token.address,
                          box.address, {"from": account}, publish_source=config['networks'][network.show_active()].get(
                              'verify', False))
    print(my_dao.address)

    tx10 = box.transferOwnership(my_dao.address, {"from": account})
    tx10.wait(1)


def main():
    deploy_contracts()
