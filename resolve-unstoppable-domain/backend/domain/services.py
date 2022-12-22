from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def resolve_unstoppable_domain(domain, currency):

    params = {
        "domain": domain,
        "currency": currency,
    }

    result = evm_api.resolve.resolve_domain(
        api_key=api_key,
        params=params,
    )

    return result
