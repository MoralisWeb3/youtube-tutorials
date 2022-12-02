from django.http import HttpResponse
from .services import get_nft_collections
import json

# Create your views here.


def get_collections(requests):
    chain = requests.GET.get('chain')
    address = requests.GET.get('address')
    limit = int(requests.GET.get('limit'))

    nft_collections = get_nft_collections(
        chain=chain, address=address, limit=limit)
    json_collectibles = json.dumps(nft_collections)
    return HttpResponse(json_collectibles)
