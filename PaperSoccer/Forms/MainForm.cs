using System;
using System.Drawing;
using System.Linq;
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
            labelPlayer1.Parent = pictureBox;
            labelPlayer2.Parent = pictureBox;
            new NewGame(_game).ShowDialog(this);
        }

        public void StartNewGame(int width, int height, PlayerNature playerNature)
        {
            _game = new Game(width, height, playerNature);
            _paperGraphics = new PaperGraphics(_game, pictureBox.Width, pictureBox.Height);
            pictureBox.Invalidate();
            moves.Text = FormatMoveText();
        }

        private void MenuNew_Click(object sender, EventArgs e)
        {
            new NewGame(new Game()).ShowDialog(this);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _game = new Game(_game.Width, _game.Height, _game.Player.Nature);
            _paperGraphics = new PaperGraphics(_game, pictureBox.Width, pictureBox.Height);
            pictureBox.Invalidate();
            moves.Text = FormatMoveText();
        }

        private string FormatMoveText()
        {
            int moves = _game == null ? 0 : _game.NumberOfMoves;
            return string.Format("{0}{1}", Resources.MainForm_StartNewGame_Moves__, moves);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            _paperGraphics.DrawField(e.Graphics);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (_paperGraphics != null)
            {
                _paperGraphics = new PaperGraphics(_game, pictureBox.Width, pictureBox.Height);
                _paperGraphics.SetHotspotsList();
            }
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs eventArgs)
        {
            if(eventArgs.Button != MouseButtons.Left ||
                !_paperGraphics.Hotspots.Any(h => h.Contains(eventArgs.Location)))
            {
                return;
            }

            var hotspot = _paperGraphics.Hotspots.Single(h => h.Contains(eventArgs.Location));
            var clickedPosition = new Point(hotspot.Right, hotspot.Bottom);
            var selectedPosition = _paperGraphics.FieldPosition(clickedPosition);
            
            _game.PlayerMove(selectedPosition);
            _paperGraphics.DrawMovesHistory(pictureBox.CreateGraphics());
            moves.Text = FormatMoveText();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var isInHotSpot = _paperGraphics.Hotspots.Any(s => s.Contains(e.Location));
            Cursor = isInHotSpot ? Cursors.Hand : Cursors.Default;
        }

        private void help_Click(object sender, EventArgs e)
        {
            new Help().Show();
        }
    }
}
