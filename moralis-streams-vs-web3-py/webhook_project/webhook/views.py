from django.http import HttpResponse
from django.views.decorators.csrf import csrf_exempt
import json

# Create your views here.
@csrf_exempt
def webhook(request):
    if request.method == "POST":
        json_object = json.loads(request.body)
        json_formatted_str = json.dumps(json_object, indent=2)
        print(json_formatted_str)
        return HttpResponse(status=200)
    else:
        return HttpResponse(status=403)
