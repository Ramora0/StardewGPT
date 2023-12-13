using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using xTile.Format;
namespace StardewGPT.ui
{
  public class MultiLineTextBox : IKeyboardSubscriber
  {
    private string _text = "";
    public string Text
    {
      get
      {
        return _text;
      }
      set
      {
        if(value==null)
          return;

        _text = "";

        for (int i = 0; i < value.Length; i++)
        {
          string prevText = _text;
          _text += value[i];

          Vector2 textSize = Font.MeasureString(_text);

          if (textSize.X >= (float)(Width - 21))
          {
            int lastSpace = _text.LastIndexOf(' ');
            string lastWord = _text.Substring(lastSpace + 1);

            if (lastWord.Contains(Environment.NewLine) || lastSpace == -1)
            {
              _text = prevText;
              _text += Environment.NewLine;
              _text += value[i];
            }
            else
            {
              _text = _text.Substring(0, lastSpace);
              _text += Environment.NewLine;
              _text += lastWord;
            }

            textSize = Font.MeasureString(_text);
          }

          if (textSize.Y >= (float)(Height - 120))
          {
            _text = prevText;
            return;
          }
        }
      }
    }

    public SpriteFont Font { get; set; }
    public Color TextColor { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Selected { get; set; }
    public event Action OnSubmit;

    public MultiLineTextBox(Rectangle bounds)
    {
      SetDimensions(bounds);
      Font = Game1.dialogueFont;
      TextColor = Game1.textColor;
      Text = "";
    }

    public void SetDimensions(Rectangle bounds) {
      X = bounds.X;
      Y = bounds.Y;
      Width = bounds.Width;
      Height = bounds.Height;
    }

    public void Draw(SpriteBatch spriteBatch, bool drawShadow = true)
    {
      bool flag = !(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 < 500.0);

      Game1.drawDialogueBox(X - 32, Y - 112 + 10, Width + 80, Height, speaker: false, drawOnlyBox: true);

      if (flag && Selected)
      {
        float cursorX = Font.MeasureString(Text.Split('\n').Last()).X;

        int lineCount = Text.Split('\n').Length;
        string newLines = new String('\n', lineCount - 1) + " ";
        float cursorY = Font.MeasureString(newLines).Y;

        spriteBatch.Draw(Game1.staminaRect, new Rectangle(X + 16 + (int)cursorX + 2, Y + (int)cursorY - 44, 4, 32), TextColor);
      }

      spriteBatch.DrawString(Font, Text, new Vector2(X + 16, Y), TextColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
    }

    public void RecieveCommandInput(char command)
    {
      if (command == 8)
      {
        if (Text.Length > 0)
        {
          Text = Text.Substring(0, Text.Length - 1);
        }
      }
      else if(command == 9) {
        OnSubmit.Invoke();
      }
      else if (command == 13)
      {
        if(ModEntry.LHelper.Input.IsDown(SButton.LeftShift) || ModEntry.LHelper.Input.IsDown(SButton.RightShift)) {
          Text += "\n";
        } else{
          OnSubmit.Invoke();
        }
      }

      // ModEntry.Log("Recieved command input: " + (int)command);
    }

    public void RecieveSpecialInput(Keys key)
    {
      // Text += key.ToString();
      // ModEntry.Log("Recieved Special input: " + key);
    }

    public void RecieveTextInput(char inputChar)
    {
      // if (Text == " ")
      //   Text = "";

      Text += inputChar;

      // ModEntry.Log("Recieved Text input: " + inputChar);
    }

    public void RecieveTextInput(string text)
    {
      // throw new NotImplementedException();
      Text += text;
      // ModEntry.Log("Recieved Char input: " + text);
    }
  }
}