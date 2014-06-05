using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedCheckersServer
{
    public class Cell
    {
        public int row, col, ID;
        public Piece piece;

        public Cell()
        {
            piece = Piece.EMPTY;
        }
    }
}
