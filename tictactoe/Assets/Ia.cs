using System;

[Serializable]
public class Ia
{
    private TicTacToe game;
    private TicTacToe.CellState iaTurn;

    public Ia(TicTacToe game, TicTacToe.CellState iaTurn) 
    {
        this.game = game;
        this.iaTurn = iaTurn;
    }

    public int FindNextMove()
    {
        return FindBestMoveRecursively(game.Board, iaTurn, true);
    }

    private int FindBestMoveRecursively(TicTacToe.CellState[] board, TicTacToe.CellState playerTurn, bool returnMove = false, int floor = 0)
    {
        // check if winner
        // return -1 | 1 => loose | win
        if (game.HasWinner(board, playerTurn)) {
            return  playerTurn == iaTurn ? 1 : -1;
        }

        int move = -1; // no move
        int score = -2; // min score is -1 (when loosing) so lowest score should be -2 meaning no score

        // recu for each empty cell as playerTurn
        for (int i = 0; i < board.Length; i++) {
            if (board[i] != TicTacToe.CellState.Empty) {
                continue;
            }

            var boardWithNewMove = (TicTacToe.CellState[])board.Clone();

            var nextTurn = playerTurn == TicTacToe.CellState.Circle ? TicTacToe.CellState.Cross : TicTacToe.CellState.Circle;

            if (playerTurn == iaTurn)
            {
                boardWithNewMove[i] = nextTurn;
                if (game.HasWinner(board, nextTurn))
                {
                    return -99 + floor;
                }
            }

            boardWithNewMove[i] = playerTurn;

            int newScore = -FindBestMoveRecursively(boardWithNewMove, nextTurn, false, floor++);

            if (newScore > score) {
                score = newScore;
                move = i;
            }
        }

        // return 0 if draw
        if (move == -1)
        {
            return 0;
        }

        if (returnMove)
            return move;

        // Should return move and not score
        return score;
    }
}
