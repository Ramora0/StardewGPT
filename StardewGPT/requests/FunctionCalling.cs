using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic;
using StardewValley;
using static StardewGPT.NPCManager;
using static StardewGPT.OpenAITypes;
namespace StardewGPT.requests
{
	public class FunctionCalling
	{
    public class Arguments {
      [JsonPropertyName("subject")]
      public string Subject { get; set; }
    }

    public static GPTFunctionResultMessage CallFunction(FunctionCall functionCall) {
      if (functionCall.Name != "get_additional_info")
        return null;
      
      Arguments arguments = JsonSerializer.Deserialize<Arguments>(functionCall.Arguments);
      if (arguments.Subject == "date" || arguments.Subject == "time")
        return ToMessage(GetDateTime());
      else if (arguments.Subject == "weather")
        return ToMessage(GetWeather());
      else if (arguments.Subject == "location")
        return ToMessage(GetLocation());
      else
        return ToMessage(GetNPCInfo(arguments.Subject));
    }

    public static object GetWeather() {
      string weather = "sunny";
      if(Game1.isLightning)
        weather = "lightning";
      else if(Game1.isRaining)
        weather = "rain";
      else if(Game1.isSnowing)
        weather = "snow";

      return new { weather };
    }

    public static object GetLocation() {
      return new { location = Game1.player.currentLocation.Name };
    }

    public static object GetDateTime() {
      WorldDate date = Game1.Date;

      string time = "night";
      if(Game1.timeOfDay < 1200)
        time = "morning";
      else if(Game1.timeOfDay < 1800)
        time = "afternoon";
      else if(Game1.timeOfDay < 2200)
        time = "evening";
      return new
      {
        day_of_week = date.DayOfWeek.ToString(),
        day_of_month = date.DayOfMonth,
        month = date.Season,
        year = date.Year,
        time
      };
    }

    public static object GetNPCInfo(string npcName) {
      if(NPCManager.HasNPCData(npcName)) {
        NPCData data = NPCManager.GetNPCData(npcName);
        return new {
          data.Expertise,
          data.Basic
        };
      }
      throw new Exception($"Cannot call GetNPCInfo for NPC {npcName}; does not have any data.");
    }

    public static GPTFunctionResultMessage ToMessage(object obj, string name = "get_additional_info") {
      return new GPTFunctionResultMessage(JsonSerializer.Serialize(obj), name);
    }
	}
}

