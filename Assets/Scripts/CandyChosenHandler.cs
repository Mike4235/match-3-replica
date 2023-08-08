using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CandyChosenHandler : MonoBehaviour
{
    const int ColumnLength = Board.ColumnLength, RowLength = Board.RowLength;
    int FirstChosen = -1;
    [SerializeField] GameObject ObjectBoard;
    Board CandyBoard;

    void Start()
    {
        CandyBoard = ObjectBoard.GetComponent<Board>();
    }

    public void SquareChoseHandler()
    {
        string SquareChosen = EventSystem.current.currentSelectedGameObject.name;
        int SquareID = int.Parse(SquareChosen);
        Debug.Log(SquareID);
        if (FirstChosen == -1) FirstChosen = SquareID;
        else
        {
            CandyBoard.SwapCandyCaller(FirstChosen, SquareID);
            FirstChosen = -1;
        }
    }


}
