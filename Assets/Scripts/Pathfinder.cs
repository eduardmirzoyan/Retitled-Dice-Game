using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder
{
    private class Node 
    {
        public Vector3Int location;

        public int G;
        public int H;
        public int F { get { return G + H; }}
        public Node previous;

        public Node(Vector3Int location) 
        {
            this.location = location;
            previous = null;
        }
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end, Room room)
    {
        // Find path using A*
        var open = new List<Node>(); // Makes sure no copies are kept, idealy should be prio queue
        var closed = new List<Node>();

        var startNode = new Node(start);

        // Start from start
        open.Add(startNode);

        while (open.Count > 0)
        {
            // Sort by F value then get first item
            var currentNode = open.OrderBy(node => node.F).First();

            open.Remove(currentNode);
            closed.Add(currentNode);

            if (currentNode.location == end)
            {
                // Debug
                // Debug.Log(open.Count);
                // string debug = "Open: ";
                // foreach (var location in open)
                // {
                //     debug += location.location + " ";
                // }
                // Debug.Log(debug);

                // Return finalized path
                return GetFinalPath(startNode, currentNode);
            }

            // Get neighbors
            var neighbors = room.GetNeighbors(currentNode.location);

            foreach (var neighbor in neighbors)
            {
                // Make node
                var neighborNode = new Node(neighbor);

                // Skip if closed contains node
                if (closed.Any(node => node.location == neighbor))
                {
                    continue;
                }

                // Update values
                neighborNode.G = ManhattanDistance(start, neighbor); // G
                neighborNode.H = ManhattanDistance(end, neighbor); // H

                // Update previous
                neighborNode.previous = currentNode;

                // Make sure no copies exist
                if (!open.Any(node => node.location == neighbor))
                    open.Add(neighborNode);
            }
        }
        
        // Return empty list
        return new List<Vector3Int>();
    }

    private List<Vector3Int> GetFinalPath(Node start, Node end) 
    {
        List<Vector3Int> result = new List<Vector3Int>();

        var current = end;

        while (current != null)
        {
            // Add location
            result.Add(current.location);
            // Increment
            current = current.previous;
        }

        // Reverse list
        result.Reverse();

        return result;
    }

    public int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
