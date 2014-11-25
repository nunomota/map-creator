![MapCreator Icon](/Assets/Resources/Textures/Icon.png?raw=true)

**MapCreator** is a simple 3D map creator (obviously), developed with Unity3D. It is meant to make the "scenario creation" step of a game/app development much easier, yet not limiting. 

#First steps:


**1)** Once you open up the application, to start creating a map, it will require you to input a directory - in which you should have the textures that will be used for that project (these textures are expected to be .png separate images).

**2)** After that, it will take care of a few things for you: as you can only build blocks by attaching them to others blocks, a non-editeable floor will be created for you.

**3)** Wait a bit and you shall be prompted with a simple GUI, as show below:

![MapCreator GUI](/Screenshots/GUI.png?raw=true)

	- There are two main areas always present throughout the whole level design, here marked with '1' and '2';
	- In the middle of the screen you can see a tiny crosshair pointing towards a semi-transparent cube, which is your 'preview-cube' - it will show you where you will be building your block;
	- Once you see this cube it means you can build a new block, in that position, by using any of the mouse buttons (left or right);
	- Area '1' holds the information related to the textures being held by each mouse key (if you use your left mouse key it means you use texture 1, right means you use texture 2);
	- Area '2' contains the information of the textures present in the directory you specified (up to 256 different textures), stored in groups of 5;

**Input**:

	- left mouse click to create cube with texture 1, from Area '1';
	- right mouse click to create cube with texture 2, from Area '1';
	- mouse scrollwheel scroll to change the selected texture in Area '2';
	- Number '1' to use that texture for left mouse click;
	- Number '2' to use that texture for right mouse click;
	- Number '3' to get to the previous group of textures in Area '2';
	- Number '4' to get to the next group of textures in Area '2';
	- 'Esc' to show the option's menu;

**4)** Finnally, to save your map, you just need to go open up the option's menu and select 'Save current map' an type in its name (the file will be created in the same directory as the textures are);

#**.wmap** file extension?

All exported maps will be saved with .wmap extension, but what is it...? 
Currently this extension doesn't go through any kind of compression, with the sole purpose of being really easy and fast to understand and re-create a map afterwards.
The way the binary is created is as follows:
	
	- The 1st byte represents the extension's signature, which I choose to be 1;
	- The 2nd byte represents the extension's version number, which is also set to 1;
	- After these 2 fixed bytes, there is just a loop in the code that goes through all the created cubes and groups them by texture (keep this in mind);
	- Another loop will go through each found texture doing the following:
		**1)** write the texture's index (1 byte long, as you are limited to 256 textures);
		**2)** 