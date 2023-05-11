import time
import numpy as np
from gym import spaces
from pynput.keyboard import Controller

keyboard = Controller()

actions = ['q', 'w', 'e', 'r', 't', 'y', 'u']

action_space = spaces.Discrete(7)

observation_space = {
    # "Benchbuildmats": {
    #     "log": np.array([3]),
    #     "apple": np.array([0]),
    #     "meat": np.array([0]),
    #     "oil": np.array([0]),
    #     "water": np.array([0]),
    #     "iron": np.array([1]),
    #     "gold": np.array([1]),
    #     "diamond": np.array([1])
    # },
    # "Firebuildmats": {
    #     "log": np.array([2]),
    #     "apple": np.array([0]),
    #     "meat": np.array([0]),
    #     "oil": np.array([0]),
    #     "water": np.array([0]),
    #     "iron": np.array([1]),
    #     "gold": np.array([0]),
    #     "diamond": np.array([0])
    # },
    # "Cookmats": {
    #     "log": np.array([1]),
    #     "apple": np.array([1]),
    #     "meat": np.array([1]),
    #     "oil": np.array([0]),
    #     "water": np.array([1]),
    #     "iron": np.array([0]),
    #     "gold": np.array([0]),
    #     "diamond": np.array([0])
    # },
    # "Rocketbuildmats": {
    #     "log": np.array([10]),
    #     "apple": np.array([0]),
    #     "meat": np.array([0]),
    #     "oil": np.array([0]),
    #     "water": np.array([0]),
    #     "iron": np.array([10]),
    #     "gold": np.array([10]),
    #     "diamond": np.array([10])
    # },
    # "Rocketlaunchmats": {
    #     "log": np.array([1]),
    #     "apple": np.array([5]),
    #     "meat": np.array([5]),
    #     "oil": np.array([0]),
    #     "water": np.array([10]),
    #     "iron": np.array([1]),
    #     "gold": np.array([1]),
    #     "diamond": np.array([1])
    # },
    # "Axebuildmats": {
    #     "log": np.array([2]),
    #     "apple": np.array([0]),
    #     "meat": np.array([0]),
    #     "oil": np.array([0]),
    #     "water": np.array([0]),
    #     "iron": np.array([3]),
    #     "gold": np.array([0]),
    #     "diamond": np.array([0])
    # },
    # "Scythebuildmats": {
    #     "log": np.array([2]),
    #     "apple": np.array([0]),
    #     "meat": np.array([0]),
    #     "oil": np.array([0]),
    #     "water": np.array([0]),
    #     "iron": np.array([0]),
    #     "gold": np.array([3]),
    #     "diamond": np.array([0])
    # },
    # "Pickaxebuildmats": {
    #     "log": np.array([2]),
    #     "apple": np.array([0]),
    #     "meat": np.array([0]),
    #     "oil": np.array([0]),
    #     "water": np.array([0]),
    #     "iron": np.array([0]),
    #     "gold": np.array([0]),
    #     "diamond": np.array([3])
    # },
    "Inventory": {
        "log": np.array([0]),
        "apple": np.array([0]),
        "meat": np.array([0]),
        "oil": np.array([0]),
        "water": np.array([0]),
        "iron": np.array([0]),
        "gold": np.array([0]),
        "diamond": np.array([0])
    },
    "Health": np.array([100.0]),
    "Hunger": np.array([100.0]),
    "BenchBuilt": np.array([False], dtype=bool),
    "CampfireBuilt": np.array([False], dtype=bool),
    "RocketBuilt": np.array([False], dtype=bool),
    "AxeBuilt": np.array([False], dtype=bool),
    "ScytheBuilt": np.array([False], dtype=bool),
    "PickaxehBuilt": np.array([False], dtype=bool),
}

reward = 0

done = False

def str_to_bool(s):
    if s == 'True':
        return True
    elif s == 'False':
        return False

def is_idle():
    f = open("observations.txt", "r")
    contents = f.readlines()
    f.close()
    if len(contents) != 0 and contents[0].split()[1] == 'False' and contents[1].split()[1] == 'False':
        return True
    else:
        return False
    
def read_info():
    f = open("observations.txt", "r")
    contents = f.readlines()
    f.close()

    if len(contents) == 0:
        f = open("observations.txt", "r")
        contents = f.readlines()
        f.close()

    inventory = contents[3].strip().split(": ")[1].strip("()").split(",")
    observation_space["Inventory"]["log"] = np.array([int(inventory[0])])
    observation_space["Inventory"]["apple"] = np.array([int(inventory[1])])
    observation_space["Inventory"]["meat"] = np.array([int(inventory[2])])
    observation_space["Inventory"]["oil"] = np.array([int(inventory[3])])
    observation_space["Inventory"]["water"] = np.array([int(inventory[4])])
    observation_space["Inventory"]["iron"] = np.array([int(inventory[5])])
    observation_space["Inventory"]["gold"] = np.array([int(inventory[6])])
    observation_space["Inventory"]["diamond"] = np.array([int(inventory[7])])
    observation_space["Health"] = np.array([float(contents[4].strip().split(": ")[1])])
    observation_space["Hunger"] = np.array([float(contents[5].strip().split(": ")[1])])
    observation_space["BenchBuilt"] = np.array([str_to_bool(contents[6].strip().split(": ")[1])])
    observation_space["CampfireBuilt"] = np.array([str_to_bool(contents[7].strip().split(": ")[1])])
    observation_space["RocketBuilt"] = np.array([str_to_bool(contents[8].strip().split(": ")[1])])
    observation_space["AxeBuilt"] = np.array([str_to_bool(contents[9].strip().split(": ")[1])])
    observation_space["ScytheBuilt"] = np.array([str_to_bool(contents[10].strip().split(": ")[1])])
    observation_space["PickaxehBuilt"] = np.array([str_to_bool(contents[11].strip().split(": ")[1])])

    global reward 
    reward = float(contents[13].strip().split(": ")[1])

    global done 
    done = str_to_bool(contents[15].strip().split(": ")[1])


def env_reset():
    # wait for idle
    while True:
        time.sleep(0.1)
        if is_idle():
            break

    # tell game to reset itself
    keyboard.press('i')
    keyboard.release('i')

    # wait for game reset
    while True:
        time.sleep(0.1)
        if is_idle():
            break

    read_info()

    return get_obs(observation_space)

def env_step(action):
    # wait for idle
    while True:
        time.sleep(0.1)
        if is_idle():
            break

    # send AI's input to game
    keyboard.press(actions[action])
    keyboard.release(actions[action])

    # wait for action to be completed
    while True:
        time.sleep(0.1)
        if is_idle():
            break
    
    read_info()

    return get_obs(observation_space), reward, done

def get_obs(observation):
    flattened_obs = []
    for key, value in observation.items():
        if isinstance(value, np.ndarray):
            flattened_obs.extend(value.tolist())
        elif isinstance(value, dict):
            # recursively flatten the nested dict
            flattened_obs.extend(get_obs(value))
    
    return flattened_obs