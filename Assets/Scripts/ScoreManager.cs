using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class ScoreManager : MonoBehaviour
{
    private Board board;
    public TMP_Text ScoreText;
    public int score;
    public Image scoreBar;
    public GameObject winnerPanel;
    int nextLevelGoalScore = 0;
    // Start is called before the first frame update
    int Level;
    void Start()
    {
        Level = SceneManager.GetActiveScene().buildIndex;
        if (Level == 0)
        {
            nextLevelGoalScore = 500;
        }
        else if (Level == 1)
        {
            nextLevelGoalScore = 800;

        }else 
        {
            nextLevelGoalScore = 500;
        }
        Debug.Log(nextLevelGoalScore);

        board = FindObjectOfType<Board>();
      

        
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = "" + score;
        if(board !=null && scoreBar != null )
        {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount= (float)score / (float)board.scoreGoals[length-1];
        }
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
       
   
      
        if (score == nextLevelGoalScore)
        {
            winnerPanel.SetActive(true);
        }

    }
    public void ChangeScene()
    {
        SceneManager.LoadScene("level 2");
    }
    public void ResetScene()
    {
        SceneManager.LoadScene("level 1");
    }
    public void resetScene2()
    {
        SceneManager.LoadScene("level 2");
    }
    public void OnApplicationQuit()
    {
        Application.Quit();
    }

}
