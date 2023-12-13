using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
namespace StardewGPT.ui
{
	public class ButtonUI
	{
    public string Text;

    public int X, Y;
    // public Action OnClick;

    public ButtonUI(string text) {
      Text = text;
    }

    public void RightX(int x) {
      X = x - (int) Game1.dialogueFont.MeasureString(Text).X;
    }

    public void MiddleX(int x) {
      X = x - (int) (Game1.dialogueFont.MeasureString(Text).X / 2);
    }

    public void LeftX(int x) {
      X = x;
    }

    public void BottomY(int y) {
      Y = y - (int) Game1.dialogueFont.MeasureString(Text).Y;
    }

    public void MiddleY(int y) {
      Y = y - (int) (Game1.dialogueFont.MeasureString(Text).Y / 2);
    }

    public void TopY(int y) {
      Y = y;
    }

    public ButtonUI(string text, int x, int y) {
      Text = text;
      X = x;
      Y = y;
    }
    
    public void Draw(SpriteBatch spriteBatch) {
      Vector2 textSize = Game1.dialogueFont.MeasureString(Text);
      
      Game1.drawDialogueBox(X - 38, Y-93, (int) textSize.X + 75, (int) textSize.Y + 132, speaker: false, drawOnlyBox: true);
      spriteBatch.DrawString(Game1.dialogueFont, Text, new Vector2(X, Y+7), Game1.textColor);
    }

    public bool ContainsPoint(int x, int y) {
      Vector2 textSize = Game1.dialogueFont.MeasureString(Text);
      return x >= X && x <= X + textSize.X && y >= Y && y <= Y + textSize.Y;
    }
	}
}

