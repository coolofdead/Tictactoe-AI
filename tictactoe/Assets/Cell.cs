using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public int row, column;

    public void OnPointerClick(PointerEventData eventData)
    {
        TicTacToe.Session.PlacePiece(row, column);
    }
}
