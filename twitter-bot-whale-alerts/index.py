import json
from flask import Flask, request
import locale
from twitter_bot import send_tweet

locale.setlocale(locale.LC_ALL, 'en_US.UTF-8')


app = Flask(__name__)


@app.route("/webhook", methods=["POST"])
def webhook():
    # Process the request data here

    webhook = request.data.decode("utf-8")
    json_object = json.loads(webhook)
    try:
        transfer = json_object["erc20Transfers"][0]
    except IndexError:
        return "OK"
    sender = transfer["from"]
    receiver = transfer["to"]
    value = transfer["value"]
    token_name = transfer["tokenName"]
    transaction_hash = transfer["transactionHash"]

    handle_response_and_tweet(sender, receiver, value,
                              token_name, transaction_hash)

    return "OK"


def handle_response_and_tweet(sender, receiver, value, token_name, transaction_hash):
    sender = sender[:6] + "..." + sender[-3:] + "..."
    receiver = receiver[:6] + "..." + receiver[-3:] + "..."
    value = "${:,.6f}".format(float(value)/1000000)
    transaction_hash = 'https://etherscan.io/tx/' + transaction_hash

    tweet = f"New Whale Alert! {sender} sent {value} {token_name} to {receiver}! in transaction {transaction_hash}"

    send_tweet(tweet)


if __name__ == "__main__":
    app.run(port=5002)
