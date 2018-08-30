using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
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

        private void GetRidOfPanelFlickering()
        {
            typeof(Panel).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, paperSoccerPanel, new object[] { true });
        }

        public void StartNewGame(int width, int height, PlayerNature playerNature)
        {
            _game = new Game(width, height, playerNature);
            SetGraphics();
            moves.Text = FormatMoveText();
        }

        private void MenuNew_Click(object sender, EventArgs e)
        {
            new NewGame(new Game()).ShowDialog(this);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _game = new Game(_game.Width, _game.Height, _game.Player.Nature);
            SetGraphics();
            moves.Text = FormatMoveText();
        }

        private void SetGraphics()
        {
            _paperGraphics = new PaperGraphics(_game, paperSoccerPanel.Width, paperSoccerPanel.Height);
            _paperGraphics.SetHotspotsList();
            pictureBox.Location = _paperGraphics.GetBallLocation(pictureBox.Width);
            paperSoccerPanel.Invalidate();
        }

        private string FormatMoveText()
        {
            int moves = _game == null ? 0 : _game.NumberOfMoves;
            return string.Format("{0}{1}", Resources.MainForm_StartNewGame_Moves__, moves);
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
            _paperGraphics.DrawMovesHistory(paperSoccerPanel.CreateGraphics());
            pictureBox.Location = _paperGraphics.GetBallLocation(pictureBox.Width);
            moves.Text = FormatMoveText();
        }

        private void paperSoccerPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var isInHotSpot = _paperGraphics.Hotspots.Any(s => s.Contains(e.Location));
            Cursor = isInHotSpot ? Cursors.Hand : Cursors.Default;
        }
    }
}
