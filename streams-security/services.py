from web3 import Web3
import json
import os
from dotenv import load_dotenv


load_dotenv()
api_key = os.getenv("MORALIS_KEY")


def verify_signature(request):
    signature = request.headers["x-signature"]
    if not signature:
        raise Exception("No signature found")
    data = request.data.decode("utf-8")
    json_data = json.dumps(data, separators=(",", ":"))

    text = json.loads(json_data) + api_key

    # # Generate signature
    generated_signature = Web3.sha3(text=text)
    string_signature = generated_signature.hex()

    if string_signature != signature:
        raise Exception("Invalid signature")
