### Setting up

In order to run experiemnts using Unity, The following dependencies are necessary.

#### Unity

Of course, the Unity game engine is required.  
[Linux installation download link](https://forum.unity.com/threads/unity-on-linux-release-notes-and-known-issues.350256/page-2)  
This code has been tested on the following Unity verisions:  
+ Unity 2018.2.0b7.

#### Python

We recommend using a python virtual environment to manage Python dependencies. For this we recommend using [pipenv](https://pypi.org/project/pipenv/), a powerful virtual environment and package management tool. 

### Creating a virtual Python environment using Pipenv

1. Inside of `ml-agents/python/` directory run `pipenv install --python 3.6`. This will try to install the dependencies captured in `requirements.txt`. If it fails,  try steps 2, 3. Otherwise you are done.
2. Activate the virtual environment by running `pipenv shell`
3. Install requirements from `requirements.txt` by running `pip install -r requirements.txt`

#### Known Problems of this section.
- If you lack grpc dependences after installing using `requirements.txt`, please install the dependence using `pip install grpcio`.
- Some linux distributions have problems installing `pipenv`. **If you have a fresh [conda](https://anaconda.org/) environment, you can continue from the step three of this section.**

### Building an environment

#### Build prerequisites:
+ Set scripting runtime version to `.NET 4.x Equivalent` inside File-> Build Setting-> PlayerSettings -> Other Settings -> Scripting Runtime Version.
+ Set `ENABLE_TENSORFLOW` inside File-> Build Setting-> PlayerSettings -> Other Settings -> Scripting Define Symbols.
+ Make sure that the relevant `Brain`s are set to external in the inspector.

#### Create a build inside of Unity Headleslly for Linux
1. Go to File -> Build Settings.
1. Tick **Headless mode** box.
2. Set **Target platform** to Linux (x86_64 build).
This will create two files:

` <environmentName>_Data/` and `<environmentName>.x86_64`

We strongly recomend to move these files inside an `environments/` directory inside of the ml-agents `python/` directory. Such that we get:

`python/environments/<environmentName>_Data/` and `python/environments/<environmentName>.x86_64`

### Running training from Python
Inside of the `ml-agents/python/` directory, run the command:  
`python learn.py environments/<environmentName>.x86_64 --train`
