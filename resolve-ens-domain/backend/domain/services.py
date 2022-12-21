from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()

api_key = os.getenv("MORALIS_API_KEY")


def resolve_ens_domain(address):

    params = {
        "address": address,
    }

    result = evm_api.resolve.resolve_address(
        api_key=api_key,
        params=params,
    )

    return result
