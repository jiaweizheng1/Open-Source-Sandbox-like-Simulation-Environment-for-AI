from typing import Optional, Union
import numpy as np
import gym
from gym import logger, spaces
import re
from pynput.keyboard import Key, Controller
import time
keyboard = Controller()

actions = ['q', 'w', 'e', 'r', 't', 'y', 'u']
env_actions = spaces.Discrete(7)
env_state = {
    "health": np.array([100.0]),
    "thirst": np.array([100.0]),
    "hunger": np.array([100.0]),

    "log": np.array([0]),
    "apple": np.array([0]),
    "meat": np.array([0]),
    "oil": np.array([0]),
    "water": np.array([0]),
    "iron": np.array([0]),
    "gold": np.array([0]),
    "diamond": np.array([0]),

    "alive": np.array([True], dtype=np.bool),
    "campfirebuilt": np.array([False], dtype=np.bool),
    "benchbuilt": np.array([False], dtype=np.bool),
    "rocketbuilt": np.array([False], dtype=np.bool),
    "axebuilt": np.array([False], dtype=np.bool),
    "scythebuilt": np.array([False], dtype=np.bool),
    "pickaxebuilt": np.array([False], dtype=np.bool),
}

done = False
reward = 0
f = open("observations.txt", "r")
def env_reset():
    keyboard.press('i')
    keyboard.release('i')
    env_state["health"] = np.array([100.0])
    env_state["thirst"] = np.array([100.0])
    env_state["hunger"] = np.array([100.0])
    env_state["log"] = np.array([0])
    env_state["apple"] = np.array([0])
    env_state["meat"] = np.array([0])
    env_state["oil"] = np.array([0])
    env_state["water"] = np.array([0])
    env_state["iron"] = np.array([0])
    env_state["gold"] = np.array([0])
    env_state["diamond"] = np.array([0])
    env_state["alive"] = False
    env_state["campfirebuilt"] = False
    env_state["benchbuilt"] = False
    env_state["rocketbuilt"] = False
    env_state["axebuilt"] = False
    env_state["scythebuilt"] = False
    env_state["pickaxebuilt"] = False
    done = False
    reward = 0
    return list(env_state.values())

def env_step(action):
    f.seek(0)
    keyboard.press(actions[action])
    keyboard.release(actions[action])
    while True:
        is_moving = (f.readline()).split[0]
        if is_moving == "True":
            f.seek(0)
            continue
        is_busy = (f.readline()).split[0]
        if is_busy == "True":
            f.seek(0)
            continue
        break
    contents = f.readlines()
    for line in contents:
        line.split()
        if line[0] == "Health:":
            env_state["Health"] = np.array([float(line[1])])
        elif line[0] == "Hunger:":
            env_state["Hunger"] = np.array([float(line[1])])
        elif line[0] == "Alive:":
            env_state["Alive"] = np.array([bool(line[1])])
        elif line[0] == "BenchBuilt:":
            env_state["benchbuilt"] = np.array([bool(line[1])])
        elif line[0] == "RocketBuilt:":
            env_state["rocketbuilt"] = np.array([bool(line[1])])
        elif line[0] == "AxeBuilt:":
            env_state["axebuilt"] = np.array([bool(line[1])])
        elif line[0] == "ScytheBuilt:":
            env_state["scythebuilt"] = np.array([bool(line[1])])
        elif line[0] == "PickaxeBuilt:":
            env_state["pickaxebuilt"] = np.array([bool(line[1])])
        elif line[0] == "CampfireBuilt:":
            env_state["campfirebuilt"] = np.array([bool(line[1])])
        elif line[0] == "Inventory":
            inventory_content = re.split("(,)",line[1])
            env_state["log"] = np.array([inventory_content[0]])
            env_state["apple"] = np.array([inventory_content[1]])
            env_state["meat"] = np.array([inventory_content[2]])
            env_state["oil"] = np.array([inventory_content[3]])
            env_state["water"] = np.array([inventory_content[4]])
            env_state["iron"] = np.array([inventory_content[5]])
            env_state["gold"] = np.array([inventory_content[6]])
            env_state["diamond"] = np.array([inventory_content[7]])
        elif line[0] == "reward":
            reward = float(line[1])
        elif line[0] == "Done":
            done = bool(line[1])
    
    return env_state, done, reward

def env_close():
    f.close()