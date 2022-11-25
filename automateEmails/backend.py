from flask import Flask
from flask import request
from flask import jsonify
from flask_cors import CORS
from email.message import EmailMessage
import ssl
import smtplib

emailPass = ""
email = ""
emailReceiver = ""
subject="New Donation"

app = Flask(__name__)
CORS(app)

@app.route('/streams', methods=["POST"])
def streams():

    if (request.json['confirmed']):
        return jsonify(success=True)


    details = request.json["txs"]

    for donation in details:

        amount = int(donation['value'])/1000000000000000000
        em = EmailMessage()
        em['From'] = email
        em['To'] = emailReceiver
        em['Subject'] = subject

        em.set_content(donation['fromAddress'] + " has just sent you " + str(amount) + " in MATIC!")

        context = ssl.create_default_context()

        with smtplib.SMTP_SSL('smtp.gmail.com', 465, context=context) as smtp:
            smtp.login(email, emailPass)
            smtp.sendmail(email, emailReceiver, em.as_string())
    
    print("Email Sent")

    return jsonify(success=True)


if __name__ == "__main__":
    app.run(host="127.0.0.1", port=3000, debug=True)