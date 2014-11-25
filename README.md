<p align="center">
  <img src="/Assets/Resources/Textures/Icon.png?raw=true" alt="MapCreator Icon"/>
</p>

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
	- right shift to toggle 'delete' mode on/off (mouse clicking will destroy objects);
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
		1) write the texture's index (1 byte, hence the limit of 256 textures);
		2) for each cube write it's X coordinate (1 byte), Y coordinate (1 byte), and Z coordinate(1 byte);
		3) once there are no more cubes for that texture, write 2 extra 'ghost cubes' at position (0, 0, 0) (this just means it will write 2 times: 0x00 for X, 0x00 for Y and 0x00 for Z);
		4) when generating the map, the step above makes it easy to see when a sequence ends as there can never be 2 cubes with the same position... (so, if the binary says there are, stop);

**NOTE:** This whole process always starts from the lower index texture found.

#Taking it a step further!

I previously said that this application would not be limiting, but we are just creating cubes with plain textures, right...? Wrong!
What we are actually doing is generating a .wmap file that contains groups of objects, not objects themselves... The cubes with plain textures are for you to get a preview of what your map will look like.
Let me give you an example.

----------

Lets say I create 4 different textures: 'a_blue_texture.png', 'b_red_texture.png', 'c_ball_texture.png' and 'd_command_texture.png'. 
Now I open up the editor and just randomly put these blocks wherever I can (just because I can...).
Promptly I save my map and exit the app. I now know the numbers correspondant to each texture in the file, right? (When loading them, the editor gets them in alphabethical order). This means that 'a_blue_texture.png' will be 0, 'b_red_texture.png' will be 1, 'c_ball_texture.png' will be 2 and 'd_command_texture.png' will be 3.

Now for the usefull part, when I load the map from the file I can do either one of two things: litterally just create the map as I made it in the editor (with cubes all the way) or I can twist things up a bit. I can read the .wmap file and say: "Computer, you will give me blue cubes for all of the positions with index 0. You shall change the color to red for index 1, but still give me cubes. When you generate at index 2, you can use spheres instead. Finnally, at index 3, you shall kill anyone who steps on it!"

In the editor you phisically see every cube you create, but they dont need to actually be physical when you play your game. When loading the map, you can program every single index to do whatever you want it to do, not just bring more cubes and shapes ;)

----------