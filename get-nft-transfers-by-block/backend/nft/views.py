from .services import get_transfers_by_block
from django.http import HttpResponse
import json


# Create your views here.


def get_transfers(requests):
    chain = requests.GET.get("chain")
    block_number = requests.GET.get("block_number")
    limit = int(requests.GET.get("limit"))

    nft_transfers = get_transfers_by_block(
        block_number=block_number, chain=chain, limit=limit
    )
    json_transfers = json.dumps(nft_transfers)
    return HttpResponse(json_transfers)
