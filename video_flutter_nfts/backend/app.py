from flask import Flask, request
from dotenv import load_dotenv
from moralis import evm_api
import json
import os

load_dotenv()

app = Flask(__name__)
api_key = os.getenv('MORALIS_API_KEY')


@app.route('/get_user_nfts', methods=['GET'])
def get_nfts():
    address = request.args.get('address')
    chain = request.args.get('chain')
    params = {
        "address": address,
        "chain": chain,
        "format": "decimal",
        "limit": 100,
        "token_addresses": [],
        "cursor": "",
        "normalizeMetadata": True,
    }

    result = evm_api.nft.get_wallet_nfts(
        api_key=api_key,
        params=params,
    )

    # converting it to json because of unicode characters
    response = json.dumps(result, indent=4)
    return response


if __name__ == "__main__":
    app.run(host='0.0.0.0', port=5002)
