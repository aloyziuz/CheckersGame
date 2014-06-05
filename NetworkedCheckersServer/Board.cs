using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetworkedCheckersServer
{
    public class Board
    {
        public Cell[,] cellsArray;

        public Board()
        {
            int cellID = 1;
            cellsArray = new Cell[8, 8];
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    cellsArray[r, c] = new Cell
                    {
                        row = r + 1,
                        col = c + 1,
                        ID = cellID

                    };
                    cellID++;
                }
            }
            resetBoard();
        }

        //set pieces in initial position
        public void resetBoard()
        {
            foreach (var cell in cellsArray)
            {
                cell.piece = Piece.EMPTY;
            }
            for (int r = 0; r < 8; r += 2)
            {
                for (int c = 0; c < 8; c += 2)
                {
                    cellsArray[r, c].piece = Piece.UNAVAILABLE;
                }
            }
            for (int r = 1; r < 8; r += 2)
            {
                for (int c = 1; c < 8; c += 2)
                {
                    cellsArray[r, c].piece = Piece.UNAVAILABLE;
                }
            }
            for (int r = 0; r < 3; r += 2)
            {
                for (int c = 1; c < 8; c += 2)
                {
                    cellsArray[r, c].piece = Piece.PLAYER1;
                }
            }
            for (int r = 1; r < 2; r += 2)
            {
                for (int c = 0; c < 8; c += 2)
                {
                    cellsArray[r, c].piece = Piece.PLAYER1;
                }
            }
            for (int r = 5; r < 8; r += 2)
            {
                for (int c = 0; c < 8; c += 2)
                {
                    cellsArray[r, c].piece = Piece.PLAYER2;
                }
            }
            for (int r = 6; r < 7; r += 2)
            {
                for (int c = 1; c < 8; c += 2)
                {
                    cellsArray[r, c].piece = Piece.PLAYER2;
                }
            }
        }

        //check whether move is valid for player 1
        public bool IsValidMovePlayer1(string move)
        {
            if (Regex.Match(move, @"^[1-8][1-8],[1-8][1-8]$").Success)
            {
                int originRow = int.Parse(move.Substring(0, 1)) - 1;
                int originCol = int.Parse(move.Substring(1, 1)) - 1;
                int destRow = int.Parse(move.Substring(3, 1)) - 1;
                int destCol = int.Parse(move.Substring(4, 1)) - 1;
                if (cellsArray[destRow, destCol].piece == Piece.EMPTY && cellsArray[originRow, originCol].piece == Piece.PLAYER1)
                {
                    //if piece moving
                    if ((destRow - originRow) == 1)
                    {
                        if (destCol - originCol == 1 || destCol - originCol == -1)
                        {
                            return true;
                        }
                        return false;
                    }
                    //if taking opp piece
                    else if ((destRow - originRow == 2))
                    {
                        //check whether the move is diagonal
                        if (destCol - originCol == 2 || destCol - originCol == -2)
                        {
                            //check whether the piece in the middle of the diagonal is opp piece
                            int oppPieceRow = (destRow + originRow) / 2;
                            int oppPieceCol = (destCol + originCol) / 2;
                            if (cellsArray[oppPieceRow, oppPieceCol].piece == Piece.PLAYER2)
                            {
                                return true;
                            }
                            return false;
                        }
                        return false;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        //check whether move is valid for player 2
        public bool IsValidMovePlayer2(string move)
        {
            if (Regex.Match(move, @"^[1-8][1-8],[1-8][1-8]$").Success)
            {
                int originRow = int.Parse(move.Substring(0, 1)) - 1;
                int originCol = int.Parse(move.Substring(1, 1)) - 1;
                int destRow = int.Parse(move.Substring(3, 1)) - 1;
                int destCol = int.Parse(move.Substring(4, 1)) - 1;
                if (cellsArray[destRow, destCol].piece == Piece.EMPTY && cellsArray[originRow, originCol].piece == Piece.PLAYER2)
                {
                    //usual moving piece
                    if ((originRow - destRow) == 1)
                    {
                        //check whether the move is diagonal
                        if (destCol - originCol == 1 || destCol - originCol == -1)
                        {
                            return true;
                        }
                        return false;
                    }
                    //indicates 'taking' move
                    else if ((originRow - destRow) == 2)
                    {
                        //check whether the move is diagonal
                        if (destCol - originCol == 2 || destCol - originCol == -2)
                        {
                            //check whether there is opp piece in the middle of the diagonal
                            int oppPieceRow = (destRow + originRow) / 2;
                            int oppPieceCol = (destCol + originCol) / 2;
                            if (cellsArray[oppPieceRow, oppPieceCol].piece == Piece.PLAYER1)
                            {
                                return true;
                            }
                            return false;
                        }
                        return false;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

        //move piece asuming the move is valid
        public void Move(string origin, string destination, Piece piece)
        {
            //origin cell becomes empty
            int oriRow = int.Parse(origin.Substring(0, 1)) - 1;
            int oriCol = int.Parse(origin.Substring(1, 1)) - 1;
            cellsArray[oriRow, oriCol].piece = Piece.EMPTY;

            //desetination cell becomes occupied
            int destRow = int.Parse(destination.Substring(0, 1)) - 1;
            int destCol = int.Parse(destination.Substring(1, 1)) - 1;
            cellsArray[destRow, destCol].piece = piece;

            //additional step to remove opp piece in 'taking' move
            float oppPieceRow = (oriRow + destRow) / 2.0f;
            float oppPieceCol = (oriCol + destCol) / 2.0f;
            //if decimal, it is not 'taking' move
            //if whole number, it is 'taking' move
            if (oppPieceRow % 1 == 0 && oppPieceCol % 1 == 0)
            {
                cellsArray[(int)oppPieceRow, (int)oppPieceCol].piece = Piece.EMPTY;
            }
        }

        //checks whether p1 wins
        public bool isP1Win()
        {
            for (int c = 0; c < 8; c += 2)
            {
                //just check the last row for p1 piece
                if (cellsArray[7, c].piece == Piece.PLAYER1)
                {
                    return true;
                }
            }
            return false;
        }

        //checks whether p2 wins
        public bool isP2Win()
        {
            for (int c = 1; c < 8; c += 2)
            {
                //just check the first row for p2 piece
                if (cellsArray[0, c].piece == Piece.PLAYER2)
                {
                    return true;
                }
            }
            return false;
        }

        //returns pieces notations
        public string GetPieces()
        {
            var response = new StringBuilder();
            List<Cell> p1Cells = new List<Cell>();
            List<Cell> p2Cells = new List<Cell>();

            foreach (var cell in cellsArray)
            {
                if (cell.piece == Piece.PLAYER1)
                {
                    p1Cells.Add(cell);
                }
                else if (cell.piece == Piece.PLAYER2)
                {
                    p2Cells.Add(cell);
                }
            }

            foreach (var cell in p1Cells)
            {
                response.Append("N" + cell.row + cell.col);
            }

            response.Append("|");
            
            foreach (var cell in p2Cells)
            {
                response.Append("N" + cell.row + cell.col);
            }
            return response.ToString();
        }
    }

}
