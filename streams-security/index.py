from flask import Flask, request
from services import verify_signature
import json

app = Flask(__name__)


@app.route("/webhook", methods=["POST"])
def webhook():
    # Process the request data here

    verify_signature(request)
    webhook = request.data.decode("utf-8")
    json_webhook = json.dumps(webhook, separators=(",", ":"))
    json_object = json.loads(json_webhook)
    print(json_object)

    return "OK"


if __name__ == "__main__":
    app.run(port=5002)
