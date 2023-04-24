import gym
from gym import spaces
import pygame
import numpy as np


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
            "days_survived": spaces.Box(low=0, high=365, shape=(1,), dtype=int)
        })

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

        return self.observation_space.copy()

    def step(self, action):
        pass

    def close(self):
        pass
