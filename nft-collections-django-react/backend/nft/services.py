from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def get_nft_collections(chain=None, address=None, limit=10):

    params = {
        "address": address,
        "chain": chain,
        "limit": limit,
        "cursor": "",
    }

    result = evm_api.nft.get_wallet_nft_collections(
        api_key=api_key,
        params=params,
    )

    return result
