from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()

api_key = os.getenv("MORALIS_API_KEY")

def get_nft_metadata(address, token_id, chain):
    params = {
        "address": address,
        "token_id": token_id,
        "chain": chain,
        "format": "decimal",
        "normalizeMetadata": True,
    }

    result = evm_api.nft.get_nft_metadata(
        api_key=api_key,
        params=params,
    )

    return result
