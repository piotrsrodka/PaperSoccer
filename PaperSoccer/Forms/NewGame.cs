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

        private string _lastName1;
        private string _lastName2;

        private PlayerNature _Player1Nature; 
        private PlayerNature _Player2Nature; 

        public NewGame(Game game)
        {
            InitializeComponent();
            
            _width = game.Width;
            _height = game.Height;
            _Player1Nature = game.PlayerOne.Nature;
            _Player2Nature = game.PlayerTwo.Nature;

            numericUpDownWidth.Value = _width == 0 ? DefaultWidth : _width;
            numericUpDownHeight.Value = _height == 0 ? DefaultHeight : _height;
            buttonOpponent.Text = _Player2Nature.ToString();
            buttonPlayer1Nature.Text = _Player1Nature.ToString();
            textBoxPlayer1Name.Text = game.PlayerOne.Name;
            textBoxPlayer2Name.Text = game.PlayerTwo.Name;
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
            switch (_Player2Nature)
            {
                case PlayerNature.Human:
                    _lastName2 = textBoxPlayer2Name.Text;
                    _Player2Nature = PlayerNature.Computer;
                    textBoxPlayer2Name.Text = "Walter";
                    break;
                case PlayerNature.Computer:
                    _Player2Nature = PlayerNature.Human;
                    textBoxPlayer2Name.Text = _lastName2;
                    break;
            }

            buttonOpponent.Text = _Player2Nature.ToString();
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
            parent.StartNewGame(_width, _height, textBoxPlayer1Name.Text, _Player1Nature,
                textBoxPlayer2Name.Text, _Player2Nature);
        }

        private void buttonPlayer1Nature_Click(object sender, EventArgs e)
        {
            switch (_Player1Nature)
            {
                case PlayerNature.Human:
                    _lastName1 = textBoxPlayer1Name.Text;
                    textBoxPlayer1Name.Text = "Walter";
                    _Player1Nature = PlayerNature.Computer;
                    break;
                case PlayerNature.Computer:
                    _Player1Nature = PlayerNature.Human;
                    textBoxPlayer1Name.Text = _lastName1;
                    break;
            }

            buttonPlayer1Nature.Text = _Player1Nature.ToString();
        }
    }
}
