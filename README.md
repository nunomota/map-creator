![MapCreator Icon](/Assets/Resources/Textures/Icon.png?raw=true)

**MapCreator** is a simple 3D map creator (obviously), developed with Unity3D. It is meant to make the "scenario creation" step of a game/app development much easier, yet not limiting. 

#First steps:

1) Once you open up the application, to start creating a map, it will require you to input a directory - in which you should have the textures that will be used for that project (these textures are expected to be .png separate images).
2) After that, it will take care of a few things for you: as you can only build blocks by attaching them to others blocks, a non-editeable floor will be created for you.
3) Wait a bit and you shall be prompted with a simple GUI, as show below:

![MapCreator GUI](/Screenshots/GUI.png?raw=true)

	- 
	- 

#How does it work?

Currently, it is meant to be used as a streamer by Android phones and tablets. 
You open up the application on your preferred device and select "Host session". After that, you will be able to connect any other device (be it PC, another Android device, etc...) by filling in the IP of the streaming device and clicking "Join session".

#Instructions

To get this working on Unity3D, you just have to follow a few simple steps:

	- Open a new Unity project and name it whatever you want;
	- Clone this repository and copy the contents inside "Assets" into your Project's Assets folder;
	- Go to "Assets/Scene" and open the scene in it;
	- Go to "Assets/Scripts" and drag: 'Main.cs' to the GameObject "Scripts"; 'draw.cs' to the "Main Camera";
	- Finnaly, go to "Assets/Resources" and drag 'skybox' to: Plane -> Mesh Renderer -> Materials -> Element 0;

After all this, you are good to go ;)