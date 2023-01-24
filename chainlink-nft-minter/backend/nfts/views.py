from django.http import HttpResponse
import json
from .services import get_nft_owners

# Create your views here.


def get_owners(request):
    address = request.GET.get("address")
    owners = get_nft_owners(address)
    owners_json = json.dumps(owners)
    return HttpResponse(owners_json, content_type="application/json")
