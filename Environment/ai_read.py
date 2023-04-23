import os
import sys
import time
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler

import time

f = open("observations.txt", "r")
contents_last = f.readlines()
print(contents_last)
while 1:
    time.sleep(0.5)
    f.seek(0)
    contents_new = f.readlines()
    if contents_last != contents_new:
        print(contents_new)
        contents_last = contents_new
