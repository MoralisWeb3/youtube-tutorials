from .services import get_user_nfts
from django.http import HttpResponse
import json


# Create your views here.
def get_nfts(request):
    address = request.GET.get("address")
    chain = request.GET.get("chain")
    if request.GET.get("cursor") != None:
        cursor = request.GET.get("cursor")
    else:
        cursor = ""

    nfts = get_user_nfts(address=address, chain=chain, cursor=cursor)
    json_nfts = json.dumps(nfts)
    return HttpResponse(json_nfts, content_type="application/json")
