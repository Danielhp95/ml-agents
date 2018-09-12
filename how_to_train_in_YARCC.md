# Training an AI in Unity ML-agents

# Training Locally

Explained in the official ML-agents repository: [here]("How to train" https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Training-ML-Agents.md)

## Training in YARCC

Training in a cluster is very similar to training in a Unix OS without a graphical display (i.e no pixel input available). We need to do three things:

1. Build the game environment headleslly for linux platforms.
2. Move over all necessary files to YARCC 
3. Queue up a job script in one of YARCC's queues.
4. Watch the world train.

### Create a build inside of Unity Headleslly for linux
1. Go to Build Settings.
1. Tick **Headless mode** box.
2. Set **Target platform** to Linux (x86_64 build).
This will create two files

`
<environmentName>_Data/
<environmentName>.x86_64
`

We strongly recomend to move these files inside an `environments/` directory inside of the ml-agents `python/` directory. Such that we get:

`
python/environments/<environmentName>_Data/
python/environments/<environmentName>.x86_64
`

### Connect to vpn For unix users
Instructions on how to connect to VPN from outside of York: [here]("VPN instructions" https://www.york.ac.uk/it-services/services/vpn/)

For Unix users that have Pulse Secure VPN client installed.
`pulsesvc -h webvpn.york.ac.uk -u <yorkusername> -r york_users_realm`

### Move Required files to YARCC

In order to train our agents in YARCC, we need to move all files involved in the training process to YARCC. These files are:

+ The directory + file generated during the build process `<environmentName>_Data/` and `<environmentName>.x86_64`
+ ML-agent's `python/` directory

We need to move these files into the `/scratch/<username/` directory in YARCC. We recommend to use `scp` for this. If all files are inside the `python` directory, one can just run:

`scp -r python/ <username>@login.yarcc.york.ac.uk:/scratch/username/`

And all files will be copied over to YARCC

### Load the required modules

YARCC uses a module system where in order to get acces to most installed programs / packages / libraries, it is first necessary to load them via the `module load <moduleName>` command.

The following modules need to be loaded, **order of load is important**.

```
module load cuda/9.1.85
module load python/3.6.4
```

### Queue up a job in one of YARCC's queues.

YARCC uses a queue system for submitting jobs into the cluster's resources. We need to submit a job containing the instructions to load all required modules for training as well as choosing to use the IGGI cluster for shorter queuing times.

#### The job file

The job file should be inside of the `scratch/<username>/python/` directory. And it should contain the following information:

```
#$ -cwd -V
#$ -l h_rt=01:0:00
#$ -l h_vmem=128G
#$ -o logs
#$ -e logs
#$ -l nvidia_k80=1
#$ -q iggi-cluster

python learn.py environments/<environmentName>.x86_64 --train
```

Explanation:
+ `#$ -cwd -V`: **mandatory flag** sets the current working directory to the directory of the job being submitted, as the default is the forbidden `home/` directory.
+ `-l h_rt`: **mandatory flag** estimated job length, used by YARCC's job scheduler.
+ `-l h_vmem=128G`: **mandatory flag**
+ `-o`: file where stdout will be redirected to
+ `-e`: file where stderr will be redirected to
+ `-l nvidia_k80`: flag that requests the nvidia_k80 gpu to be used.
+ `-q`: queue to which the job will be submitted to, the IGGI cluster is accessed through the `iggi-cluster` queue.

In order to submit the job to YARCC, we use `qsub`

`qsub <jobfile>`

If we want to see the state of the job we can check it by running the command:

`qstat`

### Training

The stdout of python script being executed will be logged inside the file specified by the `#$ -o <filename>`.

the model's output will be inside `models/` directory.
