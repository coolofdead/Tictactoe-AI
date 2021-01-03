using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    private const float DELAY_BETWEEN_IA_MOVE = 1f;
    private const CellState FIRST_PLAYER_TURN = CellState.Cross;

    public static TicTacToe Session;

    public enum CellState {Empty = 99, Cross = 1, Circle = 2}

    public bool useInterface = true;
    public bool userAgainstIa = true;
    public bool iaVersusIa = false;
    public bool iaGoesSecond = true;

    private Ia ia;
    private Ia secondIa;
    public CellState[] Board { get; private set; }
    private Text[] boardInterface;

    // 1 = cross, 2 = circle
    // cross always play first
    private CellState playerTurn;

    [SerializeField]
    private Text playerTurnText;

    void Awake()
    {
        Session = this;
        boardInterface = GetComponentsInChildren<Text>();

        ia = new Ia(this, GetIaTurnState());
        secondIa = new Ia(this, GetIaTurnState(false));
    }

    void Start()
    {
        ResetGame();
    }

    private IEnumerator IaVersusIa()
    {
        while (!HasWinner())
        {
            PlacePiece(ia.FindNextMove());

            yield return new WaitForSeconds(DELAY_BETWEEN_IA_MOVE);

            PlacePiece(secondIa.FindNextMove());

            yield return new WaitForSeconds(DELAY_BETWEEN_IA_MOVE);
        }
    }

    private CellState GetIaTurnState (bool isFirstIa = true)
    {
        return isFirstIa && iaGoesSecond ? CellState.Circle : CellState.Cross;
    }

    private int ConvertRowColumnToArrayIndex(int row, int column) 
    {
        return ((row-1) * 3 + column) - 1;
    }

    public void PlacePiece(int row, int column)
    {
        PlacePiece(ConvertRowColumnToArrayIndex(row, column));
    }

    private void PlacePiece(int pieceIndex)
    {
        // check if cell is empty at row and column on board else return
        if (Board[pieceIndex] != CellState.Empty) {
            Debug.Log("Cannot play here " + pieceIndex);
            return;
        }

        // place piece on board
        Board[pieceIndex] = playerTurn;

        // update interface
        if (useInterface)
            UpdateBoardInterface();

        // check victory or draw
        if (HasWinner()) {
            Debug.Log("Player " + playerTurn + " has win!");
            return;
        }

        // change player turn
        ChangePlayerTurn();

        if (userAgainstIa && playerTurn == GetIaTurnState()) {
            int iaMove = ia.FindNextMove();

            PlacePiece(iaMove);
        }
    }

    public CellState GetWinner()
    {
        return HasWinner() ? playerTurn : CellState.Empty;
    }

    public void UpdateInterface()
    {
        UpdateBoardInterface();
        UpdatePlayerTurnText();
    }

    private void UpdatePlayerTurnText()
    {
        playerTurnText.text = "player turn " + (playerTurn == CellState.Cross ? "x" : "o");
    }

    private void UpdateBoardInterface()
    {
        for (int i = 0; i < Board.Length; i++) {
            var piece = "";
            switch (Board[i])
            {
                case CellState.Cross:
                    piece = "x";
                    break;
                case CellState.Circle:
                    piece = "o";
                    break;
            }
            
            boardInterface[i].text = piece;
        }
    }

    private void ChangePlayerTurn() 
    {
        playerTurn = playerTurn == CellState.Cross ? CellState.Circle : CellState.Cross;
    }

    // Check if a player has win or if it's a draw
    public bool HasWinner(CellState[] board = null, CellState playerTurn = CellState.Empty)
    {
        board = board == null ? this.Board : board;
        playerTurn = playerTurn == CellState.Empty ? this.playerTurn : playerTurn;

        bool hasLeftDiagonal = ((float)playerTurn == ((float)board[0] + (float)board[4] + (float)board[8]) / 3);  // left diagonal
        bool hasRightDiagonal = (float)playerTurn == (((float)board[2] + (float)board[4] + (float)board[6]) / 3); // right diagonal

        bool hasWin = hasLeftDiagonal || hasRightDiagonal;

        for (var i = 0; i < 3; i++)
        {
            bool hasLine = (float)playerTurn == ((float)board[i*3] + (float)board[i*3+1] + (float)board[i*3+2]) / 3;
            bool hasColumn = (float)playerTurn == ((float)board[i] + (float)board[i+3] + (float)board[i+6]) / 3;

            hasWin = hasWin || hasLine || hasColumn;
        }

        return hasWin;
    }

    public void ResetGame()
    {
        playerTurn = FIRST_PLAYER_TURN;
        Board = Enumerable.Repeat(CellState.Empty, 9).ToArray();

        // USED TO MAKE DEBUG BOARDS
        //board = new TicTacToe.CellState[] {
        //    TicTacToe.CellState.Empty, TicTacToe.CellState.Empty, TicTacToe.CellState.Empty,
        //    TicTacToe.CellState.Empty, TicTacToe.CellState.Empty, TicTacToe.CellState.Empty,
        //    TicTacToe.CellState.Empty, TicTacToe.CellState.Empty, TicTacToe.CellState.Empty
        //};

        if (useInterface)
            UpdateInterface();

        if (iaVersusIa && !userAgainstIa)
            StartCoroutine(IaVersusIa());
    }
}
