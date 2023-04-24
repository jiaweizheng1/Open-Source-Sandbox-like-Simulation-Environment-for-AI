import random
import gym
import gym_environment
env = gym.make('gym_environment/env', render_mode = "human")
print(env.observation_space)
print(env.action_space.np_random)
for episode in range(1, 3+1):
    state = env.reset()
    done = False
    score = 0

    while not done:
        action = random.randint(0, 3)
        next_state, reward, done, truncated, info = env.step(action)
        score += reward
        env.render()
    
    print(f"Episode: {episode}, Score: {score}")
env.close()