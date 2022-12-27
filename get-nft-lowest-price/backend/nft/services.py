from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def get_nft_lowest_price(address, chain):

    params = {
        "address": address,
        "chain": chain,
        "days": 365,
        "marketplace": "opensea",
    }

    result = evm_api.nft.get_nft_lowest_price(
        api_key=api_key,
        params=params,
    )

    return result
