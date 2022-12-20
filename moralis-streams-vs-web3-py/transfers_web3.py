from web3 import Web3
import time
import json

infura_http_ulr = "Your Infura Key"
web3 = Web3(Web3.HTTPProvider(infura_http_ulr))
usdt_address = "0xdAC17F958D2ee523a2206206994597C13D831ec7"
with open("abi.json") as f:
    abi = json.load(f)

# New web3 Contract instance
contract = web3.eth.contract(address=usdt_address, abi=abi)

# get transfer events


def handle_event(event):
    receipt = web3.eth.getTransactionReceipt(event["transactionHash"])
    result = contract.events.Transfer().processReceipt(receipt)
    value = result[0]["args"]["value"]
    from_address = result[0]["args"]["from"]
    to_address = result[0]["args"]["to"]
    tx_hash = result[0]["transactionHash"].hex()
    print(
        {
            f"from: {from_address} to: {to_address} value: ${value}, tx_hash: {tx_hash}",
        }
    )


def log_loop(event_filter, poll_interval):
    while True:
        for event in event_filter.get_new_entries():
            handle_event(event)
            time.sleep(poll_interval)


block_filter = web3.eth.filter({"fromBlock": "latest", "address": usdt_address})
log_loop(block_filter, 2)
