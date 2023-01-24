from brownie import VRFv2Consumer, network, config, RandomNFT
from scripts.helpful_scripts import get_account
from web3 import Web3

IPFS_URLS = ['https://ipfs.moralis.io:2053/ipfs/QmcUQMKe6DbEjeX5zhR4nxNuwF3VAMw9fGtFpfPhPQMZLs/metadata.json',
             'https://ipfs.moralis.io:2053/ipfs/QmUpvNMHvXasC5gkQHEC7kVWhSU6p1edapdFKU7ZvzCMik/metadata.json',
             'https://ipfs.moralis.io:2053/ipfs/QmWpZWfmFT9gjjvkYfYnaeQxzXbDigAfAgTa78Y7Y24Dp5/metadata.json'
             ]


MINT_FEE = Web3.toWei(0.001, "ether")


def deploy_random_nft():
    account = get_account()
    vrf_coordinator = config["networks"][network.show_active(
    )]["vrf_coordinator_v2"]
    subscription_id = config["networks"][network.show_active()
                                         ]["subscriptionId"]
    gas_lane = config["networks"][network.show_active()]["gasLane"]
    call_back_gas_limit = config["networks"][network.show_active(
    )]["callBackGasLimit"]
    random_nft = RandomNFT.deploy(
        vrf_coordinator, subscription_id, gas_lane, MINT_FEE, call_back_gas_limit, IPFS_URLS,
        {'from': account}, publish_source=config["networks"][network.show_active()].get("verify", False))
    print(random_nft.address)
    return random_nft


def main():
    deploy_random_nft()
