import os
import random
import math
import tensorflow as tf
from unitytrainers import TrainerController, UnityTrainerControllerException


class SelfPlayTrainerController(TrainerController):

    def __init__(self, env_path, run_id, save_freq, curriculum_folder, fast_simulation, load, train,
                 worker_id, keep_checkpoints, lesson, seed, docker_target_name, trainer_config_path,
                 no_graphics):
        super(SelfPlayTrainerController, self).__init__(env_path, run_id, save_freq,
                                                        curriculum_folder, fast_simulation,
                                                        load, train, worker_id, keep_checkpoints,
                                                        lesson, seed, docker_target_name, trainer_config_path,
                                                        no_graphics)
        self.ghost_trainers_path = self.model_path + '/self-play-checkpoints/'
        if not os.path.exists(self.ghost_trainers_path):
            os.makedirs(self.ghost_trainers_path)

        # Self-play hyper-parameters
        self.delta = 0.0
        self.opponent_policy_change_interval = 1
        self.ghost_save_frequency = 2000

        # Self-play bookepping variables
        self.elapsed_episodes = 0

    def _initialize_trainers(self, trainer_config, sess):
        super(SelfPlayTrainerController, self)._initialize_trainers(trainer_config, sess)

        self.ghost_trainers = {brain_name: trainer for brain_name, trainer in self.trainers.items() if not trainer.is_main}

        try:
            self.main_brain_name, self.main_brain_trainer = next((brain_name, trainer) for brain_name, trainer in self.trainers.items() if trainer.is_main)
        except StopIteration:
            raise UnityTrainerControllerException("Could not find a main brain to use in the self-play system. Make sure that one brain has the flag \"is_main\".")

        self.main_trainer_saver = tf.train.Saver(var_list=tf.get_collection(tf.GraphKeys.GLOBAL_VARIABLES, scope=self.main_brain_trainer.graph_scope), max_to_keep=None)
        self.initialize_self_play_trainers(self.main_brain_trainer.graph_scope, self.ghost_trainers)

    def initialize_self_play_trainers(self, main_trainer_scope, ghost_trainers):
        """
        Initializes a tf.train.Saver for each (brain_name, trainer) pair. Each tf.train.Saver keeps
        track of the variables inside of the trainer.graph_scope.
        :param main_trainer_scope: Scope name for the main brain trainer
        :param ghost_trainer: dictionary of ghost brain names to their trainers
        """
        self.ghost_trainer_savers = {brain_name: self.create_trainer_saver(main_trainer_scope, trainer.graph_scope)
                                     for brain_name, trainer in ghost_trainers.items()}

    def create_trainer_saver(self, main_trainer_scope, scope):
        """
        Creates a tf.train.Saver that links the variables of the main brain to the variables
        of the brain linked to the :param scope:.
        :param scope: Ghost brain graph_scope
        :return: Saver for ghost brain
        """
        mapping_from_main_brain_to_ghost_brain = self.create_variable_mapping(from_scope=main_trainer_scope, to_scope=scope)
        return tf.train.Saver(var_list=mapping_from_main_brain_to_ghost_brain)

    def create_variable_mapping(self, from_scope, to_scope):
        """
        Creates dictionary of string variable brains from :param from_Brain: to
        :param from_scope: Name of the scope whose variables are going to be read from.
        :param to_scope: Name of the scope whose variables are going to be read to.
        :return: Dictionary mapping string variable names of scope :param from_scope: to the tf.Variable
        of scope :parm to_scop:
        """
        variables_from_scope = tf.get_collection(tf.GraphKeys.GLOBAL_VARIABLES, scope=from_scope)
        variables_to_scope   = tf.get_collection(tf.GraphKeys.GLOBAL_VARIABLES, scope=to_scope)
        mapping = dict(zip(map(lambda x: x.name[:-2], variables_from_scope), variables_to_scope))
        return mapping

    def save_models_and_checkpoints(self, sess, global_step, global_saver):
        super(SelfPlayTrainerController, self).save_models_and_checkpoints(sess, global_step, global_saver)
        if global_step % self.ghost_save_frequency == 0 and global_step != 0 and self.train_model:
            checkpoint_path = self.ghost_trainers_path + 'main_brain' + '-' + str(global_step) + '.cptk'
            self.main_trainer_saver.save(sess, checkpoint_path)
            print("Saving new opponents behaviour. Total checkpoints: {}".format(self.main_trainer_saver.last_checkpoints))
            self.main_trainer_saver.last_checkpoints

    def handle_episode_termination(self, curr_info, sess):
        # handle opponent policy change
        curr_info = super(SelfPlayTrainerController, self).handle_episode_termination(curr_info, sess)
        if self.env.global_done:
            print("Global done: {}".format(self.env.global_done))
            self.elapsed_episodes += 1
            print("Episode finished")
            if self.should_change_ghost_model():
                self.resample_all_ghosts(sess)
        return curr_info

    def resample_all_ghosts(self, sess):
        """
        Samples a historical policy for all ghost trainers
        :param sess: Tensorflow session.
        """
        print("Sampling policies for all ghost brains")
        print("Total checkpoints: {}".format(self.main_trainer_saver.last_checkpoints))
        for brain_name, saver in self.ghost_trainers.items():
            sampled_policy_checkpoint = self.sample_ghost_model()
            print("Resampling for {}. New policy: {}".format(brain_name, sampled_policy_checkpoint))
            self.ghost_trainer_savers[brain_name].restore(sess, sampled_policy_checkpoint)

    def should_change_ghost_model(self):
        """
        Returns if the ghost models should be resampled.
        """
        return self.elapsed_episodes % self.opponent_policy_change_interval == 0

    def sample_ghost_model(self):
        """
        Returns a model checkpoint resampled uniformly from the history of this model for the trainer in question.
        :return: String containing path to checkpoint to be used as new policy
        """
        all_checkpoints = self.main_trainer_saver.last_checkpoints
        print("Total checkpoints: {}".format(len(self.main_trainer_saver.last_checkpoints)))
        valid_checkpoints_slice = slice(math.ceil(self.delta * len(all_checkpoints)), len(all_checkpoints))
        return random.choice(all_checkpoints[valid_checkpoints_slice])
