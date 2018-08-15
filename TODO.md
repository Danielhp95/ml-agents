### Python side


- [ ] Refactor `TrainerController` so that `start_learning` function becomes more modular

- [ ] Create a new `SelfPlayTrainerController` which inherits from `TrainerController` and adds the self-play functionality

- [ ] Create a tensorflow `tf.train.Saver` for each self-play opponent brain so they save only their own scope. This will be used for save / restore operations

- [ ] Create a `set_model` function inside of `Trainer` class

- [ ] Create module that samples from a history of checkpoints

- [ ] Add code in `SelfPlayTrainerController` to trigger opponent policy resampling 

- [ ] Modify `BrainParameters` class to include new information regarding whether a brain is the "main" brain or "opponen" brain

- [ ] Include `Opponent Policy Change Interval` (OPCI) and `delta` inside `trainer_config.yaml`


### C# side
 
- [ ] Modify / Create `External` Brain type to include whether a brain is the "main" or "opponent" brain

- [ ] Create environment.

### Extensions
[ ] (Optional) Create scope system to be able to train multiple self-play systems simultaneously.
- [ ] Create scope string variable in `C#` side, pass it to `Python` `BrainParameters` class. In `SelfPlayTrainerController` replace models selectively
