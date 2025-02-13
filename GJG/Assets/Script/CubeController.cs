using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    private int targetX;
    private int targetY;
    private Board board;
    public GameObject otherCube;
    private Vector2 touchPos;
    private Vector2 targetPos;
    [Header("Match State")] 
    public bool isMatched = false;
    private MatchFinding findMatches;
    public int prevCol;
    public int prevRow;

    [Header("Special Case Variables")] 
    public bool isACase;
    public Sprite A_Case;

    public bool isBCase;
    public Sprite B_Case;

    public bool isCCase;
    public Sprite C_Case;

    public Sprite default_Case;
    public bool isTouched = false;

    // Initializes the cube and finds the necessary components
    private void Start()
    {
        Reset();
        board = FindAnyObjectByType<Board>();
        findMatches = FindAnyObjectByType<MatchFinding>();

    }

    // Resets the cube state and applies the default sprite
    public void Reset(){
        isACase = false;
        isBCase = false;
        isCCase = false;
        ApplyCaseSprite("Default");

    }

    // Handles the cube's movement towards its target position on the board
    private void Update()
    {
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            targetPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, targetPos, .6f);
            if(board.cubes_all[column, row] != this.gameObject)
            {
                board.cubes_all[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            targetPos = new Vector2(targetX, transform.position.y);
            transform.position = targetPos;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            targetPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, targetPos, .6f);
            if (board.cubes_all[column, row] != this.gameObject)
            {
                board.cubes_all[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();

        }
        else
        {
            targetPos = new Vector2(transform.position.x, targetY);
            transform.position = targetPos;
        }
    }

    // Waits and then checks if the move resulted in a match
    public IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(.5f);
        if(!isMatched){
            board.currCube = null;
            board.currState = GameState.move;
        }
        else
        {
            board.DestroyMatched();
            
        }    
        
        
    }

    // Handles cube selection on mouse click
    private void OnMouseDown()
    {
        if (board.currState == GameState.move)
        {
            touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = (int)Mathf.Round(touchPos.x);
            int y = (int)Mathf.Round(touchPos.y);

            GameObject touched_cube = board.cubes_all[x, y];
            if (touched_cube == null) return;

            touched_cube.GetComponent<CubeController>().isTouched = true;

            foreach (var group in findMatches.matchedGroups)
            {
                if (group.Contains(touched_cube))
                {
                    foreach (GameObject cube in group)
                    {
                        cube.GetComponent<CubeController>().isTouched = true;
                    }
                    StartCoroutine(CheckMove());
                    break;
                    
                }
            }
            
            board.currCube = this;
        }
        
    }

    // Identifies and applies the correct special case sprite
    public void DetectSpecialCase(){
        if(transform.position.x > column || transform.position.x < 0){
            return;
        }
        if(transform.position.y > row || transform.position.y < 0){
            return;
        }
        if(isACase){
            ApplyCaseSprite("A");
        }
        else if(isBCase){
            ApplyCaseSprite("B");
        }
        else if(isCCase){
            ApplyCaseSprite("C");
        }
        else{
            ApplyCaseSprite("default");
        }
   
    }

    // Changes the sprite of the cube based on its special case type
    public void ApplyCaseSprite(string caseType)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if(sr == null)
            return;
        
        switch(caseType)
        {
            case "A":
                sr.sprite = A_Case;
                break;
            case "B":
                sr.sprite = B_Case;
                break;
            case "C":
                sr.sprite = C_Case;
                break;
            default:
                sr.sprite = default_Case;
                break;
        }
    }


}
