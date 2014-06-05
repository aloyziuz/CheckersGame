using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Media;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace NetworkedCheckersClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow playerWindow;
        private string multicastAddress;
        private int multicastPort;
        private string playerName;
        private UDPMessageHandler messageHandler;
        private string originRow;
        private string originCol;
        private string destRow, destCol;
        private Thread multicastThread;

        public App()
            : base()
        {
            playerWindow = new MainWindow();
            messageHandler = new UDPMessageHandler();

            playerWindow.AddSetNetworkMenuHandler(HandleSetNetworkMenu);
            playerWindow.AddSetMulticastMenuHandler(HandleMulticastMenu);
            playerWindow.AddLoginMenuHandler(HandleLoginMenu);
            playerWindow.AddLogoutMenuHandler(HandleLogoutMenu);
            playerWindow.AddListViewItemHandler(HandleListViewItem);
            playerWindow.GameBoard.AddMouseHandler(HandleMouseEvent);
            //disable gameboard
            playerWindow.GameBoard.IsEnabled = false;

            playerWindow.Show();
        }

        private void HandleListViewItem(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            //check whether the clicked object is the list view item
            if (obj != null &&obj.GetType() == typeof(TextBlock))
            {
                ListViewItem a = (ListViewItem)playerWindow.OnlineView.SelectedItem;
                if (a.Content != null)
                {
                    string oppName = (string)a.Content;
                    messageHandler.sendRequest("PLAY," + playerName + "," + oppName);
                }
            }
        }

        private void HandleMulticastMenu(object sender, RoutedEventArgs e)
        {
            NetworkInformationWindow dialog = new NetworkInformationWindow();
            dialog.Owner = playerWindow;
            dialog.portSlider.Value = 50001;
            dialog.ShowDialog();
            multicastAddress = dialog.getIP();
            multicastPort = dialog.getPort();

            multicastThread = new Thread(ListenBroadcasts);
            multicastThread.IsBackground = true;
            multicastThread.Start();
        }

        private void HandleLogoutMenu(object sender, RoutedEventArgs e)
        {
            messageHandler.sendRequest("LOGOUT," + playerName);
            string response = messageHandler.getResponse();
            if (response == "OKAY")
            {
                playerName = "";
                playerWindow.Title = "Networked Checkers";
                playerWindow.statusBar.Text = "Logged Off Successfully";
            }
        }

        private void HandleMouseEvent(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                CheckerBoard board = (CheckerBoard)sender;
                IInputElement element = board.InputHitTest(e.GetPosition(board));
                board.HitTest(e.GetPosition(board));
                originRow = board.getRow().ToString();
                originCol = board.getCol().ToString();
            }
            else
            {
                CheckerBoard board = (CheckerBoard)sender;
                IInputElement element = board.InputHitTest(e.GetPosition(board));
                board.HitTest(e.GetPosition(board));
                destRow = board.getRow().ToString();
                destCol = board.getCol().ToString();
                messageHandler.sendRequest("TRY," + playerName + "," + originRow + originCol + "," + destRow + destCol);
            }
        }

        private void HandleSetNetworkMenu(object sender, RoutedEventArgs e)
        {
            NetworkInformationWindow dialog = new NetworkInformationWindow();
            dialog.Owner = playerWindow;
            dialog.ShowDialog();
            messageHandler.connectTo(dialog.getIP(), dialog.getPort());
        }

        private void HandleLoginMenu(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Owner = playerWindow;
            login.ShowDialog();
            string username = login.username;
            string password = login.password;
            var response = "";

            messageHandler.sendRequest("LOGIN," + username + "," + password);

            response = messageHandler.getResponse();
            if (response == "ERROR")
            {
                MessageBox.Show(playerWindow, "Could not log in.", "Error");
            }
            else
            {
                playerWindow.Title = "Networked Checkers - " + username + " - Online";
                playerWindow.loginMenu.IsEnabled = false;
                playerWindow.logoutmenu.IsEnabled = true;
                playerWindow.statusBar.Text = "Status: Logged in successfully as " + username;
                playerName = username;
            }
        }

        public void ListenBroadcasts()
        {
            string p1Name = "";
            string p2Name = "";
            var localIP = new IPEndPoint(IPAddress.Any, multicastPort);
            var udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.ExclusiveAddressUse = false;
            udpClient.Client.Bind(localIP);

            var multicastingIP = IPAddress.Parse(multicastAddress);
            udpClient.JoinMulticastGroup(multicastingIP);

            var running = true;
            while (running)
            {
                var bytes = udpClient.Receive(ref localIP);
                var message = Encoding.UTF8.GetString(bytes);

                if (Regex.Match(message, @"^USERS,[A-Za-z:]+$").Success)
                {
                    string[] names = message.Split(',');
                    string[] name = names[1].Split(':');
                    //clear the whole list
                    Dispatcher.Invoke(() =>
                    {
                        playerWindow.OnlineView.Items.Clear();
                    });
                    //add new listviewitems
                    foreach (string onlinePlayer in name)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ListViewItem newItem = new ListViewItem();
                            newItem.Content = onlinePlayer;
                            newItem.FontWeight = FontWeights.Bold;
                            newItem.FontSize = 30;
                            playerWindow.OnlineView.Items.Add(newItem);
                        });
                    }
                }
                else if (Regex.Match(message, @"^GAMESTATE,[A-Za-z0-9|]+$").Success)
                {
                    //place pieces according to the broadcast message
                    string[] messagePart = message.Split(',');
                    string[] piecesString = messagePart[1].Split('|');
                    string[] p1PieceArray = piecesString[0].Split('N');
                    string[] p2PieceArray = piecesString[1].Split('N');
                    int p1PieceArrayLength = p1PieceArray.Length;
                    int p2PieceArrayLength = p2PieceArray.Length;
                    for (int i = 0; i < 32; i++)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            playerWindow.GameBoard.showPiece(i, false);
                        });
                    }
                    for (int i = 1; i < p1PieceArrayLength; i++)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            int row = int.Parse(p1PieceArray[i].Substring(0, 1)) - 1;
                            int col = int.Parse(p1PieceArray[i].Substring(1, 1)) - 1;
                            int pos = (4 * row) + (int)(col / 2);
                            playerWindow.GameBoard.showPiece(pos, true);
                            playerWindow.GameBoard.setPieceColour(pos, Brushes.Red);
                        });
                    }
                    for (int i = 1; i < p2PieceArrayLength; i++)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            int row = int.Parse(p2PieceArray[i].Substring(0, 1)) - 1;
                            int col = int.Parse(p2PieceArray[i].Substring(1, 1)) - 1;
                            int pos = (4 * row) + (int)(col / 2);
                            playerWindow.GameBoard.showPiece(pos, true);
                            playerWindow.GameBoard.setPieceColour(pos, Brushes.Blue);
                        });
                    }
                }
                else if (Regex.Match(message, @"^GAMESTART,[A-Za-z]+,[A-Za-z]+$").Success)
                {
                    Dispatcher.Invoke(() =>
                    {
                        //disable all list view items
                        int noOfOnlineUsers = playerWindow.OnlineView.Items.Count;
                        for (int i = 0; i < noOfOnlineUsers; i++)
                        {
                            var item = (ListViewItem) playerWindow.OnlineView.Items.GetItemAt(i);
                            item.IsEnabled = false;
                        }

                        var msgParts = message.Split(',');
                        if (msgParts[1].Equals(playerName))
                        {
                            playerWindow.Title += " - Player 1 - Red Pieces";
                            playerWindow.IsEnabled = true;
                        }
                        else if (msgParts[2].Equals(playerName))
                        {
                            playerWindow.Title += " - Player 2 - Blue Pieces";
                            playerWindow.GameBoard.IsEnabled = false;
                        }
                        //if spectator, disable gameboard
                        else
                        {
                            p1Name = msgParts[1];
                            p2Name = msgParts[2];
                            playerWindow.GameBoard.IsEnabled = false;
                            playerWindow.statusBar.Text = "Status: Game Running: " + p1Name + " (Player 1) vs " + p2Name + " (Player2)";
                        }
                    });
                }
                else if (Regex.Match(message, @"^STATUS,[A-Za-z]+,[A-Za-z]+$").Success)
                {
                    string[] msgParts = message.Split(',');
                    //only process if the broadcast is intended to this user
                    if (msgParts[1] == playerName)
                    {
                        switch (msgParts[2])
                        {
                            case "MOVING":
                                Dispatcher.Invoke(() =>
                                {
                                    playerWindow.statusBar.Text = "Status: Your Move";
                                    playerWindow.GameBoard.IsEnabled = true;
                                });
                                break;

                            case "WAITING":
                                Dispatcher.Invoke(() =>
                                {
                                    playerWindow.statusBar.Text = "Status: Waiting For Opponent to Move";
                                    playerWindow.GameBoard.IsEnabled = false;
                                });
                                break;

                            case "WON":
                                Dispatcher.Invoke(() =>
                                {
                                    playerWindow.statusBar.Text = "Status: Game Over. You Win";
                                    playerWindow.Title = "Networked Checkers - " + playerName + " - Online";
                                    playerWindow.GameBoard.IsEnabled = false;

                                    //enable the listview items
                                    foreach (ListViewItem onlinePlayers in playerWindow.OnlineView.Items)
                                    {
                                        onlinePlayers.IsEnabled = true;
                                    }
                                });
                                break;

                            case "LOST":
                                Dispatcher.Invoke(() =>
                                {
                                    playerWindow.statusBar.Text = "Status: Game Over. You Lose";
                                    playerWindow.Title = "Networked Checkers - " + playerName + " - Online";
                                    playerWindow.GameBoard.IsEnabled = false;
                                    //enable the list view items
                                    foreach (ListViewItem onlinePlayers in playerWindow.OnlineView.Items)
                                    {
                                        onlinePlayers.IsEnabled = true;
                                    }
                                    playerWindow.GameBoard.AddMouseHandler((object sender, System.Windows.Input.MouseButtonEventArgs e) =>
                                    {

                                    });
                                });
                                break;
                        }
                    }
                    //if the user is spectator, change the status bar and enable the listviewitems
                    else
                    {
                        if (msgParts[2] == "WON")
                        {
                            if (msgParts[1].Equals(p1Name))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    playerWindow.statusBar.Text = "Status: " + p1Name + " (Player1) Won, " + p2Name + " (Player2) Lost";
                                });
                            }
                            else
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    playerWindow.statusBar.Text = "Status: " + p2Name + " (Player2) Won, " + p1Name + " (Player1) Lost";
                                });
                            }
                            Dispatcher.Invoke(() =>
                            {
                                foreach (ListViewItem onlinePlayers in playerWindow.OnlineView.Items)
                                {
                                    onlinePlayers.IsEnabled = true;
                                }
                            });
                        }
                    }
                }
            }

            udpClient.DropMulticastGroup(multicastingIP);
            udpClient.Close();
        }
    }
}