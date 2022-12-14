from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def get_multiple_nfts(chain, tokens):

    params = {
        "chain": chain,
    }
    body = {
        "tokens": tokens,
        "normalizeMetadata": True,
    }

    result = evm_api.nft.get_multiple_nfts(
        api_key=api_key,
        params=params,
        body=body,
    )

    return result
