from flask import Flask, request
from dotenv import load_dotenv
from moralis import evm_api
import json
import locale
import os

load_dotenv()


api_key = os.getenv("MORALIS_API_KEY")

locale.setlocale(locale.LC_ALL, "en_US.UTF-8")
app = Flask(__name__)


@app.route("/get_transaction", methods=["GET"])
def transactions():
    chain = request.args.get("chain")
    address = request.args.get("address")
    params = {
        "chain": chain,
        "address": address,
    }

    result = evm_api.transaction.get_wallet_transactions(api_key=api_key, params=params)

    return json.dumps(result)


if __name__ == "__main__":
    app.run(port=5002, host="0.0.0.0", debug=True)
