using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("References")]
    public Material material_StrokeChessBoard;
    public Material material_Pawn;
    public Material material_Player;
    public Material material_Caro1;
    public Material material_Caro2;
    public Material material_Wall;

  
    public List<SetColor> listSetColor = new List<SetColor>();
    private SetColor currentSetColor;
    private int currentNumber;

    [System.Serializable]
    public class SetColor
    {
        public Color strokeChessBoard;
        public Color pawn;
        public Color player;
        public Color caro1;
        public Color caro2;
        public Color obstacle;
    }

    private void Start()
    {
        RandomCurrentNumber();
        SetColorFromData();
    }

    private void RandomCurrentNumber()
    {
        for (int i = 0; i < 1; i++)
        {
            int r = Random.Range(0, listSetColor.Count);

            if(r == currentNumber)
            {
                i--;
            }
            else
            {
                currentNumber = r;
            }
        }

        currentSetColor = listSetColor[currentNumber];
    }

    private void NextCurrentNumber()
    {
        currentNumber++;

        if (currentNumber >= listSetColor.Count)
        {
            currentNumber = 0;
        }

        currentSetColor = listSetColor[currentNumber];
    }

    private void SetColorFromData()
    {
        material_StrokeChessBoard.color = currentSetColor.strokeChessBoard;
        material_Pawn.color = currentSetColor.pawn;
        material_Player.color = currentSetColor.player;
        // material_Caro1.color = currentSetColor.caro1;
        // material_Caro2.color = currentSetColor.caro2;
        material_Wall.color = currentSetColor.obstacle;
    }

    public void ChangeColor()
    {
        NextCurrentNumber();
        SetColorFromData();
    }
}
