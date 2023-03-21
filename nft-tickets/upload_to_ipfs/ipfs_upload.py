from moralis import evm_api
import json
import base64
import os
from ipfs_img import upload_images

api_key = os.getenv('MORALIS_API_KEY')

names = ['Legendary Concert Ticket']
descriptions = [
    'This is a Legendary Concert Ticket. It is a rare item that can be used to attend a concert.',
]
ipfs_urls = upload_images()
print(ipfs_urls)
Types = ['Concert']
Genre = ['Rock']
Rewards = ['150']


def upload_metadata():

    ipfs_metadatas = []

    for i in range(len(ipfs_urls)):
        content = {
            "name": names[i],
            "description": descriptions[i],
            "image": ipfs_urls[i],
            "attributes": [
                {"trait_type": "Type", "value": Types[i]},
                {"trait_type": "Genre", "value": Genre[i]},
                {"trait_type": "Reward", "value": Rewards[i]},
            ],
        }

        body = [
            {
                "path": "metadata.json",
                "content": base64.b64encode(bytes(json.dumps(content), "ascii")).decode("ascii"),
            },
        ]

        result = evm_api.ipfs.upload_folder(
            api_key=api_key,
            body=body,
        )

        ipfs_metadatas.append(result[0]['path'])

        print(result)

    ipfs_addresses = {'addresses': ipfs_metadatas}

    # Write the dictionary to a JSON
    with open('ipfs_addresses.json', 'w') as f:
        json.dump(ipfs_addresses, f)

    # Verify the data has been saved on the JSON
    with open('ipfs_addresses.json', 'r') as f:
        data = json.load(f)
        print(data)


upload_metadata()
