from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def get_nft_contract_transfers(address, chain, limit):

    params = {
        "address": address,
        "chain": chain,
        "format": "decimal",
        "limit": limit,
        "cursor": "",
    }

    result = evm_api.nft.get_nft_contract_transfers(
        api_key=api_key,
        params=params,
    )

    return result
