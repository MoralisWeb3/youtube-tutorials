from moralis import evm_api
from dotenv import load_dotenv
import os
import base64
import mimetypes
load_dotenv()

folder_path = './img/'
api_key = os.getenv('MORALIS_API_KEY')


def upload_images():
    ipfs_urls = []
    for filename in os.listdir(folder_path):
        if filename.endswith(".jpg"):
            full_path = os.path.join(folder_path, filename)

            with open(full_path, "rb") as image_file:
                file_bytes = image_file.read()
                mime = mimetypes.guess_type(image_file.name)[0]
                encoded_string = base64.b64encode(file_bytes).decode()
                final_string = f'data:{mime};base64,{encoded_string}'

                body = [
                    {
                        "path": f"image/{filename}",
                        "content": final_string,
                    }
                ]

                result = evm_api.ipfs.upload_folder(
                    api_key=api_key,
                    body=body,
                )

                ipfs_urls.append(result[0]['path'])

    return ipfs_urls
