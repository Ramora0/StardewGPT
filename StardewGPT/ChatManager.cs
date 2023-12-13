using System;
using System.Collections.Generic;
using System.Text.Json;
using StardewValley;
using static StardewGPT.NPCManager;
using static StardewGPT.OpenAITypes;
namespace StardewGPT
{
	public class ChatManager
	{
    public static List<GPTMessage> CurrentConversation { get; set; }

    public static void FinishConversation()
    {
      CurrentConversation = null;
      //Maybe save the conversation in the future? Idk what youd do with it though
    }

    public static void StartConversation(NPC npc)
    {
      CurrentConversation = new List<GPTMessage>();

      AddMessage(GetSystemMessage(npc));
    }

    public static void RemoveLastMessage() {
      CurrentConversation.RemoveAt(CurrentConversation.Count - 1);
    }

    public static GPTSystemMessage GetSystemMessage(NPC npc) {
      NPCData data = NPCManager.GetNPCData(npc);

      string message = $"Engage {Game1.player.Name} in a first-person conversation, making sure to treat them ";

      Friendship friendship = Game1.player.friendshipData[npc.Name];
      message += data.Treatment.GetDescription(friendship);

      message += $". You are the Stardew Valley Character {npc.Name}, {data.Basic.Description}"
          +$" and talks {data.Basic.Personality}."
          +" Answer with brief, conversation-like responses.";

      return new GPTSystemMessage(message);
    }

    public static void AddMessage(GPTMessage message) => CurrentConversation.Add(message);
    public static void AddMessage(GPTFunctionResultMessage message) => CurrentConversation.Add(message);

    public static List<GPTFunction> InstantiateFunctions()
    {
      List<string> Enum = new List<string>();

      //NPC Names
      Enum.AddRange(NPCManager.GetNPCNames());

      //Environment
      Enum.AddRange(new string[] { "weather", "date", "time", "location"});

      return new List<GPTFunction>
      {
        new GPTFunction
        {
          Name = "get_additional_info",
          Description = "Gets further information",
          Parameters = new Parameters
          {
            Properties = new Dictionary<string, Property>
            {
              {
                "subject",
                new Property
                {
                  Type = "string",
                  Enum = Enum.ToArray()
                }
              }
            },
            Required = new List<string>
            {
              "subject"
            }
          }
        }
      };
    }
	}
}

