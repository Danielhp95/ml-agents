import os
import tensorflow as tf
from unitytrainers import TrainerController, UnityTrainerControllerException


class SelfPlayTrainerController(TrainerController):

    def __init__(self, env_path, run_id, save_freq, curriculum_folder, fast_simulation, load, train,
                 worker_id, keep_checkpoints, lesson, seed, docker_target_name, trainer_config_path,
                 no_graphics):
        super(SelfPlayTrainerController, self).__init__(self, env_path, run_id, save_freq,
                                                        curriculum_folder, fast_simulation,
                                                        load, train, worker_id, keep_checkpoints,
                                                        lesson, seed, docker_target_name, trainer_config_path,
                                                        no_graphics)
        self.ghost_trainers_path = self.model_path + 'ghost/'
        if not os.path.exists(self.ghost_trainers_path):
            os.makedirs(self.ghost_trainers_path)

        # TODO add opci, delta and opponent save frequency

    def _initialize_trainers(self, trainer_config, sess):
        super(SelfPlayTrainerController, self)._initialize_trainers(trainer_config, sess)
        self.ghost_trainers = filter(lambda brain_name, trainer: trainer.is_ghost(), self.trainers.items)
        self.initialize_ghost_trainer_savers(self.ghost_trainer)

    def initialize_ghost_trainer_savers(self, ghost_trainers):
        """
        Initializes a tf.train.Saver for each (brain_name, trainer) pair. Each tf.train.Saver keeps
        track of the variables inside of the trainer.graph_scope.
        """
        self.ghost_trainer_savers = {brain_name: self.create_trainer_saver(trainer.graph_scope)
                                     for brain_name, trainer in ghost_trainers}

    def create_trainer_saver(scope):
        tf.train.Saver(var_list=tf.get_collection(tf.GraphKeys.GLOBAL_VARIABLES, max_to_keep=None, scope=scope))

    def save_models_and_checkpoints(self, sess, global_step, global_saver):
        super(SelfPlayTrainerController, self).save_models_and_checkpoints(sess, global_step, global_saver)
        if global_step % self.save_freq == 0 and global_step != 0 and self.train_model:
            self.logger.info("Saving new opponents behaviour")
            for brain_name, saver in self.ghost_trainer_savers.items():
                last_checkpoint = self.ghost_trainers_path + brain_name + '-' + str(global_step) + '.cptk'
                saver.save(sess, last_checkpoint)

    def restore_model_from_checkpoint(self, brain_name, checkpoint_path, sess):
        self.ghost_trainer_savers[brain_name].restore(sess, checkpoint_path)

    def handle_episode_termination(self, curr_info, sess):
        # handle opponent policy change
        super(SelfPlayTrainerController, self).handle_episode_termination(curr_info, sess)
        raise UnityTrainerControllerException("The handle_episode_termination method was not implemented.")

    def should_change_ghost_model(self, curr_info):
        """
        Returns if the ghost models should be resampled.
        """
        raise UnityTrainerControllerException("The should_change_ghost_model method was not implemented.")

    def resample_ghost_model(self, brain_name):
        """
        Returns a model checkpoint resampled from the history of this model for the trainer in question.
        """
        raise UnityTrainerControllerException("The resample_ghost_model method was not implemented.")
