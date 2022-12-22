from django.shortcuts import render
from django.http import HttpResponse
from .services import resolve_unstoppable_domain
import json

# Create your views here.
def resolve_domain(request):
    domain = request.GET.get("domain")
    currency = request.GET.get("currency")
    unstoppable_domain = resolve_unstoppable_domain(domain=domain, currency=currency)
    json_domain = json.dumps(unstoppable_domain)
    return HttpResponse(json_domain)
