using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCEscape
{
  public class Program
  {
    static void Main(string[] args)
    {
    }

    private readonly int dim = 500;
    private int[][] visited;
    HashSet<Node> nodes;

    public int lowest(String[] harmful, String[] deadly)
    {
      List<Area> areas = BuildAreas(harmful, deadly);

      visited = new int[dim][];
      for (int i = 0; i < dim; i++)
      {
        visited[i] = Enumerable.Repeat(-1, dim).ToArray();
      }

      //bool[][] visited = new bool[dim][];
      //for (int i = 0; i < dim; i++)
      //{
      //  visited[i] = Enumerable.Repeat(false, dim).ToArray();
      //}

      int lives = int.MaxValue;
      //Queue<Tuple<int, int, int>> queue = new Queue<Tuple<int, int, int>>(); //x,y,step-count
      //queue.Enqueue(new Tuple<int, int, int>(0, 0, 0));

      MinPQ<Node> queue = new MinPQ<Node>(new NodeComparer());
      queue.insert(new Node() { distance = dim * 2, lives = 0, x = 0, y = 0 });
      nodes = new HashSet<Node>();

      //visited[0][0] = true;
      visited[0][0] = 0;
      while (queue.size() > 0)
      {
        //Tuple<int, int, int> now = queue.Dequeue();
        Node now = queue.delMin();

        int x = now.x;
        int y = now.y;
        int nowLives = now.lives;

        if (IsDestination(now))
        {
          UpdateLives(ref lives, nowLives);
        }
        else
        {

          ////already visited ?
          //if (visited[x][y])
          //{
          //  UpdateLives(ref lives, nowLives);
          //}
          //else
          //{
          //  visited[x][y] = true;

          Enqueue(queue, areas, nowLives, x - 1, y);
          Enqueue(queue, areas, nowLives, x + 1, y);
          Enqueue(queue, areas, nowLives, x, y - 1);
          Enqueue(queue, areas, nowLives, x, y + 1);

          //}
        }
      }
      return lives == int.MaxValue ? -1 : lives + 1;
    }

    //private void Enqueue(Queue<Tuple<int, int, int>> queue, List<Area> areas, int nowLives, int nx, int ny)
    private void Enqueue(MinPQ<Node> queue, List<Area> areas, int nowLives, int nx, int ny)
    {
      if (nx >= 0 && nx < dim && ny >= 0 && ny < dim)
      {
        if (isAllowed(areas, nx, ny))
        {
          int newLives = nowLives + GetDamage(areas, nx, ny);
          int current = visited[nx][ny];
          if (current == -1 || current > newLives)
          {
            visited[nx][ny] = newLives;
            int distance = dim - nx + dim - ny;
            //queue.Enqueue(new Tuple<int, int, int>(nx, ny, newLives));
            queue.insert(new Node() { x = nx, y = ny, lives = newLives, distance = distance });
          }
        }
      }
    }

    private bool isAllowed(List<Area> areas, int x, int y)
    {
      foreach (Area area in areas)
      {
        if (area.areaType == AreaType.DEADLY && area.isContains(x, y))
        {
          return false;
        }
      }
      return true;
    }

    private int GetDamage(List<Area> areas, int x, int y)
    {
      foreach (Area area in areas)
      {
        if (area.areaType == AreaType.HARMFUL && area.isContains(x, y))
        {
          return 1;
        }
      }
      return 0;
    }

    private void UpdateLives(ref int lives, int nowLives)
    {
      if (nowLives < lives)
      {
        lives = nowLives;
      }
    }

    private bool IsDestination(Node node)//Tuple<int, int, int> pos)
    {
      //return (pos.Item1 == dim - 1 && pos.Item2 == dim - 1);
      return (node.x == dim - 1 && node.y == dim - 1);
    }

    private List<Area> BuildAreas(string[] harmful, string[] deadly)
    {
      List<Area> areas = new List<Area>();

      BuildAreas(areas, harmful, AreaType.HARMFUL);
      BuildAreas(areas, deadly, AreaType.DEADLY);

      return areas;
    }

    private void BuildAreas(List<Area> areas, string[] areaStrings, AreaType areaType)
    {
      if (areaStrings == null || areaStrings.Length == 0)
      {
        return;
      }

      foreach (string areaString in areaStrings)
      {
        if (String.IsNullOrEmpty(areaString)) { continue; }

        Area newArea = BuildArea(areaString, areaType);
        areas.Add(newArea);
      }
    }

    private Area BuildArea(string areaString, AreaType areaType)
    {
      int[] coords = areaString.Split().Select(x => int.Parse(x)).ToArray();

      Area newArea = new Area();
      newArea.areaType = areaType;
      newArea.xmin = Math.Min(coords[0], coords[2]);
      newArea.xmax = Math.Max(coords[0], coords[2]);
      newArea.ymin = Math.Min(coords[1], coords[3]);
      newArea.ymax = Math.Max(coords[1], coords[3]);

      return newArea;
    }

    private struct Area
    {
      public int xmin, xmax, ymin, ymax;
      public AreaType areaType;

      public bool isContains(int px, int py)
      {
        return px >= xmin && px <= xmax && py >= ymin && py <= ymax;
      }
    }

    private enum AreaType
    {
      DEADLY,
      HARMFUL
    }

    private class Node : IComparable
    {
      public int lives;
      public int distance;
      public int x;
      public int y;

      public int CompareTo(object obj)
      {
        Node other = obj as Node;
        return (lives + distance) - (other.lives + other.distance);
      }

      public override bool Equals(object obj)
      {
        Node other = obj as Node;
        return other.x == x && other.y == y;
      }
    }

    private class NodeComparer : Comparer<Node>
    {
      public override int Compare(Node x, Node y)
      {
        return (x.lives + x.distance) - (y.lives + y.distance);
      }
    }
  }
}
