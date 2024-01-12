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

    public int curDifficultyLevel = 1;
    private List<List<GameObject>>  grid = new List<List<GameObject>>(); //2D list of rows of cells
    private List<GameObject> allCells = new List<GameObject>(); //list of all cell game objects, used for cleanup purposes

    public bool timerRunning = false;
    public float timeLimit = 60.0f;
    private float timer = 0;
    private int playerScore = 0;
    private int LevelGoal = 10;

    private List<GridCell> selectedCells = new List<GridCell>();

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

                    if (curDifficultyLevel > 3)
                    {
                        //show restart button?
                        //or allow user to keep going?
                    }
                    else
                    {
                        //show next level button
                    }

                    curDifficultyLevel++;

                }
                else
                {
                    //player lost...
                    //show restart button
                    curDifficultyLevel = 1;
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
                cell.GetComponent<GridCell>().row = row;
                cell.GetComponent<GridCell>().col = col;
                cell.GetComponent<GridCell>().SetCellType((cellType)newCellType, cellSprites[newCellType]);
                allCells.Add(cell); 
                rowCells.Add(cell);
            }
            grid.Add(rowCells);
        }

    }

    /// <summary>
    /// Check if all player selected cells are the same type.
    /// 
    /// </summary>
    public void CheckSelectedCells()
    {
        int selectedCellType = -1;
        GridCell prevCell = null;

        foreach (var selectedCell in selectedCells)
        {
            if (selectedCellType == -1)
            {
                selectedCellType = (int)selectedCell.cellType;
            }
            else
            {
                if ((int)selectedCell.cellType == selectedCellType)
                {
                    if (!CheckForAdjacency(prevCell, selectedCell))
                    {
                        //cells aren't adjacent
                        selectedCells.Clear();

                        //todo play womp womp sound here

                        return;
                    } 
                }
                else 
                { 
                    //not all selected cells are of the same type!
                    selectedCells.Clear();

                    //todo play womp womp sound here

                    return;
                }
            }

            prevCell = selectedCell;
        }

        //if we've gotten this far, all cells are of the same type & connected. 
        // Now check if there are any other possible connections that can be made.
        bool allPossibleConnectionsMade = true;

        if (selectedCells.Count >= 3)
        {
            foreach (var cellObj in allCells)
            {
                GridCell cell = cellObj.GetComponent<GridCell>();
                if (cell != null) 
                {
                    if (!selectedCells.Contains(cell)) //check that this cell isn't selected
                    {
                        if ((int)cell.cellType == selectedCellType) //check if this is the selected type
                        {                          
                            foreach (var selectedCell in selectedCells)//check if the current cell is adjacent to any player selected cell
                            {
                                if (CheckForAdjacency(cell, selectedCell)) 
                                {
                                    allPossibleConnectionsMade = false;
                                } 
                            }

                        }
                    }
                }
            }

            if (allPossibleConnectionsMade)
            {
                //if there are no more possible connections. Destroy selected cells and give player points & play good sound
                playerScore += selectedCells.Count;

                foreach (var cell in selectedCells)
                {
                    Destroy(cell.gameObject);
                }

                selectedCells.Clear();
                
                //todo play score sound

            }
            else
            {  
                //if there are more possible connections, do nothing - keep selected cells

            }
         

        }

    }

    static bool CheckForAdjacency(GridCell cell1, GridCell cell2)
    {
        int rowDiff = Math.Abs(cell1.row - cell2.row);
        int colDiff = Math.Abs(cell1.col - cell2.col);

        return rowDiff <= 1 && colDiff <= 1;
    }

}
