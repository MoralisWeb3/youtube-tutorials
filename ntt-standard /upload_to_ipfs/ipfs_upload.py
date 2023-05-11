from moralis import evm_api
import json
import base64
import os
from ipfs_img import upload_images

api_key = os.getenv("MORALIS_API_KEY")

names = ["Cert 1", "Cert 2", "Cert 3"]
descriptions = [
    "Certificate number one for the project",
    "Certificate number two for the project",
    "Certificate number three for the project",
]
ipfs_urls = upload_images()
print(ipfs_urls)
Types = ["Computer Science", "Biology", "Maths"]
Powers = ["Machine learning", "BioMechanics", "Algorithms"]
Lives = ["Stanford", "MIT", "Harvard"]


def upload_metadata():
    ipfs_metadatas = []

    for i in range(len(ipfs_urls)):
        content = {
            "name": names[i],
            "description": descriptions[i],
            "image": ipfs_urls[i],
            "attributes": [
                {"trait_type": "Assignment", "value": Types[i]},
                {"trait_type": "Specialization", "value": Powers[i]},
                {"trait_type": "Insitution", "value": Lives[i]},
            ],
        }

        body = [
            {
                "path": "metadata.json",
                "content": base64.b64encode(bytes(json.dumps(content), "ascii")).decode(
                    "ascii"
                ),
            },
        ]

        result = evm_api.ipfs.upload_folder(
            api_key=api_key,
            body=body,
        )

        ipfs_metadatas.append(result[0]["path"])

        print(result)

    ipfs_addresses = {"addresses": ipfs_metadatas}

    # Write the dictionary to a JSON
    with open("ipfs_addresses.json", "w") as f:
        json.dump(ipfs_addresses, f)

    # Verify the data has been saved on the JSON
    with open("ipfs_addresses.json", "r") as f:
        data = json.load(f)
        print(data)


upload_metadata()
