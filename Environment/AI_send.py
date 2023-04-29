import time
from utils import *

inputs = [0,0,1,1,2,3,3,3,5,4,6]

i = 0
while True:
    time.sleep(2.5)
    i+=1
    if i<=10:
        env_step(inputs[i])
    else:
        i = 0
        env_reset()
qwwerrrytuiqwwerr