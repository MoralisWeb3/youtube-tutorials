from .services import get_nft_contract_transfers
from django.http import HttpResponse
import json


# Create your views here.


def get_transfers(requests):
    chain = requests.GET.get("chain")
    address = requests.GET.get("address")
    limit = int(requests.GET.get("limit"))

    nft_transfers = get_nft_contract_transfers(
        address=address, chain=chain, limit=limit
    )
    json_transfers = json.dumps(nft_transfers)
    return HttpResponse(json_transfers)
