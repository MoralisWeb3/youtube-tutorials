from django.http import HttpResponse
import json
from .services import get_nft_trades

# Create your views here.
def get_trades(requests):
    address = requests.GET.get("address")
    chain = requests.GET.get("chain")
    limit = int(requests.GET.get("limit"))

    trades = get_nft_trades(address=address, chain=chain, limit=limit)
    json_trades = json.dumps(trades)
    return HttpResponse(json_trades, content_type="application/json")
