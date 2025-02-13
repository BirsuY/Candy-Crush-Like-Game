// by Ceren Birsu YILMAZ
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currState = GameState.move;
    public int width;
    public int height;
    private int offSet = 20;
    public GameObject[] cubes;
    public GameObject destroyEffect;
    public GameObject[,] cubes_all;
    public bool isDeadlocked;
    [Header("Manager Variables")]
    private MatchFinding findMatches;
    public CubeController currCube;
    private GameSoundManager soundManager;

    // Called on startup; initializes board dimensions, finds managers, and sets up the board.
    void Start()
    {
        width = 6;
        height = 6;
        cubes_all = new GameObject[width, height];
        findMatches = FindAnyObjectByType<MatchFinding>();
        soundManager = FindAnyObjectByType<GameSoundManager>();
        SetUp();
    }

    // Sets up the board by instantiating cubes from the pool.
    private void SetUp()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 tempPosition = new Vector2(x, y + offSet);
                int cubeToUse = Random.Range(0, cubes.Length);
                GameObject cube = CubePool.Instance.GetCube(cubes[cubeToUse], tempPosition, Quaternion.identity);
                
                CubeController cc = cube.GetComponent<CubeController>();
                cc.row = y;
                cc.column = x;
                cc.isMatched = false;
                cc.isTouched = false;
                
                cube.transform.parent = this.transform;
                cube.name = "( " + x + ", " + y + " )";
                cubes_all[x, y] = cube;
            }
        }
        //currCube.DetectSpecialCase();
    }

    // Destroys (or returns to pool) a matched cube at a given board position.
    // Also plays a sound and spawns a destroy effect.
    public void DestroyMatchedCubeAt(int column, int row)
    {
        if(cubes_all[column, row].GetComponent<CubeController>().isMatched)
        {
            if(cubes_all[column, row].GetComponent<CubeController>().isTouched)
            {
                findMatches.currMatches.Remove(cubes_all[column, row]);
               
                soundManager.PlayRandom();
                Debug.Log("Sound");
                GameObject particle = Instantiate(destroyEffect, cubes_all[column, row].transform.position, Quaternion.identity);
                Destroy(particle, .5f);
                CubePool.Instance.ReturnCube(cubes_all[column, row]);
                cubes_all[column, row] = null;
            }
        }
    }

    // Iterates over the board and destroys all cubes that are marked as matched.
    // Then it starts the process to update the board after destruction.
    public void DestroyMatched()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(cubes_all[x, y] != null &&
                cubes_all[x, y].GetComponent<CubeController>().isMatched)
                {
                    DestroyMatchedCubeAt(x, y);
                }
            }
        }
        StartCoroutine(HandleBoardAfterDestruction());
    }

    // Coordinates the sequence after destruction:
    private IEnumerator HandleBoardAfterDestruction()
    {
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(DecreaseRowCol());

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FieldBoardCol());
    }

    // Shifts remaining cubes downward to fill in the gaps left by destroyed cubes.
    private IEnumerator DecreaseRowCol()
    {
        int nullCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(cubes_all[x, y] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    cubes_all[x, y].GetComponent<CubeController>().row -= nullCount;
                    cubes_all[x, y] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.5f);
    }

    // Refills the board by retrieving cubes from the pool to replace null positions.
    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cubes_all[x, y] == null)
                {
                    Vector2 targetPos = new Vector2(x, y + offSet);
                    int cubeToUse = Random.Range(0, cubes.Length);
                    GameObject piece = CubePool.Instance.GetCube(cubes[cubeToUse], targetPos, Quaternion.identity);
                    
                    CubeController cc = piece.GetComponent<CubeController>();
                    cc.row = y;
                    cc.column = x;
                    cc.isMatched = false;
                    cc.isTouched = false;
                    
                    piece.transform.parent = this.transform;
                    piece.name = "( " + x + ", " + y + " )";
                    cubes_all[x, y] = piece;
                }
            }
        }
    }

    /// Checks the entire board to determine if there is any cube that is marked as matched.
    private bool MatchOnBoard() 
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cubes_all[x, y] != null)
                {
                    if(cubes_all[x, y].GetComponent<CubeController>().isMatched){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Fills board columns after destruction by calling RefillBoard and waiting for cubes to settle.
    // If new matches are found, triggers DestroyMatched() again.
    // Finally, clears current matches, checks for deadlock, and sets game state to move.
    private IEnumerator FieldBoardCol()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);

        while (MatchOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatched(); 
            yield break; 
        }

        FindAnyObjectByType<MatchFinding>().currMatches.Clear();
        currCube = null;
        yield return new WaitForSeconds(0.5f);
        DetectDeadlock();
        currState = GameState.move;
    }

    // Checks for deadlock by verifying whether any cube has been matched.
    public void DetectDeadlock(){
        if(findMatches.currMatches.Count == 0){
            isDeadlocked = true;
            //Debug.Log("Deadlocked!!");
        }
        else{
            isDeadlocked = false;
        }
    }

    private void ShuffleBoard(){
        DetectDeadlock();
        if(!isDeadlocked){
            return;
        }
        List<GameObject> newBoard = new List<GameObject>();
        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                if(cubes_all[i, j] != null){
                    newBoard.Add(cubes_all[i,j]);
                }
            }
        }

        for(int i = 0; i < width; i++){
            for(int j = 0; j < height; j++){
                if(cubes_all[i, j] != null){
                    int pieceToUse = Random.Range(0, newBoard.Count);
                    CubeController piece = newBoard[pieceToUse].GetComponent<CubeController>();
                    piece.column = i;
                    piece.row = j;
                    cubes_all[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        DetectDeadlock();
        if(isDeadlocked){
            ShuffleBoard();
        }


    }

    
}
