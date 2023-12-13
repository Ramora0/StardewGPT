using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static StardewGPT.OpenAITypes;
using StardewModdingAPI;
using StardewGPT.ui;
using StardewModdingAPI.Events;
using static StardewGPT.OpenAI;

namespace StardewGPT
{
  public class ChatMenu : IClickableMenu, WindowResizable
  {
    public MultiLineTextBox textBox;
    ButtonUI SubmitButton;
    private NPC speaker;
    private ButtonUI _Message;
    string Message {
      get {
        return _Message?.Text;
      }
      set {
        if (value == null)
        {
          _Message = null;
          return;
        }

        if (_Message == null)
          _Message = new ButtonUI(value);
        else
          _Message.Text = value;
        _Message.MiddleX(Game1.viewport.Width / 2);
        _Message.MiddleY(Game1.viewport.Height / 2);
      }
    }

    public ChatMenu(NPC speaker) : base(ChatMenu.CalculateX(Game1.viewport.Width), ChatMenu.CalculateY(Game1.viewport.Height), 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2)
    {
      this.speaker = speaker;
      
      this.textBox = new MultiLineTextBox(CalculateTextboxDimensions());

      SubmitButton = new ButtonUI("Submit");

      this.textBox.OnSubmit += this.OnSubmit;

      Game1.keyboardDispatcher.Subscriber = this.textBox;

      OnWindowResize(Game1.viewport.Width, Game1.viewport.Height);
    }

    public Rectangle CalculateTextboxDimensions()
    {
      return new Rectangle
      {
        X = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder,
        Y = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder,
        Width = this.width - IClickableMenu.spaceToClearSideBorder * 2,
        Height = this.height - IClickableMenu.spaceToClearTopBorder - Game1.tileSize
      };
    }

    public static int CalculateX(int windowWidth)
    {
      return windowWidth / 2 - (800 + IClickableMenu.borderWidth * 2) / 2;
    }

    public static int CalculateY(int windowHeight)
    {
      return windowHeight / 2 - (600 + IClickableMenu.borderWidth * 2) / 2;
    }

    public void OnWindowResize(int windowWidth, int windowHeight)
    { 
      this.xPositionOnScreen = ChatMenu.CalculateX(windowWidth);
      this.yPositionOnScreen = ChatMenu.CalculateY(windowHeight);
      textBox.SetDimensions(CalculateTextboxDimensions());

      SubmitButton.RightX(textBox.X + textBox.Width + 10);
      SubmitButton.TopY(textBox.Y + textBox.Height - 100);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (Message != null)
      {
        return;
      }

      if (SubmitButton.ContainsPoint(x, y))
      {
        OnSubmit();
        return;
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      if (key == Keys.E)
      {
        return;
      }

      base.receiveKeyPress(key);
    }

    private async void OnSubmit()
    {
      Message = "Thinking...";
      
      ChatManager.AddMessage(new GPTUserMessage(textBox.Text));
      try
      {
        GPTMessage response = await OpenAI.GetResponse((txt) => Message = txt);
        if (response == null)
          return;
        Game1.activeClickableMenu = new ChatDialogueBox(new Dialogue(response.Content, speaker));
      }
      // catch (HttpRequestException e) 
      // {
      //   ChatManager.RemoveLastMessage();
      //   ModEntry.Log("Error making request: " + e.ToString(), LogLevel.Error);
      //   Message = "No internet bruv";
      // }
      catch(APIException e)
      {
        ChatManager.RemoveLastMessage();
        ModEntry.Log("API Error: " + e.ToString(), LogLevel.Error);
        Message = e.GetAbbreviatedError();
      }
      catch (Exception e)
      {
        ChatManager.RemoveLastMessage();
        ModEntry.Log("Error making request: " + e.ToString(), LogLevel.Error);
        Message = "Error making request";
      }
    }

    public override void draw(SpriteBatch spriteBatch)
    {
      if (Message != null)
      {
        _Message.Draw(spriteBatch);
      }else {
        textBox.Draw(spriteBatch);
        SubmitButton.Draw(spriteBatch);
      }
      drawMouse(spriteBatch);
    }

    public static void ButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      if (e.Button == SButton.MouseRight)
      {
        ICursorPosition cursorPos = ModEntry.LHelper.Input.GetCursorPosition();

        if(Game1.currentLocation == null)
          return;

        NPC npc = Game1.currentLocation.isCharacterAtTile(cursorPos.GrabTile);

        if (npc != null && npc.CurrentDialogue.Count == 0) {
          Game1.activeClickableMenu = new ChatMenu(npc);
          ChatManager.StartConversation(npc);
        }
      }
    }
  }
}