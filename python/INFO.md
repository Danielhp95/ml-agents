### Setting up

In order to run experiemnts using Unity, The following dependencies are necessary.

#### Unity

Of course, the Unity game engine is required.
LINUX ONLY: [Download Unity from the last post in this thread](https://forum.unity.com/threads/unity-on-linux-release-notes-and-known-issues.350256/page-2)
Tested in Unity 2018.2.0b7.

#### Python

We recommend using a python virtual environment. More concretely, we recommend using [pipenv](https://pypi.org/project/pipenv/), a powerful virtual environment and package manager.

#### Creating a virtual Python environment using Pipenv

1. Inside of the desired directory run `pipenv install --python 3.6`
2. Activate the virtual environment by running `pipenv shell`
3. Install requirements from `requirements.txt` by running `pip install -r requirements.txt`


### Building an environment

#### Build prerequisites:
+ Set scripting runtime version to `.NET 4.x Equivalent`.
+ Set `ENABLE_TENSORFLOW` inside PlayerSettings -> Other Settings -> Scripting Define Symbols.
+ Make sure that the relevant `Brain`s are set to external.

#### Create a build inside of Unity Headleslly for Linux
1. Go to Build Settings.
1. Tick **Headless mode** box.
2. Set **Target platform** to Linux (x86_64 build).
This will create two files:

`
<environmentName>_Data/
<environmentName>.x86_64
`

We strongly recomend to move these files inside an `environments/` directory inside of the ml-agents `python/` directory. Such that we get:

`
python/environments/<environmentName>_Data/
python/environments/<environmentName>.x86_64
`

#### Running training from Python
This is done by executing the command
`python learn.py environments/<environmentName>.x86_64 --train`
