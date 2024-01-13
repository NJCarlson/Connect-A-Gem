using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum cellType { blue = 0, green = 1, purple = 2, yellow = 3, red = 4 };

public class GridManager : MonoBehaviour
{
    [SerializeField]
    int rowSize = 6;
    [SerializeField]
    int colSize = 6;

    [SerializeField]
    float cellSize = 1.0f; // Set the size of each newCell

    [SerializeField]
    public Sprite[] cellSprites;

    [SerializeField]
    public GameObject gridCellPrefab;

    [SerializeField]
    public TMP_Text timerText;

    [SerializeField]
    public TMP_Text scoreText;

    [SerializeField]
    public TMP_Text levelText;

    [SerializeField]
    public TMP_Text goalText;

    [SerializeField]
    public GameObject startButton;

    public int curDifficultyLevel = 1;
    private List<List<GameObject>> grid = new List<List<GameObject>>(); //2D list of rows of cells
    private List<GameObject> allCells = new List<GameObject>(); //list of all newCell game objects, used for cleanup purposes

    public bool timerRunning = false;
    public float timeLimit = 60.0f;
    private float timer = 0;
    private int playerScore = 0;
    private int LevelGoal = 10;

    public List<GridCell> selectedCells = new List<GridCell>();

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

         
            //update UI text
            //timerText.text = timer.ToString();
            scoreText.text = playerScore.ToString();

            //Check for player connected Cells
            CheckSelectedCells();

            if (!GridHasConnections())
            {
                Debug.Log("no more connections!");
                ClearCells();
                GenerateGrid(curDifficultyLevel);
            }

            if (playerScore > LevelGoal)
            {
                curDifficultyLevel++;
                LevelGoal = curDifficultyLevel * 10;
                Debug.Log("Level Up!");

            }

        }
        //check for time limit
        if (timer >= timeLimit)
        {
            //game over!
            Debug.Log("Game Over!");
            timerRunning = false;
            timer = 0;

            if (curDifficultyLevel >= 3)
            {
                //Player won!

                //show restart button?
                //or allow user to keep going?

            }
            else
            {
                //player lost...
                //show restart button

            }

            //save to scoreboard here

            startButton.SetActive(true);

            //reset
            ClearCells();
            curDifficultyLevel = 1;
            LevelGoal = curDifficultyLevel * 10;


        }

    }

    public void ResetAndGenerateGrid()
    { 
        startButton.SetActive(false);

        foreach (var cell in allCells)
        {
            Destroy(cell.gameObject);
        }

        grid.Clear();
        allCells.Clear();
        selectedCells.Clear();

        playerScore = 0;

        LevelGoal = curDifficultyLevel * 10;

        GenerateGrid(curDifficultyLevel);
        timer = 0;
        timerRunning = true;
    }

    public void ClearCells()
    {
        foreach (var cell in allCells)
        {
            Destroy(cell.gameObject);
        }

        grid.Clear();
        allCells.Clear();
        selectedCells.Clear();

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
                GameObject cell = Instantiate(gridCellPrefab, position, Quaternion.identity, this.transform);

                //randomize newCell type based on difficulty
                int newCellType = 0;

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
                    if (!AreAllCellsAdjacent(selectedCells))
                    {
                        //cells aren't adjacent
                        ClearSelectedCells();

                        //todo play womp womp sound here

                        return;
                    }
                }
                else
                {
                    //not all selected cells are of the same type!
                    ClearSelectedCells();

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
                    if (!selectedCells.Contains(cell)) //check that this newCell isn't selected
                    {
                        if ((int)cell.cellType == selectedCellType) //check if this is the selected type
                        {
                            if (CheckForAdjacency(cell, selectedCells))
                            {
                                allPossibleConnectionsMade = false;
                                break;
                            }
                        }
                    }
                }
            }

            if (allPossibleConnectionsMade)
            {
                //if there are no more possible connections. Destroy selected cells and give player points & play good sound
                playerScore += selectedCells.Count;
                Debug.Log("Player Score : " + playerScore);

                foreach (var cell in selectedCells)
                {
                    // grid[cell.row].RemoveAt(cell.col);
                    // allCells.Remove(cell.gameObject);
                    cell.gameObject.SetActive(false);
                }

                selectedCells.Clear();

                //todo play score sound

            }
            else
            {
                //if there are more possible connections, do nothing - keep selected cells
                //todo play a hint particle/sound? 
            }


        }

    }

    private bool IsValidCell(int row, int col)
    {
        return row >= 0 && row < grid.Count && col >= 0 && col < grid[row].Count && grid[row][col].activeSelf;
    }

    //checks if two cells are adjacent
    static bool CheckForAdjacency(GridCell cell1, GridCell cell2)
    {
        int rowDiff = Math.Abs(cell1.row - cell2.row);
        int colDiff = Math.Abs(cell1.col - cell2.col);
        return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
    }

    //checks if one cell is adjacent to any in a given list.
    static bool CheckForAdjacency(GridCell checkCell, List<GridCell> cellList)
    {
        bool valid = false;

        foreach (var curCell in cellList)
        {
            if (CheckForAdjacency(checkCell, curCell))
            {
                valid = true;
                break;
            }
        }
        return valid;
    }

    //checks if all cells in a given list are adjacent to each other
    static bool AreAllCellsAdjacent(List<GridCell> cells)
    {
        for (int i = 0; i < cells.Count - 1; i++)
        {
            GridCell curCell = cells[i];


            if (!CheckForAdjacency(curCell, cells))
            {
                return false;
            }
        }

        return true;
    }

    //de-select the highlighted cells
    private void ClearSelectedCells()
    {
        foreach (var cell in selectedCells)
        {
            //reset visuals
            cell.selected = false;
        }

        selectedCells.Clear();
    }

    private bool GridHasConnections()
    {
        for (int row = 0; row < grid.Count; row++)
        {
            for (int col = 0; col < grid[row].Count; col++)
            {
                int type = (int)grid[row][col].GetComponent<GridCell>().cellType;


                List<GridCell> connectedCells = GetConnectedCells(row, col, type);

                if (connectedCells.Count >= 3)
                {
                    return true;
                }

            }
        }
        return false;
    }

    List<GridCell> GetConnectedCells(int row, int col, int cellType)
    {
        List<GridCell> connectedCells = new List<GridCell>();
        bool[,] visited = new bool[grid.Count, grid[0].Count];

        DepthFirstSearch(row, col, cellType, visited, connectedCells);

        return connectedCells;
    }

    void DepthFirstSearch(int row, int col, int cellType, bool[,] visited, List<GridCell> connectedCells)
    {
        if (!IsValidCell(row, col) || row < 0 || row >= grid.Count || col < 0 || col >= grid[0].Count || visited[row, col] || (int)grid[row][col].GetComponent<GridCell>().cellType != cellType)
        {
            return;
        }

        visited[row, col] = true;
        connectedCells.Add(grid[row][col].GetComponent<GridCell>());

        // Check neighbors
        DepthFirstSearch(row - 1, col, cellType, visited, connectedCells); // Up
        DepthFirstSearch(row + 1, col, cellType, visited, connectedCells); // Down
        DepthFirstSearch(row, col - 1, cellType, visited, connectedCells); // Left
        DepthFirstSearch(row, col + 1, cellType, visited, connectedCells); // Right
    }

    //called when a cell is clicked
    public void SelectCell(GridCell newCell)
    {
        if (!timerRunning)
        {
            return;
        }

        if (selectedCells.Count <= 0)
        {
            grid[newCell.row][newCell.col].GetComponent<GridCell>().selected = true;
            selectedCells.Add(newCell);
        }
        else
        {
            bool valid = false;

            valid = CheckForAdjacency(newCell, selectedCells);

            if (valid)
            {
                grid[newCell.row][newCell.col].GetComponent<GridCell>().selected = true;
                selectedCells.Add(newCell);
            }
            else
            {
                ClearSelectedCells();
            }

        }

    }

}
