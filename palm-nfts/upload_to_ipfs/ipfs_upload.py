from moralis import evm_api
import json
import base64
import os
from ipfs_img import upload_images

api_key = os.getenv('MORALIS_API_KEY')

names = ['Skadi Son', 'Odin Son', 'Heimdall Son']
descriptions = [
    'Hunters are ranged specialists that use bows and traps to take down their enemies from afar. They have balanced hit points and damage output, making them versatile characters.',
    'Mages are spellcasters that use elemental magic to deal damage from a distance. They have the lowest hit points but make up for it with their powerful spells.',
    'Warriors are heavily armored melee fighters that rely on their strength and toughness to survive. They have the highest hit points and can take a lot of damage before falling.'
]
ipfs_urls = upload_images()
print(ipfs_urls)
Types = ['Hunter', 'Mage', 'Warrior']
Powers = ['Multi-shot', 'Fireball', 'Shield Bash']
Lives = ['150', '120', '200']


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

    ipfs_addresses = {'addresses': ipfs_metadatas}

    # Write the dictionary to a JSON
    with open('ipfs_addresses.json', 'w') as f:
        json.dump(ipfs_addresses, f)

    # Verify the data has been saved on the JSON
    with open('ipfs_addresses.json', 'r') as f:
        data = json.load(f)
        print(data)


upload_metadata()
