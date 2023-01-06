from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def get_user_nfts(address: str, chain: str, cursor: str = ""):

    params = {
        "address": address,
        "chain": chain,
        "format": "decimal",
        "limit": 10,
        "token_addresses": [],
        "cursor": cursor,
        "normalizeMetadata": True,
    }

    result = evm_api.nft.get_wallet_nfts(
        api_key=api_key,
        params=params,
    )

    return result
