![MapCreator Icon](/Assets/Resources/Textures/Icon.png?raw=true)

**MapCreator** is a simple 3D map creator (obviously), developed with Unity3D. It is meant to make the "scenario creation" step of a game/app development much easier, yet not limiting. 

#First steps:

1) Once you open up the application, to start creating a map, it will require you to input a directory - in which you should have the textures that will be used for that project (these textures are expected to be .png separate images).

2) After that, it will take care of a few things for you: as you can only build blocks by attaching them to others blocks, a non-editeable floor will be created for you.

3) Wait a bit and you shall be prompted with a simple GUI, as show below:

![MapCreator GUI](/Screenshots/GUI.png?raw=true)

	- There are two main areas always present throughout the whole level design, here marked with '1' and '2';
	- In the middle of the screen you can see a tiny crosshair pointing towards a semi-transparent cube, which is your 'preview-cube' - it will show you where you will be building your block;
	- Once you see this cube it means you can build a new block, in that position, by using any of the mouse buttons (left or right);
	- Area '1' holds the information related to the textures being held by each mouse key (if you use your left mouse key it means you use texture 1, right means you use texture 2);
	- Area '2' contains the information of the textures present in the directory you specified (up to 256 different textures);

	**Input**:
	- left mouse click to create cube with texture 1, from Area '1';
	- right mouse click to create cube with texture 2, from Area '1';
	- mouse scrollwheel scroll to change the selected texture in Area '2';
	- Number '1' to use that texture for left mouse click;
	- Number '2' to use that texture for right mouse click;
	- Number '3' to get to the previous group of textures in Area '2';
	- Number '4' to get to the next group of textures in Area '2';
	- 'Esc' to show the option's menu;

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