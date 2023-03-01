from flask import Flask, request
from moralis import auth
from flask_cors import CORS
from dotenv import load_dotenv
import requests
import os


load_dotenv()

app = Flask(__name__)
CORS(app)

api_key = os.getenv("MORALIS_API_KEY")


@app.route("/requestChallenge/", methods=["GET"])
def reqChallenge():

    args = request.args

    url = "https://authapi.moralis.io/challenge/request/aptos"

    payload = {
        "domain": "amazingdomain.dapp",
        "statement": "Please confirm you want to log in",
        "uri": "https://amazingdomain.dapp/",
        "expirationTime": "2023-03-10T00:00:00.000Z",
        "notBefore": "2020-01-01T00:00:00.000Z",
        "timeout": 30,
        "chainId": 1,
        "address": args.get("address"),
        "publicKey": args.get("publicKey"),
    }
    headers = {
        "Accept": "application/json",
        "Content-Type": "application/json",
        "X-API-Key": api_key,
    }

    response = requests.request("POST", url, json=payload, headers=headers)

    return response.text


@app.route("/verifyChallenge/", methods=["GET"])
def verifyChallenge():

    args = request.args

    url = "https://authapi.moralis.io/challenge/verify/aptos"

    payload = {
        "message": args.get("message"),
        "signature": args.get("signature"),
    }
    headers = {
        "Accept": "application/json",
        "Content-Type": "application/json",
        "X-API-Key": api_key,
    }

    response = requests.request("POST", url, json=payload, headers=headers)

    return response.text


if __name__ == "__main__":
    app.run(host="127.0.0.1", port=3000, debug=True)
