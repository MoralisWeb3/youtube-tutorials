from moralis import evm_api
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")


def get_block_by_date(date, chain):

    params = {
        "date": date,
        "chain": chain,
    }

    result = evm_api.block.get_date_to_block(
        api_key=api_key,
        params=params,
    )

    return result
