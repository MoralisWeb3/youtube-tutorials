from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def get_nft_transfers(chain, address, limit):

    params = {
        "address": address,
        "chain": chain,
        "format": "decimal",
        "direction": "both",
        "from_block": 1,
        "to_block": "9007199254740991",
        "limit": limit,
        "cursor": "",
    }

    result = evm_api.nft.get_wallet_nft_transfers(
        api_key=api_key,
        params=params,
    )

    return result
