# Contribution suggestion: self-play

We are a group of researchers in the University of York, UK. We are using Unity ML-agents as a testbed for our RL experiments. We are currently implementing a self-play system for Unity ML-agents based on the following paper: [Emergent Complexity via Multi-Agent Competition](https://arxiv.org/abs/1710.03748). It is our intention to offer it as a contribution to the existing Unity ML-agents codebase.  

## Motivation

On the current Unity 0.4 version, there are two ways of implementing self-play:

### One 

Following the example of the Tennis sample environment: 
- Create two or more identical agents.
- Link each agent to the same brain.
- Start training.

**Issues**: We have tested the following disadvantage through local testing: 
The agent only gets to play against the current version of itself, overfitting to its own behaviour. And does not generalize to other strategies.

### Two

- Create two or more identical agents
- Link each agent to its own brain *(both brains trained using the same algorithm and only differing in their initial weights)*    
      
**Issues**:
Early in training one agent eventually becomes dominant and overpowers the other agent for the rest of the training. This means that the stronger agent relies too much on exploiting the weaknesses of the weaker agent.

## Our self-play contribution

### Theoretical approach

We would like to implement a self-play system inspired by that introduced in [Emergent Complexity via Multi-Agent Competition](https://arxiv.org/abs/1710.03748). The overall idea is to keep track of the latest iteration of a policy alongside with checkpoints from training history. Every fixed interval the latest policy is matched against itself or a previous checkpoint. This previous checkpoint is sampled uniformly at random from eligible versions. This approach works not only for 1v1 scenarios, but also for scenarios with many agents in play. One of the agents would be the one training and learning over time. The rest would use policies sampled from previous historical checkpoints.

This is done by introducing two hyper parameters:

- **Delta(𝛿)**: which takes values between [0,1]. It indicates how much of the policy history will be considered when sampling a new opponent. 𝛿 = 1, only the latest policy will be used, 𝛿= 0, all of the history will be considered.      

- **Opponent policy change interval**: positive number. how many episodes will be played out before a new opponent will be sampled. 

A graphical representation of the training process can be found below: 

![self-play-graph](https://github.com/Danielhp95/IGGI-2018-Workshop-Unity-Self-Play-RL/blob/master/images/self-play-graph.png)  

**Benefits:**

- Agent trains against a varied set of opponents. Avoiding overfitting to its own strategy and becoming more resilient to different strategies and levels of play. We hypothesize that this will prevent overfitting to a single playstyle, making an AI that can more easily adapt to different players.

- It becomes easy to monitor if the latest version of the agent can defeat previous (and random) versions of itself. If the training only consists of matches between the latest version of the algorithm and itself, we are not monitoring performance against other possible opponents. Meaning that an increase in overall model performance may be due to overfitting against the agent’s latest strategy.

**Things to Consider:**

- This self-play mechanism requires a history of policies that are created during training. This has the potential of requiring large amounts of storage. A checkpoint of a tensorflow graph using the default Unity-ML agents settings for two agents takes up roughly 2MB of storage. This means that a history of 1000 policies will need 2GB, which may be inconvenient for some users.

- With the above storage consideration in mind, if we store a set number of historical policies, say `n`, then there will be an issue that our value of `𝛿` will only matter up until we have stored `n` policies, after which the sample will only come from some proportion the last `n` policies. This will lead to a shifting window of historical policies, rather than a window which scales as time goes on. To avoid this we will have to start pruning policies more intelligently and having a more sparse policy history as time goes on. However we don't currently know how to achieve this.

### Code contribution
We have a few proposed solutions for the architecture for self-play, two of which involve creating a new brain type. The reason the brain needs to be modified is because we need a way of distinguishing between brains which sample from the history of the policy and represent opponents to the learning brain. The advantage of introducing new brain types as we see it is that it might make things nicer in the unity UI when just adding a brain and having it be of the self-play type.

- **Introduce new brain type `CoreBrainInternalSelfPlay`**: This would introduce a new brain type which is loosely based on the existing CoreBrainInternal, and samples from many pre-saved models. In theory this should keep more of the work in `C#`, since the randomised sampling can all be done in `C#` and we shouldn need minimal changes to the python code. This approach would feature an external agent that learns in the python side, and one or more `CoreBrainInternalSelfPlay` agents that use TensorFlowSharp to dynamically load checkpoint models created by the external agent as training progresses. Both hyperparameters would be kept as part of the new brain type's class fields.

- **Introduce new brain type `CoreBrainExternalSelfPlay`**: This would introduce a new brain type which is instead based on the existing CoreBrainExternal. This would therefore require some more significant changes to the python code to allow for the external brain to load a randomly sampled historical policy. We think the jumping off point for these changes would be in the `start_learning` method in the `trainer_controller.py` class. This method already features a check for episode termination which propagates to the various trainers which we could use to trigger policy resampling. Both hyperparameters would be stored in the python-side hyperparameter file `trainer_config.yaml`.

- **Modify Current `CoreBrainExternal` or `CoreBrainInternal`**: This wouldn't introduce a new brain-type, so may be nicer in terms of retaining the simplicity of having 4, clearly defined brain types, but would necessitate some additional parameters to the existing brains, which could be fine with sensible defaults but might make the CoreBrains slightly more confusing to use. It would require similar code to be added to either of the CoreBrain classes as the previous two solutions add to subclasses.

Our preference would be to create one of the new, specialised Brain type, either internal or external. However, we would greatly appreciate some direction on what you guys prefer, and if you think that one of these solutions is more appropriate. There may also be a better solution that we have not considered.


### Request for advice / feedback:

- How and where to store whether or not the environment should use self-play. E.g. should the brain know, and track its agents separately (in both or either c# and python), should the academy know, and have separate brains for the ‘main policy’ and past policies. Should it be handled entirely in python?
- How and where to store variables about the self-play config (e.g. 𝛿) should it be stored `trainer_config.yaml`?
