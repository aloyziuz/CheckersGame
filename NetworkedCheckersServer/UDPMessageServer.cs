using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NetworkedCheckersServer
{
    class UDPMessageServer : MessageServer
    {
        Board board;
        GameState gamestate;
        UdpClient udpServer;
        int networkPort, multicastPort, gameMoveCount;
        string p1Name, p2Name, multicastAddress;
        internal ObservableCollection<string> messages, leaderboard;

        public UDPMessageServer()
        {
            board = new Board();
            gamestate = GameState.WAIT_FOR_GAME_START;
            messages = new ObservableCollection<string>();
            leaderboard = new ObservableCollection<string>();

            showLeaderBoard();

            Record("Current Game State: " + gamestate.ToString());
        }

        public void connectToNetwork(string address, int port)
        {
            udpServer = new UdpClient(port);
        }

        public void connectToMulticast(string multicastAddress, int multicastPort)
        {
            this.multicastAddress = multicastAddress;
            this.multicastPort = multicastPort;
        }

        public void HandleRequests()
        {
            var endPoint = new IPEndPoint(IPAddress.Any, networkPort);
            while (true)
            {
                var requestBytes = udpServer.Receive(ref endPoint);
                var request = Encoding.UTF8.GetString(requestBytes);
                var response = "ERROR";

                //process the request
                //if login request, check username and password
                if (Regex.Match(request, @"^LOGIN,[A-Za-z]+,[A-Za-z0-9]+$").Success)
                {
                    string[] requestArray = request.Split(',');
                    using (var context = new NetworkedCheckersContainer())
                    {
                        var query = from user in context.Users select user;
                        foreach (var user in query)
                        {
                            if (requestArray[1].Equals(user.UserName) && requestArray[2].Equals(user.Password) && user.LoginStatus == false)
                            {
                                user.LoginStatus = true;
                                //create string of online users
                                string allUser = "";
                                foreach (var userObj in query)
                                {
                                    if (userObj.LoginStatus == true)
                                    {
                                        allUser += userObj.UserName;
                                        allUser += ":";
                                    }
                                }
                                allUser = allUser.Remove(allUser.Length - 1);

                                BroadcastMessages("USERS," + allUser);
                                response = "OKAY";
                                break;
                            }
                        }
                        context.SaveChanges();
                    }
                }
                //if logoff request
                else if (Regex.Match(request, @"^LOGOUT,[A-Za-z]+$").Success)
                {
                    string[] requestArray = request.Split(',');
                    using (var context = new NetworkedCheckersContainer())
                    {
                        var query = from user in context.Users select user;
                        foreach (var user in query)
                        {
                            //change the login status of the designated user to false
                            if (user.UserName == requestArray[1] && user.LoginStatus == true)
                            {
                                user.LoginStatus = false;
                                break;
                            }
                        }
                        context.SaveChanges();

                        //create string of online users
                        string allUser = "";
                        foreach (var user in query)
                        {
                            if (user.LoginStatus == true)
                            {
                                allUser += user.UserName;
                                allUser += ":";
                            }
                        }
                        if (allUser != "")
                        {
                            allUser = allUser.Remove(allUser.Length - 1);
                        }

                        BroadcastMessages("USERS," + allUser);
                        response = "OKAY";
                    }
                }
                else
                {
                    switch (gamestate)
                    {
                        case GameState.WAIT_FOR_GAME_START:
                            if (Regex.Match(request, @"^PLAY,[A-Za-z]+,[A-Za-z]+$").Success)
                            {
                                string[] requestArray = request.Split(',');
                                p1Name = requestArray[1];
                                p2Name = requestArray[2];
                                if (p1Name != p2Name)
                                {
                                    //set move count to 0
                                    gameMoveCount = 0;
                                    //set board to initial position
                                    board.resetBoard();
                                    gamestate = GameState.Player1_MOVING;

                                    BroadcastMessages("GAMESTATE," + board.GetPieces());
                                    BroadcastMessages("GAMESTART," + p1Name + "," + p2Name);
                                    BroadcastMessages("STATUS," + p1Name + ",MOVING");
                                    BroadcastMessages("STATUS," + p2Name + ",WAITING");
                                    response = "OKAY";
                                }
                            }
                            break;

                        case GameState.Player1_MOVING:
                            if (Regex.Match(request, @"^TRY,[A-Za-z]+,[1-8][1-8],[1-8][1-8]$").Success)
                            {
                                string[] requestArray = request.Split(',');
                                //check whether this is the player's turn to move
                                if (requestArray[1].Equals(p1Name))
                                {
                                    //check whether the move is valid
                                    if (board.IsValidMovePlayer1(requestArray[2] + "," + requestArray[3]))
                                    {
                                        //move the piece
                                        board.Move(requestArray[2], requestArray[3], Piece.PLAYER1);
                                        gameMoveCount++;

                                        BroadcastMessages("GAMESTATE," + board.GetPieces());
                                        BroadcastMessages("STATUS," + p2Name + ",MOVING");
                                        BroadcastMessages("STATUS," + p1Name + ",WAITING");
                                        response = "DONE";

                                        //check whether the player win after the move
                                        if (board.isP1Win())
                                        {
                                            gamestate = GameState.GAME_OVER;
                                            Record("Game Over. Player 1 Wins");

                                            BroadcastMessages("STATUS," + p1Name + ",WON");
                                            BroadcastMessages("STATUS," + p2Name + ",LOST");

                                            using (var context = new NetworkedCheckersContainer())
                                            {
                                                //identify the winning user from database
                                                var p1User = from user in context.Users where user.UserName.Equals(p1Name) select user;
                                                //add new highscore to Highscores table
                                                var highscore = new Highscore()
                                                {
                                                    DateTime = DateTime.Now.ToString(),
                                                    MoveCount = (Int16) gameMoveCount,
                                                    User = p1User.First()
                                                };
                                                context.Highscores.Add(highscore);
                                                context.SaveChanges();
                                            }
                                            showLeaderBoard();
                                        }
                                        else
                                        {
                                            gamestate = GameState.Player2_MOVING;
                                        }
                                    }
                                }
                            }
                            break;

                        case GameState.Player2_MOVING:
                            if (Regex.Match(request, @"^TRY,[A-Za-z]+,[1-8][1-8],[1-8][1-8]$").Success)
                            {
                                //check whether this is the player's turn to move
                                string[] requestArray = request.Split(',');
                                if (requestArray[1].Equals(p2Name))
                                {
                                    //check whether the move is valid
                                    if (board.IsValidMovePlayer2(requestArray[2] + "," + requestArray[3]))
                                    {
                                        //move the piece
                                        board.Move(requestArray[2], requestArray[3], Piece.PLAYER2);
                                        gameMoveCount++;

                                        BroadcastMessages("GAMESTATE," + board.GetPieces());
                                        BroadcastMessages("STATUS," + p1Name + ",MOVING");
                                        BroadcastMessages("STATUS," + p2Name + ",WAITING");
                                        response = "DONE";

                                        //check whether the player win after the move
                                        if (board.isP2Win())
                                        {
                                            gamestate = GameState.GAME_OVER;
                                            Record("Game Over. Player 2 Wins");
                                            BroadcastMessages("STATUS," + p2Name + ",WON");
                                            BroadcastMessages("STATUS," + p1Name + ",LOST");

                                            //record the win into the database
                                            using (var context = new NetworkedCheckersContainer())
                                            {
                                                //identify the winning user from database
                                                var p2User = from user in context.Users where user.UserName.Equals(p2Name) select user;
                                                //add new highscore to Highscores table
                                                var highscore = new Highscore()
                                                {
                                                    DateTime = DateTime.Now.ToString(),
                                                    MoveCount = (Int16)gameMoveCount,
                                                    User = p2User.First()
                                                };
                                                context.Highscores.Add(highscore);
                                                context.SaveChanges();
                                            }
                                            showLeaderBoard();
                                        }
                                        else
                                        {
                                            gamestate = GameState.Player1_MOVING;
                                        }
                                    }
                                }
                            }
                            break;

                        case GameState.GAME_OVER:
                            if (Regex.Match(request, @"^PLAY,[A-Za-z]+,[A-Za-z]+$").Success)
                            {
                                gamestate = GameState.WAIT_FOR_GAME_START;
                                goto case GameState.WAIT_FOR_GAME_START;
                            }
                            break;
                    }
                }

                //send response
                var responseBytes = Encoding.UTF8.GetBytes(response);
                udpServer.Send(responseBytes, responseBytes.Length, endPoint);

                Record("Request: " + request);
                Record("Response: " + response);
                Record("New Game State: " + gamestate.ToString());
                Record("Current Game State: " + gamestate.ToString());
            }
        }

        public void BroadcastMessages(string msg)
        {
            Console.WriteLine(msg);
            var multicastIP = IPAddress.Parse(multicastAddress);
            var udpClient = new UdpClient();
            udpClient.JoinMulticastGroup(multicastIP);
            var endPoint = new IPEndPoint(multicastIP, multicastPort);

            var bytes = Encoding.UTF8.GetBytes(msg);
            udpClient.Send(bytes, bytes.Length, endPoint);

            udpClient.DropMulticastGroup(multicastIP);
            udpClient.Close();
        }

        //add the string to the log listview
        public void Record(string msg)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    string time = DateTime.Now.ToShortTimeString();
                    messages.Insert(0, time + ": " + msg);
                }
            ));
        }

        //updates the leaderboard listview
        public void showLeaderBoard()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    leaderboard.Clear();
                    using (var context = new NetworkedCheckersContainer())
                    {
                        var query = from highscore in context.Highscores orderby highscore.MoveCount ascending select highscore;
                        foreach (var highscore in query)
                        {
                            string highscoreString = "";
                            highscoreString += highscore.User.UserName;
                            highscoreString += " ";
                            highscoreString += highscore.MoveCount;
                            highscoreString += " ";
                            highscoreString += highscore.DateTime;

                            leaderboard.Add(highscoreString);
                        }
                    }
                }
            ));
        }
    }
}
