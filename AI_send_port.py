from socket import *
import socket
import time
import pyautogui

inputs = ['q','q','w','w','e','r','r','r','y','t','u']

s = socket.socket(AF_INET, SOCK_STREAM)
s.bind((gethostbyname(gethostname()), 5005))
s.listen(5)
connectionSocket, addr = s.accept()
i = 0
while True:
    time.sleep(1)
    i+=1
    if i>10:
        i = 0
    connectionSocket.sendall(inputs[i])
    pyautogui.press(inputs[i])
