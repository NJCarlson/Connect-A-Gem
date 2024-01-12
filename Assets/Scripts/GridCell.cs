using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    [SerializeField]
    public cellType cellType;

    [SerializeField]
    public Image cellImage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCellType(cellType newCellType, Sprite cellSprite)
    {
        cellType = newCellType;
        cellImage.sprite = cellSprite;

    }

}
