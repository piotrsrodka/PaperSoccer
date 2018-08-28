using System;
using System.Windows.Forms;
using PaperSoccer.Enums;

namespace PaperSoccer.Forms
{
    public partial class NewGame : Form
    {
        private const int DefaultWidth = 8;
        private const int DefaultHeight = 10;

        private int _width;
        private int _height;

        private PlayerNature _opponentPlayerNature; 

        public NewGame(Game game)
        {
            InitializeComponent();
            
            _width = game.Width;
            _height = game.Height;
            _opponentPlayerNature = game.Player.Nature;

            numericUpDownWidth.Value = _width == 0 ? DefaultWidth : _width;
            numericUpDownHeight.Value = _height == 0 ? DefaultHeight : _height;
            buttonOpponent.Text = _opponentPlayerNature.ToString();
            ActiveControl = buttonStartNewGame;
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            numericUpDownWidth.Value = DefaultWidth;
            numericUpDownHeight.Value = DefaultHeight;
        }

        private void buttonStartNewGame_Click(object sender, EventArgs e)
        {            
            Close();
        }

        private void numericUpDownWidth_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownWidth.Value % 2 != 0) numericUpDownWidth.Value++;
        }

        private void numericUpDownHeight_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownHeight.Value % 2 != 0) numericUpDownHeight.Value++;
        }

        private void buttonOpponent_Click(object sender, EventArgs e)
        {
            switch (_opponentPlayerNature)
            {
                case PlayerNature.Human:
                    _opponentPlayerNature = PlayerNature.Computer;
                    break;
                case PlayerNature.Computer:
                    _opponentPlayerNature = PlayerNature.Human;
                    break;
            }

            buttonOpponent.Text = _opponentPlayerNature.ToString();
        }

        private void NewGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            _width = (int) numericUpDownWidth.Value;
            _height = (int) numericUpDownHeight.Value;

            if (_width % 2 != 0) _width++;
            if (_height % 2 != 0) _height++;
            if (_width < 3 || _width > 30) _width = DefaultWidth;
            if (_height < 3 || _height > 30) _height = DefaultHeight;

            var parent = (MainForm) Owner;
            parent.StartNewGame(_width, _height, _opponentPlayerNature);
        }
    }
}
