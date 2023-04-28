import time
from pynput.keyboard import Key, Controller
keyboard = Controller()

inputs = ['q','q','w','w','e','r','r','r','y','t','u']

i = 0
while True:
    time.sleep(2.5)
    i+=1
    if i>10:
        i = 0
    keyboard.press(inputs[i])
    keyboard.release(inputs[i])
