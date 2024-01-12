using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum cellType {blue = 0, green = 1, purple = 2, yellow = 3, red = 4};

public class GridManager : MonoBehaviour
{
    [SerializeField]
    int rowSize = 6;
    [SerializeField]
    int colSize = 6;

    [SerializeField]
    float cellSize = 1.0f; // Set the size of each cell

    [SerializeField]
    public Sprite[] cellSprites;

    [SerializeField]
    public GameObject gridCellPrefab;

    [SerializeField]
    public TMP_Text timerText;

    [SerializeField]
    public TMP_Text scoreText;

    private int curDifficultyLevel = 1;
    private List<List<GameObject>>  grid = new List<List<GameObject>>(); //2D list of rows of cells
    private List<GameObject> allCells = new List<GameObject>(); //list of all cell game objects, used for cleanup purposes

    public bool timerRunning = false;
    public float timeLimit = 60.0f;
    private float timer = 0;
    private int playerScore = 0;
    private int LevelGoal = 10;

    private List<GameObject> selectedCells = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            //update timer
            timer += Time.deltaTime;

            //check for time limit
            if (timer >= timeLimit)
            {
                //game over!
                timerRunning = false;
                timer = 0;

                //save to scoreboard here

                if (playerScore > LevelGoal)
                {
                    //Player won!


                }
                else
                {
                    //player lost...
                    //show restart button

                }

            

                if (curDifficultyLevel > 3)
                {
                    //show restart button
                    //todo allow user to keep going?
                }

            }

            //update UI text
            timerText.text = timer.ToString();
            scoreText.text = playerScore.ToString();

            //Check for player connected allCells

        }

    }

    public void StartGameButton()
    {
        foreach (var cell in allCells)
        {
            Destroy(cell.gameObject);
        }

        grid.Clear();
        allCells.Clear();

        playerScore = 0;

        LevelGoal = curDifficultyLevel * 10;

        GenerateGrid(curDifficultyLevel);
        timer = 0;
        timerRunning = true;
    }

    public void GenerateGrid(int difficultyLevel)
    {
        Vector3 offset = new Vector3(-2.5f, 2.5f, 0f); // Offset to center the grid

        for (int row = 0; row < rowSize; row++)
        {
            List<GameObject> rowCells = new List<GameObject>();

            for (int col = 0; col < colSize; col++)
            {
                Vector3 position = new Vector3(col * cellSize, -row * cellSize, 0f) + offset;
                GameObject cell = Instantiate(gridCellPrefab, position, Quaternion.identity,this.transform);

                //randomize cell type based on difficulty
                int newCellType=0;

                if (difficultyLevel <= 1)
                {
                    newCellType = UnityEngine.Random.Range(0, cellSprites.Length - 2);
                }
                else if (difficultyLevel == 2)
                {
                    newCellType = UnityEngine.Random.Range(0, cellSprites.Length - 1);
                }
                else if (difficultyLevel >= 3)
                {
                    newCellType = UnityEngine.Random.Range(0, cellSprites.Length);
                }
                
                cell.GetComponent<GridCell>().SetCellType((cellType)newCellType, cellSprites[newCellType]);
                allCells.Add(cell); 
                rowCells.Add(cell);
            }
            grid.Add(rowCells);
        }
        
    }

    public void CheckSelectedCells()
    {


    }

}
