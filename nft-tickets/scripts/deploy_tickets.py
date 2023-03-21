from brownie import TicketNFT, network, config
from scripts.helpful_scripts import get_account


def deploy_random_nft():
    account = get_account()
    nft_contract = TicketNFT.deploy(
        {"from": account})
    print(nft_contract.address)


def main():
    deploy_random_nft()
