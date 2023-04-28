import random
import gym
import gym_examples
import gym_environment
env = gym.make('gym_environment/env', render_mode = "human")
print(env.observation_space)
print(env.action_space)
for episode in range(1, 3+1):
    state = env.reset()
    terminated = False
    score = 0

    while not terminated:
        action = random.randint(0, 3)
        observation, reward, terminated, truncated, info = env.step(action)
        print(f"Observation: {observation}, Reward: {reward}, Info: {info}")
        score += reward
        env.render()
    
    print(f"Episode: {episode}, Score: {score}")
env.close()