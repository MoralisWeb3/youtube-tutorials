from django.http import HttpResponse
import json
from .services import get_nft_lowest_price

# Create your views here.


def get_lowest(requests):
    address = requests.GET.get("address")
    chain = requests.GET.get("chain")
    lowest = get_nft_lowest_price(address=address, chain=chain)
    lowest_json = json.dumps(lowest)
    return HttpResponse(lowest_json, content_type="application/json")
