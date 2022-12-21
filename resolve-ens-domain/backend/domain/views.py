from django.shortcuts import render
from django.http import HttpResponse
from .services import resolve_ens_domain
import json

# Create your views here.
def resolve_domain(request):
    address = request.GET.get("address")

    domain = resolve_ens_domain(address)
    json_domain = json.dumps(domain)
    return HttpResponse(json_domain)
