from django.http import HttpResponse
from .services import get_nft_metadata
import json


# Create your views here.


def get_metadata(requests):
    chain = requests.GET.get("chain")
    address = requests.GET.get("address")
    token_id = requests.GET.get("token_id")

    nft_metadata = get_nft_metadata(address=address, token_id=token_id, chain=chain)
    json_metadata = json.dumps(nft_metadata)
    return HttpResponse(json_metadata)
