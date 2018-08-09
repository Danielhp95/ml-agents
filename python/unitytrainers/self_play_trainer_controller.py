from unitytrainers import TrainerController, UnityTrainerControllerException


class SelfPlayTrainerController(TrainerController):

    def handle_episode_termination(self, curr_info):
        # handle opponent policy change

        if(self.should_change_ghost_model(curr_info)):
            ghost_brains = filter(lambda brain_name, trainer: trainer.is_ghost(), self.trainers.items)
            map(lambda trainer : trainer.set_model(self.resample_ghost_model()), ghost_brains)

        super(SelfPlayTrainerController, self).handle_episode_termination(curr_info)

    def should_change_ghost_model(self, curr_info):
        """
        Returns if the ghost models should be resampled.
        """
        raise UnityTrainerControllerException("The should_change_ghost_model method was not implemented.")


    def resample_ghost_model(self):
        """
        Returns a model checkpoint resampled from the history of this model for the trainer in question.
        """
        raise UnityTrainerControllerException("The resample_ghost_model method was not implemented.")