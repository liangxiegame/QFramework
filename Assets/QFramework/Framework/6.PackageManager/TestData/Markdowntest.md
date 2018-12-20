中文测试
# Create a Level

[Link to Markdeep](markdeepworking.md.html)

A level will require at least 4 things :
 - a Camera
 - a Player
 - a Tilemap
 - a LevelSettings object

A little amount of setup required

## Camera

You can grab the prefab camera called "DefaultCam" that is in the folder Kit

This camera define a basic Deadzone windows (see objects doc in the Docs folder for a doc of how it work)

## Player

Just grab the Player prefab in the Kit/Character folder and put it in the scene (preferably inside the camera zone)
See the Objects doc in the Docs folder for more info on the different settings.

## Tilemap

Right click in the Hierarchy and choose "2D Object/Tile Map" to create an empty tilemap.
It will create BOTH a grid and child of it a tile map.

On THE TILE MAP, add :
 - A tile map collider
 	- Check "use by composite" on that collider
 - Add a Composide Collider 2D. This will also add a Rigidbody
 	- Set the rigidbody "Body Type" to static

### Painting

To paint :
 - Open the Tile map Palette (window/tile map palette)
 - if empty (telling you to create a new tilemap), find the default tilemap in : Kit/Tiles/LevelPalette and drop the LevelPalette in the window
 - you now should have a collection of all the default tile.
Note that the level one (brown & grey 3x3 square, one with slanted corner) are automatic tiles : you can click on any of the 3x3 tile and paint, it will selected the right one accordingly.

## LevelSettings


LevelSettings define some global data for that level (e.g. gravity). There is a default prefab one in the Kit folder.

## Setup

Note to setup :

 - Drag the player into the target slot of the Camera DeadZone script component
 - Drag the camera into the cam slot of the PlayerInput script component

![](https://liangxiegame.github.io/QFramework/DocRes/QFramework-icon-0.1.0-512x128.png)