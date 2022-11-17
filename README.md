# DataSnake
This is a simple snake game made with the Monogame NuGet package that can be controlled with a keyboard or controller.

# Controls:
W, Up Arrow, D-Pad Up - Move Up  
A, Left Arrow, D-Pad Left - Move Left  
S, Down Arrow, D-Pad Down - Move Down  
D, Right Arrow, D-Pad Right - Move Right  
Esc or Start - Pause  
While paused or dead:  
F1 or Right Bumper to view or exit log  
Space to navigate through log  
F4 or Back Button to quit the game  

# Rules
Navigate the snake with the movement controls above. To score, the snake's head must hit a berry. The snake's tail will get longer with each berry eaten. If you collide with yourself or the walls, it's game over.

# About
This project uses a database to read and write the score and high score each game. It also writes to a text file for the log. Many actions in the code are logged, including when the database is read from/written to. It uses a list to keep track of the snake's position, and the tail is generated based on a calculation using the list. To load content, it uses a generic class. The log and database are generated locally where the game is ran if they do not exist. The log can be viewed in game by pausing (Esc) and pressing F1.

# Requirements
1. Create a dictionary or list, populate it with several values, retrieve at least one value, and use it in your program  
2. Implement a log that records errors, invalid inputs, or other important events and writes them to a text file  
3. Make a generic class and use it  
4. Add comments to your code explaining how you are using at least 2 of the solid principles