from brownie import PalmNFT, network, config
from scripts.helpful_scripts import get_account


IPFS_URIS = [
    "https://ipfs.moralis.io:2053/ipfs/QmXahbHcPJa4BfxCsqNhXoabrSgQkZFwCv8CJtxUoEE1Pp/metadata.json",
    "https://ipfs.moralis.io:2053/ipfs/QmVKHak29AGZsqRMKqyWagF11eRuJou5nHXK4JqpXaYSu5/metadata.json",
    "https://ipfs.moralis.io:2053/ipfs/QmQLURsJjNZyi2mCymTf9WsQXFyXpJnCqFC5bgkNYbLPen/metadata.json"
]


def mint_nft():
    account = get_account()
    nft = PalmNFT[-1]
    transaction = nft.mintViking(IPFS_URIS[0], {'from': account})
    transaction.wait(1)
    print("Minted NFT!")


def main():
    mint_nft()
