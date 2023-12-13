using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using StardewGPT.ui;

namespace StardewGPT
{
  public class ChatDialogueBox : DialogueBox, WindowResizable
  {
    public ButtonUI chatButton;

    public ChatDialogueBox(Dialogue dialogue) : base(dialogue)
    {
      chatButton = new ButtonUI("Chat");
      OnWindowResize(Game1.viewport.Width, Game1.viewport.Height);
    }


    public void OnWindowResize(int width, int height)
    {
      chatButton.LeftX(Game1.viewport.Width / 2 - 600);
      chatButton.TopY(Game1.viewport.Height - 520);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (chatButton.ContainsPoint(x, y))
      {
        Game1.activeClickableMenu = new ChatMenu(base.characterDialogue.speaker);
        return;
      }

      base.receiveLeftClick(x, y, playSound);
    }

    public override void draw(SpriteBatch spriteBatch)
    {
      chatButton.Draw(spriteBatch);
      base.draw(spriteBatch);
    }
  }
}