from moralis import streams
import json
from dotenv import load_dotenv
import os

load_dotenv()
api_key = os.getenv("MORALIS_API_KEY")

url = "https://5bc9-2800-bf0-140-d6b-80f9-c948-c8d6-d6a.ngrok.io/webhook/"

with open("abi.json") as f:
    abi = json.load(f)

body = {
    "webhookUrl": url,
    "description": "usdt50 transfers",
    "tag": "usdt50",
    "topic0": ["Transfer(address,address,uint256)"],
    "allAddresses": False,
    "includeNativeTxs": False,
    "includeContractLogs": True,
    "includeInternalTxs": False,
    "abi": abi,
    "advancedOptions": [
        {
            "topic0": "Transfer(address,address,uint256)",
            "filter": {"gt": ["value", "50000000000"]},
        }
    ],
    "chainIds": ["0x1"],
}

result = streams.evm.create_stream(
    api_key=api_key,
    body=body,
)

print(result)
stream_id = result["id"]

params = {
    "id": stream_id,
}

address_body = {
    # USDT Address
    "address": "0xdAC17F958D2ee523a2206206994597C13D831ec7",
}

result2 = streams.evm.add_address_to_stream(
    api_key=api_key,
    params=params,
    body=address_body,
)

print(result2)
