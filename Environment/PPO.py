import torch
import torch.nn as nn
from torch.distributions import Categorical

# set device to cpu or cuda; cuda gpu can have better performance
device = torch.device('cuda:0') if torch.cuda.is_available() else torch.device('cpu')

################################## PPO For Discrete Action Space Implementation ##################################
# initialize/management of data stack for PPO
class RolloutBuffer:
    # initialize arrays
    def __init__(self):
        self.actions = []
        self.states = []
        self.logprobs = []
        self.rewards = []
        self.state_values = []
        self.is_terminals = []

    # clear array buffers for reuse
    def clear(self):
        del self.actions[:]
        del self.states[:]
        del self.logprobs[:]
        del self.rewards[:]
        del self.state_values[:]
        del self.is_terminals[:]

# hybrid neural network architecture(A2C or Advantage Actor Critic) combining value-based and policy-based methods to help stabilize training
class ActorCritic(nn.Module):
    def __init__(self, state_dim, action_dim):
        super(ActorCritic, self).__init__()
        # neural network for actor(Policy-based); this is how the agent behaves/thinks
        # in: state of environment
        # out: action
        self.actor = nn.Sequential(
                        # input layer
                        nn.Linear(state_dim, 128),
                        nn.Sigmoid(),

                        # hidden layers
                        nn.Linear(128, 128),
                        nn.Sigmoid(),
                        
                        # output layer
                        nn.Linear(128, action_dim),
                        nn.Softmax(dim=-1)
                    )
        # neural network for critic(Value-based); this nn measures how good the action taken is 
        # in: state of environment
        # out: relative number, say 1, 2, 3, for how good action taken is
        self.critic = nn.Sequential(
                        # input layer
                        nn.Linear(state_dim, 128),
                        nn.Sigmoid(),

                        # hidden layers
                        nn.Linear(128, 128),
                        nn.Sigmoid(),

                        # output layer
                        nn.Linear(128, 1)
                    )
    
    def act(self, state):
        # feed observations or env input state variables into actor nn and get the action probabilities
        action_probs = self.actor(state)
        dist = Categorical(action_probs)

        # sample from action probabilities and decide on a action to take
        action = dist.sample()

        # in machine learning and AI, we take the log of probabilities to magnify the magnitudes of the probabilities when
        # they are very small. it helps with numerical stability and bigger numbers are easier to work with.
        action_logprob = dist.log_prob(action)

        # feed observations or env input state variables into critic nn and get the rating for how good the action taken will be
        state_val = self.critic(state)

        return action.detach(), action_logprob.detach(), state_val.detach()
    
    def evaluate(self, state, action):
        action_probs = self.actor(state)
        dist = Categorical(action_probs)
        action_logprobs = dist.log_prob(action)

        # compute entropy of action probabilities; entropy is the predictability of the actions of on agent; 
        # like how certain are we that the decision we make will yield the highest cumulative reward in the long run.
        dist_entropy = dist.entropy()
        state_values = self.critic(state)
        
        return action_logprobs, state_values, dist_entropy


class PPO:
    def __init__(self, state_dim, action_dim, lr_actor, lr_critic, gamma, K_epochs, eps_clip):
        # hyperparameter gamma or discount factor; between 0 and 1
        # discount current potential rewards to priotize future potential larger rewards
        self.gamma = gamma

        # hyperparameter epsilon or clip ratio; VERY IMPORTANT THIS IS THE MAIN IDEA OF PPO; for clipping the policy objective
        # so the new updated policy dont strave too far from the current old policy; policy is the actor nn + critic nn
        self.eps_clip = eps_clip

        # number of policy updates
        self.K_epochs = K_epochs
        
        self.buffer = RolloutBuffer()

        # bind the actor nn + critic nn to the policy for PPO
        self.policy = ActorCritic(state_dim, action_dim).to(device)
        # Adam optimizer for stochastic gradient ascent
        self.optimizer = torch.optim.Adam([
                        {'params': self.policy.actor.parameters(), 'lr': lr_actor},
                        {'params': self.policy.critic.parameters(), 'lr': lr_critic}
                    ])

        self.policy_old = ActorCritic(state_dim, action_dim).to(device)
        # state_dict() is the weights in the policy
        self.policy_old.load_state_dict(self.policy.state_dict())
        
        # Mean Square Error for stochastic gradient ascent
        self.MseLoss = nn.MSELoss()

    def select_action(self, state):
        # pass current env state into old policy to select an action
        with torch.no_grad():
                state = torch.FloatTensor(state).to(device)
                action, action_logprob, state_val = self.policy_old.act(state)
            
        self.buffer.states.append(state)
        self.buffer.actions.append(action)
        self.buffer.logprobs.append(action_logprob)
        self.buffer.state_values.append(state_val)

        return action.item()

    def update(self):
        # Monte Carlo estimate of returns/rewards
        rewards = []
        discounted_reward = 0
        for reward, is_terminal in zip(reversed(self.buffer.rewards), reversed(self.buffer.is_terminals)):
            if is_terminal:
                discounted_reward = 0
            # 
            discounted_reward = reward + (self.gamma * discounted_reward)
            rewards.insert(0, discounted_reward)
            
        # Normalizing the rewards
        rewards = torch.tensor(rewards, dtype=torch.float32).to(device)
        rewards = (rewards - rewards.mean()) / (rewards.std() + 1e-7)

        # convert list to tensor
        old_states = torch.squeeze(torch.stack(self.buffer.states, dim=0)).detach().to(device)
        old_actions = torch.squeeze(torch.stack(self.buffer.actions, dim=0)).detach().to(device)
        old_logprobs = torch.squeeze(torch.stack(self.buffer.logprobs, dim=0)).detach().to(device)
        old_state_values = torch.squeeze(torch.stack(self.buffer.state_values, dim=0)).detach().to(device)

        # calculate advantages
        advantages = rewards.detach() - old_state_values.detach()

        # Optimize policy for K epochs
        for _ in range(self.K_epochs):

            # Evaluating old actions and values
            logprobs, state_values, dist_entropy = self.policy.evaluate(old_states, old_actions)

            # match state_values tensor dimensions with rewards tensor
            state_values = torch.squeeze(state_values)
            
            # Finding the ratio (pi_theta / pi_theta_old)
            ratios = torch.exp(logprobs - old_logprobs.detach())

            # Finding Surrogate Loss  
            surr1 = ratios * advantages
            surr2 = torch.clamp(ratios, 1 - self.eps_clip, 1 + self.eps_clip) * advantages

            # final loss of clipped objective PPO
            loss = -torch.min(surr1, surr2) + 0.5 * self.MseLoss(state_values, rewards) - 0.01 * dist_entropy
            
            # take gradient step
            self.optimizer.zero_grad()
            loss.mean().backward()
            self.optimizer.step()
            
        # Copy new weights into old policy
        self.policy_old.load_state_dict(self.policy.state_dict())

        # clear buffer for next policy update
        self.buffer.clear()
    
    def save(self, checkpoint_path):
        # save actor + critic nn model
        torch.save(self.policy_old.state_dict(), checkpoint_path)

    def load(self, checkpoint_path):
        # load actor + critic nn model
        self.policy_old.load_state_dict(torch.load(checkpoint_path, map_location=lambda storage, loc: storage))
        self.policy.load_state_dict(torch.load(checkpoint_path, map_location=lambda storage, loc: storage))