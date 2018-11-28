using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor.AI
{
    class CustomAgent : Agent
    {
        GameData data;
        LinkedList<MoveAction> moves = new LinkedList<MoveAction>();
        List<LinkedList<Node>> paths = new List<LinkedList<Node>>();
        Point blocked;
        bool initalized = false;
        int goalPosition;
        int optimalDirection;
        int knownNumberOfEnemyWalls = 1000;
        int distanceToGoal;
        int distanceFromStart;
        int distanceFromPos;
        int currentPath;
        int currentWeight;
        int forward;
        int backward;
        int rightward;
        int leftward;

        public static void Main()
        {
            new CustomAgent().Start();
        }

        public override Action DoAction(GameData status)
        {
            if (!initalized)
            {
                data = status;
                Initalize();
                initalized = true;
            }
            moves.RemoveFirst();

            foreach (Player p in status.Players)
            {
                if (p != status.Self)
                {
                    blocked = p.Position;
                    if (ThereIsBlocker())
                    {
                        knownNumberOfEnemyWalls = p.NumberOfWalls;
                        moves.Clear();
                        paths.Clear();
                        FindPath("none");
                    }
                    else if (p.NumberOfWalls < knownNumberOfEnemyWalls)
                    {
                        knownNumberOfEnemyWalls = p.NumberOfWalls;
                        if (ThereIsWall())
                        {
                            moves.Clear();
                            paths.Clear();
                            FindPath("none");
                        }
                    }
                }
            }
            return moves.First();
        }

        private void FindPath(string direction)
        {
            if (direction == "none")
            {
                DrawInitialPath();
                FindLowestWeight();
            }
            if (currentWeight < paths[currentPath].First.Value.weight)
            {
                FindLowestWeight();
            }
            if (!WallBetween(paths[currentPath].First.Value.Position, new Point(paths[currentPath].First.Value.Position.X, paths[currentPath].First.Value.Position.Y + optimalDirection)) && IsWithinGameBoard(paths[currentPath].First.Value.Position.X, paths[currentPath].First.Value.Position.Y + optimalDirection))
            {
                //Skapa en ny lista av noder till paths[] som är en kopia av paths[currentPath]
                //Lägg till den nya noden i den nya listan
                //Men detta ska bara behöva göras om den nya noden är ytterligare en nod, tror jag?
                //Det vill säga, om vi hittar en öppen nod neråt, så lägger vi bara till den i currentpath
                //Och om vi hittar en till öppen nod i någon av de andra riktningarna, så lägger vi till en kopia ala ovan instruktioner.
                //Kanske inte här inne, men vi behöver hålla reda på om vi kommit in i en återvändsgränd, också.
                LinkedList<Node> temp = paths[currentPath];
                temp.AddFirst(new Node(distanceFromStart, goalPosition - data.Self.Position.Y, data.Self.Position));
                paths.Add(temp);
                paths[currentPath].AddFirst(new Node(distanceFromStart, goalPosition - data.Self.Position.Y, data.Self.Position));
            }
            if (!WallBetween(paths[currentPath].First.Value.Position, new Point(paths[currentPath].First.Value.Position.X + 1, paths[currentPath].First.Value.Position.Y)) && IsWithinGameBoard(paths[currentPath].First.Value.Position.X + 1, paths[currentPath].First.Value.Position.Y))
            {

            }
            if (!WallBetween(paths[currentPath].First.Value.Position, new Point(paths[currentPath].First.Value.Position.X - 1, paths[currentPath].First.Value.Position.Y)) && IsWithinGameBoard(paths[currentPath].First.Value.Position.X - 1, paths[currentPath].First.Value.Position.Y))
            {

            }
            if (!WallBetween(paths[currentPath].First.Value.Position, new Point(paths[currentPath].First.Value.Position.X, paths[currentPath].First.Value.Position.Y - optimalDirection)) && IsWithinGameBoard(paths[currentPath].First.Value.Position.X, paths[currentPath].First.Value.Position.Y - optimalDirection))
            {

            }
            if (paths[currentPath].First.Value.Position.Y == goalPosition)
            {
                foreach (Node n in paths[currentPath])
                {
                    moves.AddFirst(new MoveAction(n.Position.X, n.Position.Y));
                }
                //Put nodes into moveaction list.
            }
            else
            {
                FindPath();
            }
            return;
        }

        private bool ThereIsWall()
        {
            if (paths.Count == 0)
            {
                return true;
            }
            if (WallBetween(data.Self.Position, paths[currentPath].ElementAt(0).Position))
            {
                return true;
            }
            for (int x = 0; x < paths[currentPath].Count - 1; x++)
            {
                if (WallBetween(paths[currentPath].ElementAt(x).Position, paths[currentPath].ElementAt(x + 1).Position))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ThereIsBlocker() // Needs fix
        {
            if (paths.Count == 0)
            {
                return true;
            }
            if (WallBetween(data.Self.Position, paths[currentPath].ElementAt(0).Position))
            {
                return true;
            }
            for (int x = 0; x < paths[currentPath].Count - 1; x++)
            {
                if (WallBetween(paths[currentPath].ElementAt(x).Position, paths[currentPath].ElementAt(x + 1).Position))
                {
                    return true;
                }
            }
            return false;
        }

        private void DrawInitialPath()
        {
            int x = 0;
            distanceFromStart = 1;
            if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X, data.Self.Position.Y + optimalDirection)) && IsWithinGameBoard(data.Self.Position.X, data.Self.Position.Y + optimalDirection))
            {
                paths.Add(new LinkedList<Node>());
                paths[x].AddFirst(new Node(distanceFromStart, goalPosition - (data.Self.Position.Y + optimalDirection), new Point(data.Self.Position.X, data.Self.Position.Y + optimalDirection)));
                x++;
            }
            if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X + 1, data.Self.Position.Y)) && IsWithinGameBoard(data.Self.Position.X + 1, data.Self.Position.Y))
            {
                paths.Add(new LinkedList<Node>());
                paths[x].AddFirst(new Node(distanceFromStart, goalPosition - data.Self.Position.Y, new Point(data.Self.Position.X + 1, data.Self.Position.Y)));
                x++;
            }
            if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X - 1, data.Self.Position.Y)) && IsWithinGameBoard(data.Self.Position.X - 1, data.Self.Position.Y))
            {
                paths.Add(new LinkedList<Node>());
                paths[x].AddFirst(new Node(distanceFromStart, goalPosition - data.Self.Position.Y, new Point(data.Self.Position.X - 1, data.Self.Position.Y)));
                x++;
            }
            if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X, data.Self.Position.Y - optimalDirection)) && IsWithinGameBoard(data.Self.Position.X, data.Self.Position.Y - optimalDirection))
            {
                paths.Add(new LinkedList<Node>());
                paths[x].AddFirst(new Node(distanceFromStart, goalPosition - (data.Self.Position.Y - optimalDirection), new Point(data.Self.Position.X, data.Self.Position.Y - optimalDirection)));
            }
        }

        private void FindLowestWeight()
        {
            int x = 0;
            for (int i = 0; i < paths.Count; i++)
            {
                for (int j = i + 1; j < paths.Count; i++)
                {
                    if (paths[i].First.Value.weight < paths[j].First.Value.weight)
                    {
                        x = i;
                    }
                }
            }
            currentPath = x;
            currentWeight = paths[x].First.Value.weight;
        }

        private bool WallBetween(Point start, Point end)
        {
            if (start.X == end.X)
            {
                int num = Math.Min(start.Y, end.Y);
                return data.HorizontalWall[start.X, num];
            }
            if (start.Y == end.Y)
            {
                int num2 = Math.Min(start.X, end.X);
                return data.VerticalWall[num2, start.Y];
            }
            return true;
        }

        protected bool IsWithinGameBoard(int column, int row)
        {
            if (0 <= column && column < data.Tiles.GetLength(0) && 0 <= row)
            {
                return row < data.Tiles.GetLength(1);
            }
            return false;
        }

        private void GetGoal()
        {
            if (data.Self.Position.Y == 0)
            {
                goalPosition = data.Tiles.GetLength(1);
                optimalDirection = 1;
            }
            else if (data.Self.Position.Y == data.Tiles.GetLength(1))
            {
                goalPosition = 0;
                optimalDirection = -1;
            }
        }
        private void Initalize()
        {
            GetGoal();
            forward = data.Self.Position.Y + optimalDirection;
            backward = data.Self.Position.Y - optimalDirection;
            rightward = data.Self.Position.X + 1;
            leftward = data.Self.Position.X - 1;
        }
    }
}