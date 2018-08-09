from unitytrainers import TrainerController


class SelfPlayTrainerController(TrainerController):

    def handle_episode_termination(self, curr_info):
        # handle opponent policy change
        super(SelfPlayTrainerController, self).handle_episode_termination(curr_info)