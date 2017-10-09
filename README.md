# DatPlatformGen
Fast-prototyped procedural 2D platformer level generator

The idea is that, when the player finishes a level, a new one is automatically generated
</br> -- NOW FUN NEVER ENDS   ( ͡° ͜ʖ ͡°)

- Contains 2 generators : LevelGenerator && FlatLevelGenerator.
- The GameManager is presented as example (and as well because I might lose it !)

### LevelGenerator

creates a level, based on smaller level area ("Rooms"), connected to each other via a feasible path, randomly disposed in a 6x5 2D array on X and Y positions

Still in development, the algorithm doesn't work at the moment, basically it is made of a big 6x5 2D array, containing 20x16 smaller 2D array (called "Rooms").
Those rooms are put next to each other and contain at least 2 walls (3 for the starting and end rooms).
They can be on top, under, left or right of the previous room, as long as it stays withing the 6x5 2D array boudaries.
Then, to be sure the player has always a way to beat the level, we pick random X,Y coordinates in a room and in the following one,
then iterate those coordinates until the first one reaches the other room, etc...
Giving us at the end a feasible path for the player.
Once the "solution path" is created, we add random platforms all over the map, so the player can't clearly see the "solution path"

#### Note :
It is for the moment really bugged since I haven't yet implement a way for the player to move on the Y axis (i.e.: ladder, ropes, elevators, etc).
I need as well to implement more testing to end up with a playable level (and not some "arty-ish" kind-of-pixelated monster made of platforms tiles)


### FlatLevelGenerator

This one works. <3
Basically FlatLevelGenerator is a simplified version of LevelGenerator since it only creates platforms on a Y axis equal to 0 and/or 3
The result is a simplified "Mario-like" type of level consisting of going from left to right, with few elevated platforms.
