using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkedCheckersClient
{
    /// <summary>
    /// Interaction logic for CheckerBoard.xaml
    /// </summary>
    public partial class CheckerBoard : UserControl
    {
        int row;
        int col;

        public CheckerBoard()
        {
            InitializeComponent();
        }

        public void showPiece(int position, bool isShowing)
        {
            int value = isShowing ? 1 : 0;
            Dispatcher.Invoke(() =>
            {
                Panel.SetZIndex(MainGrid.Children[position], value);
            });
        }

        public void setPieceColour(int position, Brush brush)
        {
            Dispatcher.Invoke(() =>
            {
                Ellipse ellipse = (Ellipse)MainGrid.Children[position];
                ellipse.Fill = brush;
            });
        }

        public void AddMouseHandler(MouseButtonEventHandler handler)
        {
            MouseDown += handler;
            MouseUp += handler;
        }

        public void HitTest(Point point)
        {
            UIElement element = (UIElement)InputHitTest(point);
            row = Grid.GetRow(element) + 1;
            col = Grid.GetColumn(element) + 1;
        }

        public int getRow()
        {
            return row;
        }

        public int getCol()
        {
            return col;
        }
    }
}
