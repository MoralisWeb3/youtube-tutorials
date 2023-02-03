from moralis import evm_api
import os
from dotenv import load_dotenv

load_dotenv()

api_key = os.getenv("MORALIS_API_KEY")


def get_nft_owners(address: str):

    params = {
        "address": address,
        "chain": "palm",
        "format": "decimal",
        "limit": 100,
        "cursor": "",
        "normalizeMetadata": True,
    }

    result = evm_api.nft.get_nft_owners(
        api_key=api_key,
        params=params,
    )

    return result
