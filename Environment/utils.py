from typing import Optional, Union
import numpy as np
import gym
from gym.spaces import *
from gym import logger, spaces
import re
from pynput.keyboard import Key, Controller
import time
keyboard = Controller()

actions = ['q', 'w', 'e', 'r', 't', 'y', 'u']
action_space = spaces.Discrete(7)
observation_space = {
    "health": np.array([100.0]),
    "thirst": np.array([100.0]),
    "hunger": np.array([100.0]),
    "inventory": {
        "log": np.array([0]),
        "apple": np.array([0]),
        "meat": np.array([0]),
        "oil": np.array([0]),
        "water": np.array([0]),
        "iron": np.array([0]),
        "gold": np.array([0]),
        "diamond": np.array([0])
    },
    "is_alive": np.array([True], dtype=bool),
    "AxeBuilt": np.array([False], dtype=bool),
    "BenchBuilt": np.array([False], dtype=bool),
    "CampfireBuilt": np.array([False], dtype=bool),
    "RocketBuilt": np.array([False], dtype=bool),
    "ScytheBuilt": np.array([False], dtype=bool),
    "PickaxehBuilt": np.array([False], dtype=bool)
}

done = False
reward = 0
f = open("observations.txt", "r")
def env_reset():
    keyboard.press('i')
    keyboard.release('i')
    observation_space = {
        "health": np.array([100.0]),
        "thirst": np.array([100.0]),
        "hunger": np.array([100.0]),
        "inventory": {
            "log": np.array([0]),
            "apple": np.array([0]),
            "meat": np.array([0]),
            "oil": np.array([0]),
            "water": np.array([0]),
            "iron": np.array([0]),
            "gold": np.array([0]),
            "diamond": np.array([0])
        },
        "is_alive": np.array([True], dtype=bool),
        "AxeBuilt": np.array([False], dtype=bool),
        "BenchBuilt": np.array([False], dtype=bool),
        "CampfireBuilt": np.array([False], dtype=bool),
        "RocketBuilt": np.array([False], dtype=bool),
        "ScytheBuilt": np.array([False], dtype=bool),
        "PickaxehBuilt": np.array([False], dtype=bool)
    }
    done = False
    reward = 0
    return get_obs(observation_space)

def env_step(action):
    f.seek(0)
    done = False
    reward = 0
    keyboard.press(actions[action])
    keyboard.release(actions[action])
    while True:
        is_moving = (f.readline()).split()
        if is_moving == "True":
            f.seek(0)
            continue
        is_busy = (f.readline()).split()
        if is_busy == "True":
            f.seek(0)
            continue
        break
    contents = f.readlines()
    for line in contents:
        line.split()
        if line[0] == "Health:":
            observation_space["Health"] = np.array([float(line[1])])
        elif line[0] == "Hunger:":
            observation_space["Hunger"] = np.array([float(line[1])])
        elif line[0] == "Alive:":
            observation_space["Alive"] = np.array([bool(line[1])])
        elif line[0] == "BenchBuilt:":
            observation_space["benchbuilt"] = np.array([bool(line[1])])
        elif line[0] == "RocketBuilt:":
            observation_space["rocketbuilt"] = np.array([bool(line[1])])
        elif line[0] == "AxeBuilt:":
            observation_space["axebuilt"] = np.array([bool(line[1])])
        elif line[0] == "ScytheBuilt:":
            observation_space["scythebuilt"] = np.array([bool(line[1])])
        elif line[0] == "PickaxeBuilt:":
            observation_space["pickaxebuilt"] = np.array([bool(line[1])])
        elif line[0] == "CampfireBuilt:":
            observation_space["campfirebuilt"] = np.array([bool(line[1])])
        elif line[0] == "Inventory":
            inventory_content = re.split("(,)",line[1])
            observation_space["log"] = np.array([inventory_content[0]])
            observation_space["apple"] = np.array([inventory_content[1]])
            observation_space["meat"] = np.array([inventory_content[2]])
            observation_space["oil"] = np.array([inventory_content[3]])
            observation_space["water"] = np.array([inventory_content[4]])
            observation_space["iron"] = np.array([inventory_content[5]])
            observation_space["gold"] = np.array([inventory_content[6]])
            observation_space["diamond"] = np.array([inventory_content[7]])
        elif line[0] == "reward":
            reward = float(line[1])
        elif line[0] == "Done":
            done = bool(line[1])
    
    return get_obs(observation_space), done, reward

def env_close():
    f.close()


def get_obs(observation):
    flattened_obs = []
    for key, value in observation.items():
        if isinstance(value, np.ndarray):
            flattened_obs.extend(value.tolist())
        elif isinstance(value, dict):
            # recursively flatten the nested dict
            flattened_obs.extend(get_obs(value))
    
    return flattened_obs
