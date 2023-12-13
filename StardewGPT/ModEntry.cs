using System;
using StardewGPT.ui;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using static StardewGPT.OpenAITypes;

namespace StardewGPT
{
  /// <summary>The mod entry point.</summary>
  internal sealed class ModEntry : Mod
  {
    public static IModHelper LHelper;
    public static IMonitor LMonitor;
    public static string ID;

    public static void Log(string message, LogLevel level = LogLevel.Debug)
    {
      LMonitor.Log(message, level);
    }
    /*********
    ** Public methods
    *********/
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
      ModEntry.LHelper = helper;
      ModEntry.LMonitor = Monitor;
      NPCManager.LoadMetaData();

      OpenAI.InstantiateFunctions();

      helper.Events.Input.ButtonPressed += ChatMenu.ButtonPressed;
      helper.Events.Input.ButtonPressed += this.OnButtonPressed;
      helper.Events.Display.MenuChanged += this.OnMenuChanged;
      helper.Events.Display.WindowResized += this.OnWindowResize;

      string id = helper.Data.ReadGlobalData<string>("unique-id");
      if(!Guid.TryParse(id, out _))
      {
        id = Guid.NewGuid().ToString();
        helper.Data.WriteGlobalData("unique-id", id);
      }

      ModEntry.ID = id;
      
      OpenAI.GetSubscriptionStatus();

      Log("Unique ID: "+id);
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      if (e.Button == SButton.J)
      {
        Log(Game1.player.currentLocation.Name);
      }
    }

    private void OnWindowResize(object sender, WindowResizedEventArgs e)
    {
      if (Game1.activeClickableMenu is WindowResizable windowResizeableMenu)
      {
        windowResizeableMenu.OnWindowResize(e.NewSize.X, e.NewSize.Y);
      }
    }

    private void OnMenuChanged(object sender, MenuChangedEventArgs e)
    {
      if (e.NewMenu != null && e.NewMenu.GetType() == typeof(DialogueBox))
      {

        DialogueBox dialogueBox = (DialogueBox)e.NewMenu;
        Dialogue dialogue = dialogueBox.characterDialogue;

        if (dialogue == null || dialogue.speaker == null || !NPCManager.HasNPCData(dialogue.speaker))
          return;

        ChatManager.StartConversation(dialogue.speaker);
        ChatManager.AddMessage(new GPTAssistantMessage(string.Join(' ', dialogue.dialogues)));
        Game1.activeClickableMenu = new ChatDialogueBox(dialogue);
      }

      if(e.NewMenu == null && e.OldMenu.GetType() == typeof(ChatDialogueBox))
      {
        ChatManager.FinishConversation();
      }
    }
  }
}