using System;
using System.Collections.Generic;
namespace Quoridor.AI
{
    class CustomAgent : Agent
    {
        GameData data;
        List<MoveAction> moves = new List<MoveAction>();
        List<List<Node>> paths = new List<List<Node>>();
        int goalPosition;
        int optimalDirection;
        int enemyWalls = 1000;
        int distanceToGoal;
        int distanceFromStart;
        public static void Main()
        {
            new CustomAgent().Start();
        }

        public override Action DoAction(GameData status)
        {
            moves.RemoveAt(0);
            data = status;
            GetGoal();
            foreach (Player p in status.Players)
            {
                if (p != status.Self)
                {
                    if (p.NumberOfWalls < enemyWalls)
                    {
                        if (goalPosition == 0)
                        {
                            Point p1 = new Point(data.Tiles.GetLength(0) / 2, data.Tiles.GetLength(1));
                            Point p2 = new Point(data.Self.Position.X, data.Self.Position.Y);
                            distanceFromStart = (p1.X - p2.X) + (p1.Y - p2.Y);
                            
                        }
                        else
                        {
                            distanceToGoal = data.Self.Position.Y;
                        }
                        
                        enemyWalls = p.NumberOfWalls;
                        moves.Clear();
                        FindPath(status.Self.Position);
                    }
                }
            }
            return moves[0];
        }

        private void FindPath(Point pos)
        {
            int i = FindLowestWeight();
            if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X, data.Self.Position.Y + optimalDirection)) && IsWithinGameBoard(data.Self.Position.X, data.Self.Position.Y + optimalDirection))
            {
                paths.Add(new List<Node> { new Node(distanceFromStart, goalPosition - data.Self.Position.Y, data.Self.Position) });
                paths[i].Add(new Node(distanceFromStart, goalPosition - data.Self.Position.Y, data.Self.Position));
            }
            else if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X + 1, data.Self.Position.Y)) && IsWithinGameBoard(data.Self.Position.X + 1, data.Self.Position.Y))
            {

            }
            else if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X - 1, data.Self.Position.Y)) && IsWithinGameBoard(data.Self.Position.X - 1, data.Self.Position.Y))
            {

            }
            else if (!WallBetween(data.Self.Position, new Point(data.Self.Position.X, data.Self.Position.Y - optimalDirection)) && IsWithinGameBoard(data.Self.Position.X, data.Self.Position.Y - optimalDirection))
            {

            }
            else
            {
                Console.WriteLine("Something dun goofd");
            }
            return;
        }
        private int FindLowestWeight()
        {
            int i = 0;
            foreach(List<Node> path in paths)
            {
                foreach(Node node in path)
                {
                    foreach(List<Node> nextPath in paths)
                    {
                        foreach(Node nextNode in nextPath)
                        {
                            if(node.weight < nextNode.weight)
                            {
                                i = paths.IndexOf(path);
                            }
                        }
                    }
                }
            }
            return i;
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
    }
}