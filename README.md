# SpaceProject

![image](https://user-images.githubusercontent.com/29784234/132104531-22b2cd26-48f0-43be-8c25-62dfcc75f552.png)

Pet-project to implement a simple XCOM-like turn-based strategy on Unity.

Build on WebGL: https://otisorigin.github.io/SpaceProject-WebGL/

Trello board: https://trello.com/b/jxXAc7aS/space-project

Source code: https://github.com/otisorigin/SpaceProject/tree/master/Assets/Game/Scripts

Key features:
 - 2D Sprite graphics, top-down
 - 2 players take turns
 - 3 unit sizes (by occupied cells) 1x1, 2x2, 3x3
 - 6 types of units
 
Was implemented: 
 - Field grid display
 - Dynamically generated background before the battle (nebulae and planets are dynamically placed against the background of space)
 - Dynamically generated obstacles on the map (asteroids) before the battle
 - Highlighting units when hovering over: blue (you can select this unit), green (selected unit), red (enemy unit)
 - Selecting a unit to move by cursor (mouse control)
 - Move the camera around the map using the W, A, S, D keys
 - Zoom in / out the camera on Q / E
 - While selecting a unit, the available turn area is indicated in yellow;
 - Each unit has its own range
 - The choice of the trajectory of movement is carried out by moving the cursor with the mouse
 - When choosing a path, the trajectory of movement is displayed on the map
 - The player has the opportunity not to spend the entire turn of a unit at once, but to go part of the way and then switch to another unit
 - If there is no need to move unit or go the entire possible length of the path, you can set it to the defense mode
 - It is possible to reset the set path during the movement of the unit (Reset Path button)
 - There is a button to select the next available unit (Next Unit)
 - Ending a turn and switching to the next player
 - Health component and HP bar for each unit
 - The HP bars of friendly units are highlighted in green, and those of enemy units are highlighted in red
 - 6 types of units (without tuned characteristics)

Need to be implemented:
 - Fire mode
 - Menu, UI
 - Choosing a combination of 6 units before the battle by each player
 - Graphic effects (jet engine effect, shooting effects, etc.)
 
Technical aspects:
 - Pathfinding based on A*
 - Graph grid for 3 size of units (1х1, 2х2, 3х3)
 - Used Zenject DI container
 - 4 GameStates: Unit Selection, Unit Movement, Unit Attack, End Of Turn
 - When calculating the path on the grid of graphs, static obstacles (asteroids) and dynamic obstacles (other units) are taken into account
 - I tried to transfer logic from Update methods (called at each Tick) to events as much as possible

Resources:
 - (Videolesson) Pathfinding 1x1 using Dijkstra algorithm: https://www.youtube.com/watch?v=kYeTW2Zr8NA
 - (Videolesson) Unity Healthbar: https://www.youtube.com/watch?v=CA2snUe7ARM
 - Assets: https://opengameart.org/content/space-game-art-pack-extended
 
