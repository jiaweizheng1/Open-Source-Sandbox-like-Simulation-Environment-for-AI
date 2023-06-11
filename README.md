# Open Source Sandbox Like Simulation Environment for AI (User Manual)

# Table of Contents
- Introduction
    - [Preface](#Preface)
    - [Project Overview](#Project-Overview)
    - [Installation/Dependencies](#InstallationDependencies)
- Game
    - [Environment Settings Menu](#Environment-Settings-Menu)
    - [Reward Settings Menu](#Reward-Settings-Menu)
    - [Action Shape and Observation Shape](#Action-Shape-and-Observation-Shape)
- API
    - [observations.txt](#observationstxt)
    - [utils.py](#utilspy)
- AI
    - [Proximal Policy Optimization (Clip Variant)](#Proximal-Policy-Optimization-Clip-Variant)
    - [Trained Models](#Trained-Models)
- Additional Information
    - [Troubleshooting](#Troubleshooting)
    - [FAQ](#FAQ)
    - [Appendix](#Appendix)
    - [Developer Notes](#Developer-Notes)
    - [Contact Information](#Contact-Information)
    - [References](#References)

# Introduction

## Preface
Our target audience is anyone who is interested in Unity Game Development or reinforcement learning algorithms. The goal of this project is to provide users who already wrote their own reinforcement learning algorithms, a means to test how well their algorithms will actually perform. The testing environment we created is very similar to environments in the Python library Gym created by OpenAI except the visualization of the game is done through Unity Game Engine instead of Python. For users who do not know where to begin with reinforcement learning, we also provide a sample implementation of OpenAI's latest reinforcement learning algorithm called PPO (Proximal Policy Optimization) for discrete inputs written in Python via torch.

We also want our project to foster unique and healthy competition between different AI models through our sandbox environment in order to drive innovation within the AI community.

## Project Overview
![](https://i.imgur.com/msCW9Z8.gif)
### Game
When the player spawns, they start with a full status bar (health, hunger, and thirst) and zero materials in their inventory. They have to travel to areas and harvest to gather the corresponding resource of that area (forest to get wood, for example). Once they meet the resource requirments displayed at the top of the crafting bench, campfire, and rocket, they can go to those areas and construct them. The campfire is necessary to replenish health, hunger, and thirst through cooking. The tools, made in the crafting bench, helps increase harvesting efficency. Finally, the "winning" condition is they have to construct the rocket and launch it.

### API
From the Unity side, in the main game script, we use system IO to periodically output the current environmental state variables to a file called `observations.txt`. From the user side, we provide a python script called `utils.py` which sends actions to the games, read changes in environmental variables in `observations.txt` as a result of that action, and return the necessary information such as rewards and processed observations that users’ AIs need for training.

### AI
Initially, after we finish making the game, we took Unity Machine Learning Agents for granted. With careful tuning of action rewards and some hyperparameters, we manage to train an PPO model that could finish the game in about 3-5 days. In the second quarter, we started learning about PPO, we implemented our own in Python, and we once again trained another PPO model that could finish the game in 3-5 days (but this time, with our `observations.txt` and `utils.py` API).

## Installation/Dependencies
- **Unity Game Editor 2022.3.0f1** (Optional because we provide a standalone executable for the testing environment, however, this is necessary if you want to make dramatic changes to our testing environment.)
- **Python 3.9.13** (We strongly recommend this version because this is the version we used and mlagents has very strict version requirements for Python)
- Python libraries installed via `pip install`
    - **mlagents**
    - **torch**
    - **numpy**
    - **pynput**
    - **gym** (if you want to experiment with OpenAI's testing environments)

# Game
## Environment Settings Menu
Users have the power to change several features at will before and during the game's runtime. These configurable settings include:
- How much health and resources the robot begins with
- **Tool Toggle:** Allow crafting/usage of tools
- **God Mode Toggle:** Prevents any damage from being inflicted onto the robot's HP (this is for the case if users think their AI is too basic and will not perform adequately)
- **Random Toggle:** Input actions are randomized (this can help discourage exploitation and encourage exploration because user AI's will have to learn the game again every time the game restarts)
- **Enemy Toggle:** Allows the spawning of an enemy spider to attack the robot's campfire (note: spiders spawn at night)

## Reward Settings Menu
Users can change the reward values of different actions. For instance, if a player wanted their AI to prioritize survival over completing the game, they would input a larger reward value for eating at the campfire.

**It is highly recommended to use values between -1 and 1. Any values outside of this range will lead to unstable training.**

## Action Shape and Observation Shape
### Input Shape
The input shape is a integer value from **0 to 6**, which contains all the actions available in the game; these are the values your AI should select from if it wants to make an action in the game.

|Value| Action                 |
|-----|------------------------|
| 0   | Chop trees for Logs  |
| 1   | Gather Apples, Meat, and Oil|
| 2   | Collect Water |
| 3   | Mine Iron, Gold, and Diamonds|
| 4   | Build Campfire or Cook/Eat at Campfire|
| 5   | Build Axe, Scythe, and Pickaxe |
| 6   | Build Rocket or Launch the rocket |

### Observation Shape
The `observation_space` in `utils.py` is a dictionary structure in Python; this structure contains all the environmental state variables that should be fed into the neural network of your AI model.
| Observation Name | Type            | Example Value |
|------------------|-----------------|---------------|
| Inventory        | dict            | `{"log": np.array([0]), "apple": np.array([0]), ...}` |
| Health           | np.array        | `[100.0]`     |
| Hunger           | np.array        | `[100.0]`     |
| BenchBuilt       | np.array (bool) | `[False]`     |
| CampfireBuilt    | np.array (bool) | `[False]`     |
| RocketBuilt      | np.array (bool) | `[False]`     |
| AxeBuilt         | np.array (bool) | `[False]`     |
| ScytheBuilt      | np.array (bool) | `[False]`     |
| PickaxeBuilt     | np.array (bool) | `[False]`     |


# API
## observations.txt
From the Unity side, in the main game script, we use System IO to periodically output game variables to a file called `observations.txt`. Here is an overview of the information it contains:
| Variable Name  | Type            | Purpose |
|----------------|-----------------|-------------------|
| Moving         | bool            | Indicates if the agent is currently moving |
| Busy           | bool            | Indicates if the agent is currently busy |
| Inventory      | list of ints           | Contains the count of different items in the inventory. Each position in the tuple corresponds to a particular item |
| Health         | float             | The current health of the agent |
| Hunger         | float           | The current hunger level of the agent |
| BenchBuilt     | bool            | Indicates if a bench has been built |
| CampfireBuilt  | bool            | Indicates if a campfire has been built |
| RocketBuilt    | bool            | Indicates if a rocket has been built |
| AxeBuilt       | bool            | Indicates if an axe has been built |
| ScytheBuilt    | bool            | Indicates if a scythe has been built |
| PickaxeBuilt  | bool            | Indicates if a pickaxe has been built |
| Reward         | float           | The reward received in the last action |
| Done           | bool            | Indicates if the current episode is done |

The `Moving` and `Busy` variables are particularly important because they are used to synchronize your AI making an action, that action happening in the game, and the reward for that action plus the new environment state variables that we should return back to your AI.

Further, you should notice this is how we get the environmental state variables in `observation_space` that your AI needs to train.

## utils</span>.py

### env_step()
The `env_step()` function facilitates AI progression through the game. Your AI should make a call to this everytime it wants to send an action. It feeds the action inputted by your AI into the game, waits for the action to be completed, and retrieves the new set of state information from the game. Then, the processed `observation_space`, `reward`, and `done` from the new set of state information is returned to you.

### env_reset()
On the other hand, the `env_reset()` function resets the game and prepares it for a new training episode. Then, we return the new `observation_space` to you so you can feed it to your AI. Note: We do not need to return `reward` and `done` variables because it is a brand new episode (no actions has been made yet and the rocket has not been launched).

# AI
## Proximal Policy Optimization (Clip Variant)
Background: Reinforcement Learning is basically a continuous cycle where first, the AI observes and gather data from its environment. Then, it makes a decision based on the data it has. The decision it makes corresponds to an action in the game. Then, depending on the action, it can either get a penalty or a reward. Finally, because of the AI’s action, the state of the game changes so the AI has made a new observation, and the cycle continues. Note that as the AI goes through this cycle, it builds upon its own neural network, and caters more and more to action patterns that maximize the rewards it gets.

![Imgur Image](https://i.imgur.com/sthC6Uy.jpg)  
PPO is OpenAI's latest reinforcement learning algorithm.  The main idea behind this algorithm is to introduce a new hyperparameter called epislon, $\epsilon$. Once a feed forward pass in the neural network is completed and we compute a new, possibly better set of parameters to activation functions in the neural network, we do not immediately update that new set back into the neural network. Instead, we compute the ratios of change between that new set of parameters and old set of parameters, and clip that into a limited range based on the value of $\epsilon$. Then, we do backprogation with the new clipped set of parameters to update weights. This process basically ensures that the new policy does not get too far from the old policy and the model slowly converges to a optimal solution. 

## Trained Models

We trained two PPO models, one using Unity Machine Learning Agents and one using our `PPO.py`, to finish the game in 3-5 days.

With unity editor opened, select **Robot** gameobject, go to ***Behavior Parameters/Behavior Type*** on the inspector window, and change it from ***Heuristic Only*** to ***Inference Only***. Click play and you should see our PPO model trained using UMLA playing the game.

With `test.py` open in vscode, start the application and immediately change the main window to the game opened in the unity editor or in the executable. You should see your PPO model trained using `PPO.py` playing the game.

# Additional Information
## Troubleshooting
### Issue 1: The robot is stucked in one of the places
- <strong>Possible Causes</strong>: The environment speed is set to too high or too slow.
- <strong>Solution</strong>: click **Robot** gameobject and make changes to the **Timescale** value. 

### Issue 2: The robot sometimes do not board the rocket
- <strong>Possible Causes</strong>: You are resetting the game way too fast.
- <strong>Solution</strong>: Slow down with resetting the game so Unity has enough time to reset everything properly.

## FAQ
<details>
    <summary><strong>
        Question 1: [Why is my keyboard spamming buttons (q, w, e, r, t, y, u) during training?]
    </strong></summary>
    We use python library pynput in utils.py to send your AI's actions to the game using the keyboard.
</details>

<details>
    <summary><strong>
        Question 3: [How do I play the game myself?]
    </strong></summary>
    You can open the project executable and press the keys (q, w, e, r, t, y, u). The first four keys are used for basic resource gathering, while the later three keys provide access to crafting bench, campfire, and rocket. Have fun!
</details>

<details>
    <summary><strong>
        Question 2: [Why choose PPO out of all reinforcement learning algorithms?]
    </strong></summary>
    Early on in our development, we were using Unity Machine Learning Agents (UMLA) because we did not have the full foundation at the time to fully understand Reinforcement Learning. By the midpoint in the development process after we had successfully built a functional sandbox and tested it with UMLA, we discovered that UMLA was built around PPO which enabled us to explore concepts around how Reinforcement Learning and PPO functioned. It was a system that we knew would work if we used it again but this time, for training an external AI (other than UMLA) on the game. Further, PPO threw away away the second order functions from predecessor reinforcement learning algorithm so it was one of the much easier ones to understand and implement.
</details>

# Appendix
## Cost Analysis
All the platforms we plan to use are free. However, it could be possible that we eventually run into an obstacle like lacking some necessary tools in Unity that would require us to get the premium version. We believe for our purposes that we will not need to make any monetary purchases for additional tools and assets for the foreseeable future.
## Social/Legal Aspect
While our project is not geared towards making an impactful effect on society, it serves to be a demonstration of decision making processes based on the resources and assets one would have in a controlled environment, where the AI is a reflection of a person. This project has the potential to explore different aspects of machine learning and the advancement of artificial intelligence to develop decision making processes that would normally be attributed to the intuition of human
beings. 

Because our project is very much so open-ended and offers a contributing perspective to the overall development of artificial intelligence, we intend to make our project open source and its availability accessible to all that may be interested. Modifications to the source code are allowed without legal repercussions and further expansion on it is more than welcome. Access to Unity is irrelevant as this program will be capable of running as its own executable file after compilation.
    
## Developer Notes
During our development time, we had hoped to provide users an experience with competing AI elements as part of the gameplay, such as a enemy robot that would also compete for limited resources against the player robot. However, due to time constraints, we managed to only deploy with a hostile spider AI that would randomly spawn in at night and break the player's campfire (also not yet linked to `observations.txt`, `utils.py`, and rewards menu). The purpose of this hostile challenge was to promote more difficulty through variability.

Due to time constraints, for the API, sending AI's actions to the game is implemented through Python library `pynput`, which means the users will have to effectively give up their keyboard and computer during AI training in our testing environment. The better practice would have been to use socket programming with a port to facilitate communication between AI and the game. That way the users do not have to give up their pc during training.

The ultimate goal of our project is to assist AI researchers in testing their models beyond their own datasets. Our project provides an additional platform for simulation testing, allowing researchers to observe how their AI models behave in new environments. They can test whether their AI can find the most efficient strategies for survival over extended periods, or even to test the effectiveness of how their AI will develop strategies to collect basic resources more efficiently.

# Contact Information
Yihong He: yihonghe0@gmail.com

Matthew Tom: mattom12@yahoo.com

Chonghan Wang: cwang942@usc.edu

Jiawei Zheng: jiaweizheng416528@gmail.com

# References
> [Unity ML Agents Hyperparameters Tuning Guide](https://github.com/miyamotok0105/unity-ml-agents/blob/master/docs/Training-PPO.md)
> 
> [OpenAI's official PPO documentation](https://spinningup.openai.com/en/latest/algorithms/ppo.html)
> 
> [PPO Python implementation](https://github.com/nikhilbarhate99/PPO-PyTorch)
> [Diagram for PPO-clip](https://www.researchgate.net/figure/PPO-with-Actor-Critic-style_fig3_359450568)
> 
> [Character Physics and Smooth Rotation](https://www.youtube.com/watch?v=xHoRkZR61JQ)
> 
> [Resource Gathering Game Inspiration](https://www.youtube.com/watch?v=OE4FG9apSB0)
> 
> [Additional Free Game Assets](https://quaternius.com/)
