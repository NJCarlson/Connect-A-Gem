using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    [SerializeField]
    public cellType cellType;

    [SerializeField]
    public Image cellSelectedImage;

    [SerializeField]
    public Image cellImage;

    [SerializeField]
    public int row;
    public int col;

    public bool selected;

    GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = GameObject.FindObjectOfType<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            cellSelectedImage.enabled = true;
        }
        else
        {
            cellSelectedImage.enabled = false;
        }
    }

    public void SetCellType(cellType newCellType, Sprite cellSprite)
    {
        cellType = newCellType;
        cellImage.sprite = cellSprite;

    }

    public void CellClicked()
    {
        if (!selected)
        {
            gridManager.SelectCell(this);
        }
    }

}
