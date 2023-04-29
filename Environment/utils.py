from typing import Optional, Union
import numpy as np
import gym
from gym import logger, spaces

from pynput.keyboard import Key, Controller

keyboard = Controller()

letter_inputs = ['q', 'w', 'e', 'r', 't', 'y', 'u']

def env_reset():
    keyboard.press('i')
    keyboard.release('i')

def env_step(action):
    keyboard.press(letter_inputs[action])
    keyboard.release(letter_inputs[action])
