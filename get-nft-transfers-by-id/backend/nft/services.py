from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def get_transfers_by_id(chain, address, token_id):

    params = {
        "address": address,
        "token_id": token_id,
        "chain": chain,
        "format": "decimal",
        "limit": 10,
        "cursor": "",
    }

    result = evm_api.nft.get_nft_transfers(
        api_key=api_key,
        params=params,
    )

    return result
