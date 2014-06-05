using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NetworkedCheckersClient;

namespace TestBoard
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();
            Console.WriteLine("testboard valid/invalid move");
            //wrong move p1
            Console.WriteLine("wrong p1 move (Answer should be false)");
            Console.WriteLine(board.IsValidMovePlayer1("21,41"));
            Console.WriteLine(board.IsValidMovePlayer1("21,32"));
            Console.WriteLine(board.IsValidMovePlayer1("21,22"));
            Console.WriteLine(board.IsValidMovePlayer1("21,31"));
            Console.WriteLine(board.IsValidMovePlayer1("21,11"));
            Console.WriteLine(board.IsValidMovePlayer1("21,12"));
            Console.WriteLine(board.IsValidMovePlayer1("21,42"));
            //p1 wrong origin
            Console.WriteLine("wrong p1 pick (answer should be false)");
            Console.WriteLine(board.IsValidMovePlayer1("61,52"));
            Console.WriteLine(board.IsValidMovePlayer1("63,54"));
            //wrong move p2
            Console.WriteLine("wrong p2 move (Answer should be false)");
            Console.WriteLine(board.IsValidMovePlayer2("61,51"));
            Console.WriteLine(board.IsValidMovePlayer2("61,62"));
            Console.WriteLine(board.IsValidMovePlayer2("61,72"));
            Console.WriteLine(board.IsValidMovePlayer2("61,71"));
            Console.WriteLine(board.IsValidMovePlayer2("63,53"));
            Console.WriteLine(board.IsValidMovePlayer2("63,53"));
            Console.WriteLine(board.IsValidMovePlayer2("63,62"));
            Console.WriteLine(board.IsValidMovePlayer2("63,64"));
            //p2 wrong origin
            Console.WriteLine("wrong p2 pick (Answer should be false)");
            Console.WriteLine(board.IsValidMovePlayer2("32,41"));
            Console.WriteLine(board.IsValidMovePlayer2("32,43"));
            //right move p1 
            Console.WriteLine("correct p1 move (Answer should be true)");
            Console.WriteLine(board.IsValidMovePlayer1("32,41"));
            Console.WriteLine(board.IsValidMovePlayer1("32,43"));
            Console.WriteLine(board.IsValidMovePlayer1("36,47"));
            Console.WriteLine(board.IsValidMovePlayer1("36,45"));
            //right move p2
            Console.WriteLine("correct p2 move (Answer should be true)");
            Console.WriteLine(board.IsValidMovePlayer2("61,52"));
            Console.WriteLine(board.IsValidMovePlayer2("63,52"));
            Console.WriteLine(board.IsValidMovePlayer2("63,54"));
            Console.WriteLine("");

            Console.WriteLine("testboard get pieces");
            Console.WriteLine(board.GetPieces());
            Console.WriteLine("");

            Console.WriteLine("testboard move");
            board.Move("32", "41", Piece.PLAYER1);
            board.Move("21", "32", Piece.PLAYER1);
            board.Move("12", "21", Piece.PLAYER1);
            board.Move("61", "52", Piece.PLAYER2);
            board.Move("72", "61", Piece.PLAYER2);
            board.Move("81", "72", Piece.PLAYER2);
            Console.WriteLine(board.GetPieces());
            //try moving backward
            Console.WriteLine(board.IsValidMovePlayer2("72,81"));
            Console.WriteLine(board.IsValidMovePlayer1("21,12"));

            Console.ReadKey();
        }
    }
}
