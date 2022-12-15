from base64_img import BASE64IMG
from moralis import evm_api

api_key = "your api key"
body = [
    {
        "path": "moralis/logo.jpg",
        "content": BASE64IMG,
    }
]

result = evm_api.ipfs.upload_folder(
    api_key=api_key,
    body=body,
)

print(result)
