from flask import Flask, request, jsonify
from dotenv import load_dotenv
from moralis import evm_api
import os

load_dotenv()

app = Flask(__name__)
api_key = os.getenv("MORALIS_API_KEY")


@app.route("/get_user_nfts", methods=["GET"])
def get_nfts():
    address = request.args.get("address")
    params = {
        "address": address,
        "chain": "goerli",
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

    return jsonify(result)


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5002)
