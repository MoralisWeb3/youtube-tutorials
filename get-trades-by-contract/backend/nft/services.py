from moralis import evm_api
from dotenv import load_dotenv
from web3 import Web3
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")
infura_endpoint = os.getenv("INFURA_WEB3_PROVIDER")


def get_nft_trades(address: str, chain: str, limit: int):
    params = {
        "address": address,
        "chain": chain,
        "from_block": 500000,
        "to_block": get_latest_block(),
        "from_date": "",
        "to_date": "",
        "marketplace": "opensea",
        "cursor": "",
        "limit": limit,
    }

    result = evm_api.nft.get_nft_trades(
        api_key=api_key,
        params=params,
    )

    return result


def get_latest_block():

    w3 = Web3(Web3.HTTPProvider(infura_endpoint))
    block = str(w3.eth.get_block("latest")["number"])
    return block
