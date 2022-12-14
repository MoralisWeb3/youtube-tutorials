from .services import get_multiple_nfts
from django.views.decorators.csrf import csrf_exempt
from django.http import HttpResponse
import json


# Create your views here.


@csrf_exempt
def get_nfts(request):

    if request.method == "POST":
        json_data = json.loads(request.body)
        chain = json_data["chain"]
        tokens = json_data["tokens"]

        multiple_nfts = get_multiple_nfts(chain=chain, tokens=tokens)
        json_nfts = json.dumps(multiple_nfts)
    return HttpResponse(json_nfts)
