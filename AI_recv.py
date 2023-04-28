from socket import *
import socket

s = socket.socket()
s.connect((gethostbyname(gethostname()), 5005))
while True:
    print(s.recv(16))
