using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.UIElements;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    private FindMatches findMatches;
    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    public GameObject winnerPanel;
    
    public int[]scoreGoals;
   
    // Start is called before the first frame update
    void Start()
    {
        findMatches= FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        scoreManager= FindObjectOfType<ScoreManager>();
        Setup();
    }

   
    public void Setup()
    {
        for(int i = 0;i< width;i++)
        {
            for(int j = 0;j< height;j++)
            {
                Vector2 tempPosition = new Vector2(i,j + offset);
                Vector2 tilePosition = new Vector2(i, j);
               GameObject backgroundTile= Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                int dotToUse = Random.Range(0, dots.Length);

                int maxIterations = 0;

                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i,j] = dot;
            }
        }
    }

    private bool MatchesAt(int column,int row, GameObject piece)
    {
        if( column> 1 && row> 1 )
        {
            if (allDots[column-1, row ].tag== piece.tag && allDots[column-2,row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column , row-1].tag == piece.tag && allDots[column , row-2].tag == piece.tag)
            {
                return true;
            }

        }
        else if(column <=1 || row<=1)
        {
            if(row>1 )
            {
                if (allDots[column,row-1].tag == piece.tag && allDots[column,row-2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column-1, row ].tag == piece.tag && allDots[column-2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column,int row) 
    {
        if (allDots[column,row].GetComponent<Dot>().isMatched) 
        {
            //findMatches.currentMatches.Remove(allDots[column,row]);
            SoundManager.Instance.PlayRandomDestroySound();
            GameObject Particle =Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(Particle, .5f);
            Destroy(allDots[column,row]);
            scoreManager.IncreaseScore(basePieceValue * streakValue);
           
            
            allDots[column,row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int i = 0;i<width;i++)
        {
            for(int j = 0;j<height;j++)
            {
                if (allDots[i,j] !=null)
                {
                    DestroyMatchesAt(i,j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
       
        for(int i = 0;i<width; i++)
        {
            for(int j = 0;j<height; j++)
            {
                if (allDots[i,j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for(int i= 0 ;i<width;i++)
        {
            for(int j = 0;j<height;j++)
            {
                if (allDots[i,j] == null)
                {
                    Vector2 tempPosition = new Vector2(i,j + offset);
                    int dotToUse = Random.Range(0,dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse],tempPosition,Quaternion.identity);
                    allDots[i,j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }

    }

    private bool MatchesOnBoard()
    {
        for(int i= 0; i < width; i++)
        {
            for( int j= 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    if (allDots[i,j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard())
        {
            streakValue++;
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
        streakValue = 1;
    }
}
