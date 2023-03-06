from django.http import HttpResponse
from .services import get_latest_coins
import json

# Create your views here.
def get_coins(request):
    limit = request.GET.get("limit")
    coins = get_latest_coins(limit)
    json_coins = json.loads(coins)
    return HttpResponse(json.dumps(json_coins), content_type="application/json")
