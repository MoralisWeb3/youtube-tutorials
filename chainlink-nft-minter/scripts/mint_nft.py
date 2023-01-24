from brownie import RandomNFT
from scripts.helpful_scripts import get_account


def mint_nft():
    account = get_account()
    random_nft = RandomNFT[-1]
    mint_fee = random_nft.getMintFee()
    transaction = random_nft.requestNft(
        {'from': account, 'value': mint_fee})
    transaction.wait(1)
    print("Minted NFT!")


def main():
    mint_nft()
