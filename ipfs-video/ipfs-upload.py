from moralis import evm_api
import json
import base64

api_key = "your api key"

content = {
    "name": "Moralis Mage",
    "description": "The Ultimate Moralis NFT",
    "image": "https://ipfs.moralis.io:2053/ipfs/QmNf6caHKrK6AUCrJjY1ALCMSnSGD3TRFPyShWp5yUkL23/moralis/logo.jpg",
    "attributes": [
        {"trait_type": "Hat", "value": "Mage"},
        {"trait_type": "Eyes", "value": "Closed"},
        {"trait_type": "Beard", "value": "Orange"},
    ],
}

body = [
    {
        "path": "metadata.json",
        "content": base64.b64encode(bytes(json.dumps(content), "ascii")).decode(
            "ascii"
        ),
    }
]


result = evm_api.ipfs.upload_folder(
    api_key=api_key,
    body=body,
)

print(result)
