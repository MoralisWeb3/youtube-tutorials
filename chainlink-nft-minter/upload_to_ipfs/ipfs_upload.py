from moralis import evm_api
import json
import base64
import os
from ipfs_img import upload_images

api_key = os.getenv('MORALIS_API_KEY')

names = ['Synthetica', 'Naturaia', 'Sombrus']
descriptions = ['Synthetica is an artificial type Monster, its power skill is to control and manipulate technology, it has a high intelligence and can hack into any electronic system',
                'Naturaia is a nature type Monster, its power skill is to control and manipulate the elements of nature, such as water, earth, and air, it can create natural barriers and summon natural disasters',
                'Sombrus is a shadow type Monster, its power skill is to manipulate shadows and darkness, it can create illusions and disappear in the shadows, it has a high speed and agility,'
                ]
ipfs_urls = upload_images()
Types = ['Artificial', 'Nature', 'Shadow']
Powers = ['Digital Storm', 'Forest Fury', 'Nightmare Strike']
Lives = ['130', '150', '200']


def upload_metadata():

    ipfs_metadatas = []

    for i in range(len(ipfs_urls)):
        content = {
            "name": names[i],
            "description": descriptions[i],
            "image": ipfs_urls[i],
            "attributes": [
                {"trait_type": "Type", "value": Types[i]},
                {"trait_type": "Power", "value": Powers[i]},
                {"trait_type": "Life", "value": Lives[i]},
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
