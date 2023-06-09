import requests
import json

# API endpoint
url = "http://192.168.100.47:5002/get_transaction"

# Transaction parameters
params = {
    "chain": "sepolia",
    "address": "0xe9300E469DF57989d6A0fD9db9d7ce3c48324789",
}

# Send a GET request
response = requests.get(url, params=params)

# Ensure the request was successful
if response.status_code == 200:
    # Load the JSON response
    data = json.loads(response.text)

    # Iterate over each transaction
    for transaction in data["result"]:
        # Print gas details
        gas = transaction["gas"]
        gas_price = transaction["gas_price"]
        gas_used = transaction["receipt_gas_used"]

        print(f'Transaction Hash: {transaction["hash"]}')
        print(f"Gas: {gas}")
        print(f"Gas Price: {gas_price}")
        print(f"Gas Used: {gas_used}")
        print(f"Total Gas Paid (in Wei): {int(gas_used) * int(gas_price)}")
        print("-----------------------------")
else:
    print("Error:", response.status_code)
