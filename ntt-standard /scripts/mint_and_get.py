from brownie import Certificate
from scripts.helpful_scripts import get_account
import random


uris = [
    [
        "https://ipfs.moralis.io:2053/ipfs/QmYEZjpkB1saZvShzJK2DybXHU4NvTjNQcwV4yBygdhjkH/metadata.json",
        "https://ipfs.moralis.io:2053/ipfs/QmSJUHUKMbtVeq7enMBLBkUWp1xGdHUMGf5NjBXoPCi4mM/metadata.json",
        "https://ipfs.moralis.io:2053/ipfs/QmaoYEuBqYPXFXoDpfYhSFMLZAAPo44bRzXfvsaBt4eSGu/metadata.json",
    ]
]


def mint_new_certificate():
    account = get_account()
    certificate = Certificate[-1]
    token_uri = random.choice(uris)
    certificate.createCertificate(account.address, token_uri, {"from": account})
    print(f"Certificate minted for {account.address}")


def get_ntts():
    certificate = Certificate[-1]
    account = get_account()
    ntts = certificate.tokensOfOwner(account.address)
    for element in ntts:
        print(f"Certificate URI: {certificate.tokenURI(element)}")
        print(f"Certificate owner: {certificate.ownerOf(element)}")
        print(f"Certificate token id: {element}")


def main():
    mint_new_certificate()
    get_ntts()


# tokensOfOwner
