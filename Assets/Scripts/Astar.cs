using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    private Dictionary<Vector2Int, Node> OpenNodes;
    private Dictionary<Vector2Int, Node> ClosedNodes;

    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        OpenNodes = new();
        ClosedNodes = new();

        OpenNodes.Add(startPos, CreateNode(startPos, null, 0, (int)Vector2Int.Distance(startPos, endPos)));

        while (OpenNodes.Count > 0)
        {
            //a Look for lowest F score.
            Node lowestScoreNode = OpenNodes.First().Value;
            foreach (var openNode in OpenNodes)
            {
                lowestScoreNode = openNode.Value.FScore <= lowestScoreNode.FScore
                ? openNode.Value : lowestScoreNode;
            }

            //b
            OpenNodes.Remove(lowestScoreNode.position);

            //c
            List<Node> neighbours = GetNeighbours(lowestScoreNode, grid);

            //d
            foreach (var neighbour in neighbours)
            {
                //i
                if (neighbour.position == endPos) return GetRoute(neighbour);

                //ii
                neighbour.GScore = lowestScoreNode.GScore + Vector2Int.Distance(neighbour.position, lowestScoreNode.position);
                neighbour.HScore = Vector2Int.Distance(neighbour.position, endPos);

                //iii
                if (OpenNodes.ContainsKey(neighbour.position)
                    && OpenNodes[neighbour.position].FScore < neighbour.FScore) continue;
                //iv
                if (ClosedNodes.ContainsKey(neighbour.position)
                    && ClosedNodes[neighbour.position].FScore < neighbour.FScore) continue;

                OpenNodes[neighbour.position] = neighbour;
            }

            //e
            if (!ClosedNodes.ContainsKey(lowestScoreNode.position))
                ClosedNodes.Add(lowestScoreNode.position, lowestScoreNode);
        }

        return GetClosesdRoute();
    }

    private List<Node> GetNeighbours(Node node, Cell[,] grid)
    {
        List<Node> reachable = new();
        List<Cell> cellList = grid[node.position.x, node.position.y].GetNeighbours(grid);

        foreach (var cell in cellList)
        {
            Node currentNode = CreateNode(cell.gridPosition, node, 0, 0);

            if (!cell.HasWall(Wall.DOWN) && cell.gridPosition == node.position + Vector2Int.up) reachable.Add(currentNode);
            if (!cell.HasWall(Wall.RIGHT) && cell.gridPosition == node.position + Vector2Int.left) reachable.Add(currentNode);
            if (!cell.HasWall(Wall.LEFT) && cell.gridPosition == node.position + Vector2Int.right) reachable.Add(currentNode);
            if (!cell.HasWall(Wall.UP) && cell.gridPosition == node.position + Vector2Int.down) reachable.Add(currentNode);
        }

        return reachable;
    }

    private List<Vector2Int> GetRoute(Node endNode)
    {
        List<Vector2Int> route = new();
        Node currentNode = endNode;


        while (currentNode.parent != null)
        {
            route.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        route.Reverse(); // Feels like a bandaid solution...

        return route;
    }

    private List<Vector2Int> GetClosesdRoute()
    {
        //Try get to closesd node.
        Node closesdNode = ClosedNodes.First().Value; ;
        foreach (var node in ClosedNodes)
        {
            closesdNode = node.Value.HScore < closesdNode.HScore
            ? node.Value : closesdNode;
        }

        return GetRoute(closesdNode);
    }

    private Node CreateNode(Vector2Int position, Node parent, int GScore, int HScore)
    {
        return new Node(
            position,
            parent,
            GScore,
            HScore
        );
    }

    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore
        { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
