import time
from utils import *

inputs = [0,0,1,1,2,3,3,3]

i = 0
while True:
    time.sleep(2.5)
    if i<=7:
        env_step(inputs[i])
    else:
        i = 0
        env_reset()
    i+=1
