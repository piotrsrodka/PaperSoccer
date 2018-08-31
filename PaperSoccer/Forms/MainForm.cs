using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using PaperSoccer.Enums;
using PaperSoccer.Properties;

namespace PaperSoccer.Forms
{
    public partial class MainForm : Form
    {
        private PaperGraphics _paperGraphics;
        private Game _game = new Game();

        public MainForm()
        {
            InitializeComponent();
            GetRidOfPanelFlickering();
            labelPlayer1.Parent = paperSoccerPanel;
            labelPlayer2.Parent = paperSoccerPanel;
            new NewGame(_game).ShowDialog(this);
        }

        public void StartNewGame(int width, int height, 
            string player1Name, PlayerNature player1Nature,
            string player2Name, PlayerNature player2Nature)
        {
            _game = new Game(width, height, player1Name, player1Nature,
            player2Name, player2Nature);
            SetGraphics();
            FormatMoveText(_game.NumberOfMoves);
        }

        private void paperSoccerPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                !_paperGraphics.Hotspots.Any(h => h.Contains(e.Location)))
            {
                return;
            }

            var hotspot = _paperGraphics.Hotspots.Single(h => h.Contains(e.Location));
            var clickedPosition = new Point(hotspot.Right, hotspot.Bottom);
            var selectedPosition = _paperGraphics.FieldPosition(clickedPosition);

            _game.PlayerMove(selectedPosition);
            UpdateMove();

            if (_game.IsComputerTurn)
            {
                Thread.Sleep(250);
                _game.ComputerMove();
                UpdateMove();
            }
        }

        private void UpdateMove()
        {
            _paperGraphics.DrawLastMoves(paperSoccerPanel.CreateGraphics());
            pictureBox.Location = _paperGraphics.GetBallLocation(pictureBox.Width);
            FormatMoveText(_game.NumberOfMoves);
        }

        private void GetRidOfPanelFlickering()
        {
            typeof(Panel).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, paperSoccerPanel, new object[] { true });
        }

        private void MenuNew_Click(object sender, EventArgs e)
        {
            new NewGame(new Game()).ShowDialog(this);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _game = new Game(_game.Width, _game.Height,  _game.PlayerOne.Name, _game.PlayerOne.Nature,
                _game.PlayerTwo.Name, _game.PlayerTwo.Nature);
            SetGraphics();
            FormatMoveText(_game.NumberOfMoves);
        }

        private void SetGraphics()
        {
            _paperGraphics = new PaperGraphics(_game, paperSoccerPanel.Width, paperSoccerPanel.Height);
            pictureBox.Location = _paperGraphics.GetBallLocation(pictureBox.Width);
            labelPlayer1.Text = _game.PlayerOne.Name + "[" + _game.PlayerOne.Nature + "]";
            labelPlayer2.Text = _game.PlayerTwo.Name + "[" + _game.PlayerTwo.Nature + "]";
            paperSoccerPanel.Invalidate();
        }

        private void FormatMoveText(int numberOfMoves)
        {
            if (numberOfMoves > 0)
            {
                moves.Text = string.Format("{0}{1}", Resources.MainForm_StartNewGame_Moves__, numberOfMoves);
            }
            else
            {
                moves.Text = Resources.MainForm_NoMoves;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            SetGraphics();
        }

        private void help_Click(object sender, EventArgs e)
        {
            new Help().Show();
        }

        private void paperSoccerPanel_Paint(object sender, PaintEventArgs e)
        {
            _paperGraphics.DrawField(e.Graphics);
        }

        private void paperSoccerPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var isInHotSpot = _paperGraphics.Hotspots.Any(s => s.Contains(e.Location));
            Cursor = isInHotSpot ? Cursors.Hand : Cursors.Default;
        }
    }
}
