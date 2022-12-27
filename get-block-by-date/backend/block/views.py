from django.shortcuts import render
from django.http import HttpResponse
import json
from .services import get_block_by_date

# Create your views here.
def get_block(request):
    date = request.GET.get("date")
    chain = request.GET.get("chain")
    block = get_block_by_date(date=date, chain=chain)
    json_block = json.dumps(block)
    return HttpResponse(json_block, content_type="application/json")
