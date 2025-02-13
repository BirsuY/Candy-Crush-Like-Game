// by Ceren Birsu YILMAZ
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinding : MonoBehaviour
{
    private Board board;
    public List<GameObject> currMatches = new List<GameObject>();
    public List<List<GameObject>> matchedGroups = new List<List<GameObject>>();

    // Pre-cached directions (left, right, down, up) used for flood-fill search
    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
    };

    // Called on script initialization. Finds and stores a reference to the Board.
    void Start()
    {
        board = FindAnyObjectByType<Board>();
    }

    // Initiates the match-finding process.
    // Changes the game state to 'wait' and starts the matching coroutine.
    public void FindAllMatches()
    {
        board.currState = GameState.wait;
        StartCoroutine(FindAllMatchesCo());
    }

    // Coroutine that performs a flood-fill search on the board to find all match groups.
    // For each group found with 2 or more cubes, it marks cubes as matched and assigns special cases.
    // After processing, it resets any cube that isn't matched.
    // Finally, the game state is set back to move.
    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);

        matchedGroups.Clear();
        currMatches.Clear();

        bool[,] visited = new bool[board.width, board.height];

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (visited[x, y])
                    continue;

                GameObject currCube = board.cubes_all[x, y];
                if (currCube == null)
                    continue;

                string cubeTag = currCube.tag;

                List<GameObject> matchGroup = new List<GameObject>();
                Queue<Vector2Int> queue = new Queue<Vector2Int>();
                queue.Enqueue(new Vector2Int(x, y));

                while (queue.Count > 0)
                {
                    Vector2Int pos = queue.Dequeue();
                    int qx = pos.x, qy = pos.y;

                    if (qx < 0 || qx >= board.width || qy < 0 || qy >= board.height)
                        continue;

                    if (visited[qx, qy])
                        continue;

                    GameObject cube = board.cubes_all[qx, qy];
                    if (cube == null || cube.tag != cubeTag)
                        continue;

                    visited[qx, qy] = true;
                    matchGroup.Add(cube);

                    for (int i = 0; i < directions.Length; i++)
                    {
                        Vector2Int nextPos = new Vector2Int(qx + directions[i].x, qy + directions[i].y);
                        queue.Enqueue(nextPos);
                    }
                }

                if (matchGroup.Count >= 2)
                {

                    matchedGroups.Add(matchGroup);
                    int size = matchGroup.Count;
                    for (int i = 0; i < matchGroup.Count; i++)
                    {
                        
                        CubeController cubeController = matchGroup[i].GetComponent<CubeController>();
                        if (cubeController != null)
                        {
                            cubeController.isMatched = true;
                            cubeController.Reset();
                        }

                        if(size >= 10){
                            cubeController.isCCase = true;
                        }
                        else if(size >= 7){
                            cubeController.isBCase = true;
                        }
                        else if(size >= 5){
                            cubeController.isACase = true;
                        }
                        currMatches.Add(matchGroup[i]);
                        cubeController.DetectSpecialCase();

                    }

                }
            }
        }

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (board.cubes_all[x, y] != null)
                {
                    CubeController cc = board.cubes_all[x, y].GetComponent<CubeController>();
                    if (cc != null && !cc.isMatched)
                    {
                        cc.Reset();
                    }
                }
            }
        }
        
        board.currState = GameState.move;
    }
}
