HOW TO USE THE TILE ENGINE TO CREATE ROOMS
------------------------

The way the Tile Engine works is that it can dynamically load rooms from .csv files.
.CSV is a file extension called "Comma-Separated Values".  It's exactly what it sounds like.

To easily create a room, open Excel.  Create a spreadsheet and fill each cell with a number.
Each number corresponds to a tile on the tilesheet.  Currently 0 = wood and 1 = stone brick.
The room can be any size, although keeping it less than 20h x 30w would probably keep it onscreen.
Once you have created a room, save the spreadsheet as "Windows/DOS CSV" and give it a meaningful name

Move the .csv file to the project folder.  Currently it goes in the following path:
CollisionTest/CollosionTest/CollisionTest/bin/x86/Debug/Rooms

The game will automatically load all files in this directory, so no need to import it in the code.