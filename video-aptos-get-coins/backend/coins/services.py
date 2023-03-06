import requests
from dotenv import load_dotenv
import os

load_dotenv()

api_key = os.getenv("MORALIS_API_KEY")


def get_latest_coins(limit: str):

    url = f"https://mainnet-aptos-api.moralis.io/coins/latest?limit={limit}"

    headers = {
        "Accept": "application/json",
        "X-API-Key": api_key,
    }

    response = requests.request("GET", url, headers=headers)

    return response.text
