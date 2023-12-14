using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using StardewGPT.helpers;
using StardewModdingAPI;
using StardewValley;
using static StardewGPT.OpenAITypes;
using static StardewGPT.types.MetaDataTypes;

namespace StardewGPT
{
	public class NPCManager
	{
    public class NPCData
    {
      [JsonPropertyName("expertise")]
      public List<string> Expertise { get; set; }

      [JsonPropertyName("basic")]
      public BasicData Basic { get; set; }

      [JsonPropertyName("friendship")]
      public Treatment Treatment { get; set; }

      public NPCData() {
        Treatment = Treatment ?? new Treatment();
      }
    }

    public class BasicData
    {
      [JsonPropertyName("description")]
      public string Description { get; set; }

      [JsonPropertyName("personality")]
      public string Personality { get; set; }

      [JsonPropertyName("relationships")]
      public Dictionary<string, string> Relationships { get; set; }

      [JsonPropertyName("living_situation")]
      public string Living_Situation { get; set; }

      [JsonPropertyName("birthday")]
      public string Birthday { get; set; }
    }

    public static Dictionary<string, NPCData> storedData;

    public static MetaData MetaData { get; set; }

    public static void LoadMetaData()
    {
      string path = "assets/npcs/meta.json";
      MetaData = ModEntry.LHelper.ModContent.Load<MetaData>(path);
    }

    public static string[] GetNPCNames()
    {
      return MetaData.Names;
    }

    public static NPCData LoadNPCData(string name)
    {
      try
      {
        string path = $"assets/npcs/{name}.json";
        Logger.Log("Loading NPC Data: " + path);
        return ModEntry.LHelper.ModContent.Load<NPCData>(path);
      } catch{
        throw;
      }
    }

    public static NPCData GetNPCData(NPC npc)
    {
      return GetNPCData(npc.Name);
    }

    public static NPCData GetNPCData(string npcName)
    {
      try
      {
        if (storedData == null)
        {
          storedData = new Dictionary<string, NPCData>();
        }

        if (!storedData.ContainsKey(npcName))
        {
          storedData[npcName] = LoadNPCData(npcName);
        }

        return storedData[npcName];
      } catch{
        throw;
      }
    }

    public static bool HasNPCData(NPC npc)
    {
      return HasNPCData(npc.Name);
    }

    public static bool HasNPCData(string npcName)
    {
      try
      {
        GetNPCData(npcName);
        return true;
      }
      catch
      {
        Logger.Error("NPC Data not found for " + npcName );
        return false;
      }
    }
	}
}

