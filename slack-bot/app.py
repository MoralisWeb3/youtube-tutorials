from flask import Flask, request
from dotenv import load_dotenv
import json
import locale
import os

from slack_sdk import WebClient
from slack_sdk.errors import SlackApiError

load_dotenv()

locale.setlocale(locale.LC_ALL, 'en_US.UTF-8')
app = Flask(__name__)


SLACK_BOT_TOKEN = os.getenv('SLACK_BOT_TOKEN')
SLACK_CHANNEL = os.getenv('SLACK_CHANNEL')

slack_client = WebClient(token=SLACK_BOT_TOKEN)


@app.route('/webhook', methods=['POST'])
def webhook():
    webhook = request.data.decode('utf-8')
    json_object = json.loads(webhook)
    try:
        transfer = json_object['erc20Transfers'][0]
    except IndexError:
        return 'No transfers'

    sender = transfer["from"]
    receiver = transfer["to"]
    value = transfer["value"]
    token_name = transfer["tokenName"]
    transaction_hash = transfer["transactionHash"]

    handle_response_and_chat(sender, receiver, value,
                             token_name, transaction_hash)

    return 'OK'


def send_massage(channel, attachment):
    try:
        response = slack_client.chat_postMessage(
            channel=channel,
            attachments=[attachment]
        )
        print(f"Message sent to channel {channel}")
    except SlackApiError as e:
        print(f"Error: {e}")


def handle_response_and_chat(sender, receiver, value, token_name, transaction_hash):
    sender = sender[:7] + "..." + sender[-3:]
    receiver = receiver[:7] + "..." + receiver[-3:]
    value = "${:,.6f}".format(float(value)/1000000)
    transaction_hash = 'https://etherscan.io/tx/' + transaction_hash

    attachment = {
        "fallback": f"Transfer Details:\nSender: {sender}\nReceiver: {receiver}\nValue: {value}\nToken Name: {token_name}\nTransaction Hash: {transaction_hash}",
        "color": "#36a64f",
        "pretext": "New Whale Transfer!",
        "fields": [
            {
                "title": "Sender",
                "value": sender,
                "short": True
            },
            {
                "title": "Receiver",
                "value": receiver,
                "short": True
            },
            {
                "title": "Value",
                "value": value,
                "short": True
            },
            {
                "title": "Token Name",
                "value": token_name,
                "short": True
            },
            {
                "title": "Transaction Hash",
                "value": f"<{transaction_hash}|{transaction_hash}>",
                "short": False
            }
        ]


    }

    send_massage(SLACK_CHANNEL, attachment)


if __name__ == "__main__":
    app.run(port=5002)
