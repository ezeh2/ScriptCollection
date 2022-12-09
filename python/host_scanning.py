


# How to make http request
import requests as req
import re

# range(1, 255)
nginx_ip_addresses = [5,6,7,30]
axis_ip_addresses = [184, 186, 192,193,194,195,197,198]
host_ip_addresses = [35,37,43,65,68,70,71,72,78,80,87,120,129,131,132,133,135,138,143,149,155,160,162,163,164,167,173,177,178,179]

for x in range(1, 255):
    try:        
        r = req.get('http://10.41.42.' + str(x) + '/remoteAdmin', timeout=1)
        # print(r)
        if r.status_code == 200:
            print("ok " + str(x))
            print(r.headers["Server"])
            content  = r.content
            content_str = str(content)
            ms = re.findall('Edition:.*ContentTableValue">[^<^>]+', content_str)
            print(ms)
        else:
            print("status_code: " + str(r.status_code))
    except Exception as arg:
        print("exception " + str(x) + " " )




