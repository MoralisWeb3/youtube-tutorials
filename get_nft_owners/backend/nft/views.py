from .services import get_nft_owners
from django.http import HttpResponse
import json


# Create your views here.


def get_owners(requests):
    chain = requests.GET.get("chain")
    address = requests.GET.get("address")
    limit = int(requests.GET.get("limit"))

    nft_owners = get_nft_owners(chain=chain, address=address, limit=limit)
    json_owners = json.dumps(nft_owners)
    return HttpResponse(json_owners)
