# HayDay-Project

About
-----
HayDay is a 3D resource collection / management game based around the buying, selling  and farming of cattle. The player must develop their farm into a thriving business. The target audience could roughly range from 6 + or older, I'm not entirely certain what age group this game fits into.
The game is still in early-alpha, expect buggy gameplay at the moment.

Running the Game
----------------
To run this 3D game, enable 'Unknown Sources' in your Android settings screen. Copy the apk file from the 'Build' folder to your device and tap on the file to start the installation. You may need to select 'Application Manager' or 'APK Installer' depending on your device's manufacturer. The game requires your device's operating system to be Android 4.0 or higher.

Gameplay
--------
The original game concept was designed to be played on a touchscreen mobile devices, which means screen size will vary immensely. It’s unclear at this stage if other platforms like PCs & consoles will require support in the future. The gameplay primarily evolves around herding animals on your farm, increasing your cash reserve and generally improve farm over time. To achieve this the player must buy and sell cattle at the local mart. Supplies will also be required to improve the wellbeing of the animals on the farm. Other animals will be later added expand the game at a later stage, but generally for now the game will evolve around simulating cattle farming.
As of version (0.6), the gameplay does not contain much content, but development continues as of writing this document.

Development
-----------
I believe the game has potential to grow and develop into more than just another farm simulator game. From an education point of view the game could teach kids more about farm animals and how to properly take care them. Helpful tips and information could be displayed on screen on occasion to progressively teach the player about each animal as they play.
List of other ideas:
* Gameplay features like private sales between the player and NPC's in-game could be an aspect to implement later in development, normal these private sales between farmers take place occasionally. 
* Multiplayer support may be included later in the project, which means likely that player chat, buying and selling between players could be features implemented afterwards. 
Overall the game should be quite basic at first with more features being added to the gameplay over time. To cap off the project concept, it must have a cartoony feel and look to the game. I believe this will make it more welcoming to all ages, young or old.

Game Flow
---------
The world will be constructed in 3D open environment, which means the player must be able to freely navigate their way around the world. A virtual joystick will implemented to allow players with touchscreen devices control the character.
Transitions between levels takes place when the player’s character comes into contact with a special object / area in-game that triggers an event to display a menu. This menu allows the player to transition from either the farm scene to the mart scene or back to the main menu.

The original design draft of the game contained no game progression, since the implementation stage allows me to experiment with different ideas to improve the gameplay we've added features like animal feeding and interactive monitoring with the animals. Players can tap on the animals to view their individual stats on each animal, the health of the animal & happiness too. Eventually the addition of veterinarian & vaccinations could be an interesting feature to add later.

Mechanics
---------
The game mechanics is largely tied to the unity 3D engine. Interactions between objects and the game world & player are all handled by the in-built physics engine. Each item / asset contains unity components that attach to each game object. If for example the player needs high detailed mesh collider, unity has pre-made a component for this task that takes the rendered model of the player object and creates a high detailed mesh collider that interact with the 3D world.
Many game objects we employ use pre-made components from unity, it allows me to quickly prototype ideas we need to implement, test & deploy to the final game scene. 

Physics
-------
The game physics in this game world employs the built-in unity physics engine. Collisions between objects and the player are all handled by components attached to the game objects in the game scene. The same is true for the game animals and the game world. The terrain in the game scene must response to collisions between game objects & players to create the feeling that it can be walked upon and explored by the players and used by world game objects.
Unity's terrain engine is quite powerful tool to the developer. The terrain in every game scene contains a layer that allows game objects like trees, farm structures and player's character to detect the height and depth of the terrain. This layer allows the physics engine reduce the amount of CPU cycles required, but also allows the developer to create complex environments.
A number of physics problems still exist in the game as of version (0.6), which needs addressing fairly soon. The animals while in motion can glitch slightly if the player's character's physics collider bumps into the mesh renderer of an animal, it could send the animal flying. Which sounds ridiculous really and needs to be fixed ASAP.

Interface
---------
The visual interface or GUI has been partially built using NGUI which contains some powerful features that allow developers build scalable interfaces for different screen sizes and device whether it be a tablet or smartphone. This was a major problem to overcome during the designing phase. 
### Main Menu
The main menu is a vital part of any game, both in a visual and function sense. It must contain the vital paths the player needs to navigate through the game. From this menu the player must be able to access the settings / options menu, the new game menu and the load game option.
### Options / Settings Menu
The options menu contains the sound controls and difficulty rating for the game. The difficulty rating can be changed and applied to a current game save if the player is finding it difficult to get their farm to turn a profit.
### New Game / Player Name Menu
As the title states this menu simply gets the player name and allows you start a new game. This menu should also allow the player to get back to main menu by pressing the back button.

Future Goals
------------
Future goals to implement & experiment with, mainly gameplay features.

* We would like to add the ability to upgrade the farm and acquire more land to farm more cows.
* Additionally we would like to increase the intractability of the bidders and other characters in the mart.
