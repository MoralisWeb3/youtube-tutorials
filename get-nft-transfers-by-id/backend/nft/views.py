from .services import get_transfers_by_id
from django.http import HttpResponse
import json


# Create your views here.


def get_transfers(requests):
    chain = requests.GET.get("chain")
    address = requests.GET.get("address")
    token_id = requests.GET.get("token_id")

    nft_transfers = get_transfers_by_id(chain=chain, address=address, token_id=token_id)
    json_transfers = json.dumps(nft_transfers)
    return HttpResponse(json_transfers)
