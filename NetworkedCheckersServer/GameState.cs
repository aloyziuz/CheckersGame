using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedCheckersServer
{
    enum GameState
    {
        WAIT_FOR_GAME_START,
        Player1_MOVING,
        Player2_MOVING,
        GAME_OVER
    }
}
