using System;
using System.Collections.Generic;
using System.IO;

class TurtleGame
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: TurtleGame <game-settings-file> <moves-file>");
            return;
        }

        string gameSettingsFile = args[0];
        string movesFile = args[1];

        try
        {   // Fetch the game settings from the Gamesettings.txt file
            GameSettings settings = ReadGameSettings(gameSettingsFile);

            // Read move sequences from the Moves.txt file
            List<string> moveSequences = ReadMoveSequences(movesFile);
            // Iterates through each move sequence and plays the game.
            int sequenceNumber = 1;
            foreach (string moveSequence in moveSequences)
            {
                Console.Write($"Sequence {sequenceNumber++}: ");
                
                PlayGame(settings, moveSequence);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static GameSettings ReadGameSettings(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        int n = int.Parse(lines[0].Split(' ')[0]);
        int m = int.Parse(lines[0].Split(' ')[1]);
        int startX = int.Parse(lines[1].Split(' ')[0]);
        int startY = int.Parse(lines[1].Split(' ')[1]);
        Direction startDirection = Enum.Parse<Direction>(lines[1].Split(' ')[2], true);
        int exitX = int.Parse(lines[2].Split(' ')[0]);
        int exitY = int.Parse(lines[2].Split(' ')[1]);

        List<(int, int)> mines = new List<(int, int)>();
        for (int i = 3; i < lines.Length; i++)
        {
            int mineX = int.Parse(lines[i].Split(' ')[0]);
            int mineY = int.Parse(lines[i].Split(' ')[1]);
            mines.Add((mineX, mineY));
        }

        return new GameSettings(n, m, startX, startY, startDirection, exitX, exitY, mines);
    }

    static List<string> ReadMoveSequences(string filePath)
    {
        return new List<string>(File.ReadAllLines(filePath));
    }

    // Inside the PlayGame method in the TurtleGame class
    static void PlayGame(GameSettings settings, string moveSequence)
    {
        Turtle turtle = new Turtle(settings.StartX, settings.StartY, settings.StartDirection);

        foreach (char move in moveSequence)
        {
            if (move == 'm')
            {
                if (!turtle.Move(settings.N, settings.M, settings.Mines))
                {
                    Console.WriteLine("Mine hit!");
                    return; // when Turtle hit a mine, exit the method

                }

                // Check exit condition after each 'm' move 
                if (turtle.IsAtExit(settings.ExitX, settings.ExitY))
                {
                    Console.WriteLine("Success!");
                    return; //  when Turtle reached the exit, exit the method
                }


            }
            else if (move == 'r')
            {
                turtle.RotateRight();
            }

        }
        // Check exit condition after the entire sequence is completed
        if (turtle.IsAtExit(settings.ExitX, settings.ExitY))
        {
            Console.WriteLine("Success!");
        }
        else
        {   // Turtle postion in danger
            Console.WriteLine("Still in danger!");
        }
    }

}

class GameSettings
{
    public int N { get; }  // X axis of boardsize
    public int M { get; }  // Y axis of Boardsize
    public int StartX { get; }  // Turtle X axis position
    public int StartY { get; }   // Turtle y axis position
    public Direction StartDirection { get; } // Turtle Direction position
    public int ExitX { get; }   // Exit X axis position
    public int ExitY { get; }    // Exit Y axis position
    public List<(int, int)> Mines { get; }  // List of Mine positions

    public GameSettings(int n, int m, int startX, int startY, Direction startDirection, int exitX, int exitY, List<(int, int)> mines)
    {
        N = n;
        M = m;
        StartX = startX;
        StartY = startY;
        StartDirection = startDirection;
        ExitX = exitX;
        ExitY = exitY;
        Mines = mines;
    }
}

enum Direction
{
    North,
    East,
    South,
    West
}

class Turtle
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Direction Facing { get; private set; }

    public Turtle(int x, int y, Direction facing)
    {  
        X = x;
        Y = y;
        Facing = facing;
    }

    public void RotateRight()
    {
        Facing = (Direction)(((int)Facing + 1) % 4);
    }

    public bool Move(int n, int m, List<(int, int)> mines)
    {
        int nextX = X;
        int nextY = Y;

        switch (Facing)
        {
            case Direction.North:
                nextY--;
                break;
            case Direction.East:
                nextX++;
                break;
            case Direction.South:
                nextY++;
                break;
            case Direction.West:
                nextX--;
                break;
        }

        // Check if any mines there
        if (mines.Contains((nextX, nextY)))
        {
            return false; // Hit a mine
        }

        // Check boundaries
        if (nextX >= 0 && nextX <=m && nextY >= 0 && nextY <n)
        {         
            X = nextX;
            Y = nextY;
            return true; // Move successful
        }

        return false; 
    }

    public bool IsAtExit(int exitX, int exitY)
    {
        return X == exitX && Y == exitY; // reached Exit point
    }
}
