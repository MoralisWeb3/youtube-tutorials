from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def get_transfers_by_block(block_number, chain, limit):

    params = {
        "block_number_or_hash": block_number,
        "chain": chain,
        "subdomain": "",
        "limit": limit,
        "cursor": "",
    }

    result = evm_api.nft.get_nft_transfers_by_block(
        api_key=api_key,
        params=params,
    )

    return result
