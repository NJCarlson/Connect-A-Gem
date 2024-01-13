using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoardItem : MonoBehaviour
{

    public string m_name;
    public string date;
    public string score;

    [SerializeField]
    public TMP_Text scoreText;

    [SerializeField]
    public TMP_Text nameText;

    [SerializeField]
    public TMP_Text dateText;

    // Start is called before the first frame update
    void Start()
    {       
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score;
        nameText.text = m_name;
        dateText.text = date;
    }
}
