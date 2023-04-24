from gym.envs.registration import register

register(
    id="gym_environment/env",
    entry_point="gym_environment.envs:GymWorldEnv",
)
