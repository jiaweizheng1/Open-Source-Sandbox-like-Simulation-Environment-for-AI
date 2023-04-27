import gym
from gym import spaces
import pygame
import numpy as np
import random

class GymWorldEnv(gym.Env):
    metadata = {"render_modes": ["human", "rgb_array"], "render_fps": 4}

    def __init__(self, render_mode=None, size=5):
        self.action_space = spaces.Discrete(7)
        self.observation_space = spaces.Dict({
            "health": spaces.Box(low=0, high=100, shape=(1,), dtype=float),
            "thirst": spaces.Box(low=0, high=100, shape=(1,), dtype=float),
            "hunger": spaces.Box(low=0, high=100, shape=(1,), dtype=float),
            "inventory": spaces.Dict({
                "log": spaces.Box(low=0, high=100, shape=(1,), dtype=int),
                "apple": spaces.Box(low=0, high=100, shape=(1,), dtype=int),
                "meat": spaces.Box(low=0, high=100, shape=(1,), dtype=int),
                "oil": spaces.Box(low=0, high=100, shape=(1,), dtype=int),
                "water": spaces.Box(low=0, high=100, shape=(1,), dtype=int),
                "iron": spaces.Box(low=0, high=100, shape=(1,), dtype=int),
                "gold": spaces.Box(low=0, high=100, shape=(1,), dtype=int),
                "diamond": spaces.Box(low=0, high=100, shape=(1,), dtype=int)
            }),
            "campfirebuilt": spaces.Box(low=0, high=1, shape=(1,), dtype=bool),
            "benchbuilt": spaces.Box(low=0, high=1, shape=(1,), dtype=bool),
            "axebuilt": spaces.Box(low=0, high=1, shape=(1,), dtype=bool),
            "scythebuilt": spaces.Box(low=0, high=1, shape=(1,), dtype=bool),
            "pickaxebuiltbuilt": spaces.Box(low=0, high=1, shape=(1,), dtype=bool),
            "rocketbuilt": spaces.Box(low=0, high=1, shape=(1,), dtype=bool),
            "days_survived": spaces.Box(low=0, high=365, shape=(1,), dtype=int)
        })
        self.decrease_rate = 0.3
    def reset(self):
        # Initialize the environment state
        self.observation_space["health"] = np.array([100.0])
        self.observation_space["thirst"] = np.array([100.0])
        self.observation_space["hunger"] = np.array([100.0])
        self.observation_space["inventory"]["log"] = np.array([0])
        self.observation_space["inventory"]["apple"] = np.array([0])
        self.observation_space["inventory"]["meat"] = np.array([0])
        self.observation_space["inventory"]["oil"] = np.array([0])
        self.observation_space["inventory"]["water"] = np.array([0])
        self.observation_space["inventory"]["iron"] = np.array([0])
        self.observation_space["inventory"]["gold"] = np.array([0])
        self.observation_space["inventory"]["diamond"] = np.array([0])
        self.observation_space["days_survived"] = np.array([0])
        self.observation_space["campfirebuilt"] = False
        self.observation_space["benchbuilt"] = False
        self.observation_space["rocketbuilt"] = False
        self.observation_space["axebuilt"] = False
        self.observation_space["scythebuilt"] = False
        self.observation_space["pickaxebuilt"] = False

        return self.observation_space.copy()

    def step(self, action):
        reward = 0
        done = False
        if action == 0:
            self.observation_space["inventory"]["log"] += np.array([1])
            reward += 0.01
        elif action == 1:
            whichfood = random.randint(0, 2)
            oilchance = random.randint(0,10)
            if whichfood == 0:
                self.observation_space["inventory"]["apple"] += np.array([1])
            else:
                self.observation_space["inventory"]["meat"] += np.array([1])
            if oilchance < 2:
                self.observation_space["inventory"]["oil"] += np.array([1])
            reward += 0.01
        elif action == 2:
            self.observation_space["inventory"]["water"] += np.array([1])
            reward += 0.01
        elif action == 3:
            whichmineral = random.randint(0,2)
            diamondchance = random.randint(0,10)
            if whichmineral == 0:
                self.observation_space["inventory"]["iron"] += np.array([1])
            else:
                self.observation_space["inventory"]["gold"] += np.array([1])
            if diamondchance < 2:
                self.observation_space["inventory"]["diamond"] += np.array([1])
            reward += 0.01
        elif action == 4:
            if self.observation_space["campfirebuilt"] == False and self.observation_space["inventory"]["log"] >= 2 and self.observation_space["inventory"]["iron"] >= 1:
                reward += 0.9
                self.observation_space["campfirebuilt"] == True
            elif self.observation_space["campfirebuilt"] == True and self.observation_space["inventory"]["water"] >=1 and self.observation_space["inventory"]["meat"] >=1 and self.observation_space["inventory"]["apple"] >= 1 and self.observation_space["inventory"]["log"] >=1:
                reward += 0.1
                self.observation_space["inventory"]["meat"] -= np.array([1])
                self.observation_space["inventory"]["log"] -= np.array([1])
                self.observation_space["inventory"]["water"] -= np.array([1])
                self.observation_space["inventory"]["apple"] -= np.array([1])
        elif action == 5:
            if self.observation_space["benchbuilt"] == False and self.observation_space["inventory"]["log"] >=3 and self.observation_space["inventory"]["iron"] >=1 and self.observation_space["inventory"]["gold"] >=1 and self.observation_space["inventory"]["diamond"] >=1:
                self.observation_space["benchbuilt"] == True
                self.observation_space["inventory"]["diamond"] -= np.array([1])
                self.observation_space["inventory"]["gold"] -= np.array([1])
                self.observation_space["inventory"]["iron"] -= np.array([1])
                self.observation_space["inventory"]["log"] -= np.array([3])
                reward += 0.85
            elif self.observation_space["benchbuilt"] == True and self.observation_space["axebuilt"] == False and self.observation_space["inventory"]["log"] >= 2 and self.observation_space["inventory"]["iron"] >= 3:
                self.observation_space["inventory"]["iron"] -= np.array([3])
                self.observation_space["inventory"]["log"] -= np.array([2])
                reward += 0.85
                self.observation_space["axebuilt"] = True
            elif self.observation_space["benchbuilt"] == True and self.observation_space["axebuilt"] == True and self.observation_space["scythebuilt"] == False and self.observation_space["inventory"]["log"] >= 2 and self.observation_space["inventory"]["gold"] >= 3:
                self.observation_space["inventory"]["gold"] -= np.array([3])
                self.observation_space["inventory"]["log"] -= np.array([2])
                reward += 0.85
                self.observation_space["scythebuilt"] = True
            elif self.observation_space["benchbuilt"] == True and self.observation_space["axebuilt"] == True and self.observation_space["scythebuilt"] == True and self.observation_space["pickaxebuilt"] == False and self.observation_space["inventory"]["log"] >= 2 and self.observation_space["inventory"]["gold"] >= 3:
                self.observation_space["inventory"]["diamond"] -= np.array([3])
                self.observation_space["inventory"]["log"] -= np.array([2])
                reward += 0.85
                self.observation_space["pickaxebuilt"] = True

        elif action == 6:
                if self.observation_space["rocketbuilt"] == False and self.observation_space["inventory"]["log"] >=10 and self.observation_space["inventory"]["iron"] >= 10 and self.observation_space["inventory"]["gold"] >= 10 and self.observation_space["inventory"]["diamond"] >= 10:
                    self.observation_space["inventory"]["log"] -= np.array([10])
                    self.observation_space["inventory"]["iron"] -= np.array([10])
                    self.observation_space["inventory"]["gold"] -= np.array([10])
                    self.observation_space["inventory"]["diamond"] -= np.array([10])
                    reward +=1
                elif self.observation_space["rocketbuilt"] == True and self.observation_space["inventory"]["log"] >=1 and self.observation_space["inventory"]["meat"] >= 5 and self.observation_space["inventory"]["apple"] >= 5 and self.observation_space["inventory"]["oil"] >= 10 and self.observation_space["inventory"]["iron"] >= 1 and self.observation_space["inventory"]["gold"] >= 1 and self.observation_space["inventory"]["diamond"] >= 1:
                    self.observation_space["inventory"]["oil"] -= np.array([10])
                    self.observation_space["inventory"]["meat"] -= np.array([5])
                    self.observation_space["inventory"]["apple"] -= np.array([5])
                    self.observation_space["inventory"]["diamond"] -= np.array([1])
                    self.observation_space["inventory"]["gold"] -= np.array([1])
                    self.observation_space["inventory"]["iron"] -= np.array([1])
                    self.observation_space["inventory"]["log"] -= np.array([1])
                    reward +=1

        self.observation_space["hunger"] -= np.array([self.decrease_rate])
        self.observation_space["thirst"] -= np.array([self.decrease_rate])
        if self.observation_space["hunger"] <= 0.0 or self.observation_space["thirst"] <= 0.0:
            self.observation_space["health"] -= np.array([self.decrease_rate])
        if self.observation_space["health"] == 0:
            done = True

        return self.observation_space,reward,done

