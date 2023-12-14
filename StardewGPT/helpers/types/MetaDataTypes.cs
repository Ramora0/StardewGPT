using System;
using System.Linq;
using StardewValley;
namespace StardewGPT.types
{
	public class MetaDataTypes
	{
    public class MetaData
    {
      public string[] Names { get; set; }

      // [JsonPropertyName("default_friendliness")]
      public Treatment Treatment { get; set; }
    }

    public class Treatment
    {
      public static FriendshipPoint[] DefaultFriendliness { get; set;}
      public FriendshipPoint[] Friendliness { get; set; }

      public string GetDescription(int points)
      {
        
        Friendliness = Friendliness.OrderByDescending(p => p.Points).ToArray();

        foreach (FriendshipPoint point in Friendliness)
        {
          if (points >= point.Points)
          {
            return point.Description;
          }
        }

        return null;
      }

      public Relationships Relationships { get; set; }

      public string GetDescription(Friendship friendship) {
        if(friendship.IsRoommate()) {
          return Relationships.Roomate;
        } else if(friendship.DaysUntilBirthing != -1) {
          return Relationships.Pregnant.GetDescription(Game1.player.IsMale).Replace("[DaysUntilBirthing]", friendship.DaysUntilBirthing.ToString());
        } else if(friendship.IsMarried()) {
          return Relationships.Marriage.Replace("[DaysMarried]", friendship.DaysMarried.ToString());
        } else if(friendship.IsEngaged()) {
          return Relationships.Engaged.Replace("[CountdownToWedding]", friendship.CountdownToWedding.ToString());
        } else if(friendship.IsDating()) {
          return Relationships.Dating;
        } else if(friendship.IsDivorced()) {
          return Relationships.Divorce;
        }

        return GetDescription(friendship.Points);
      }

      public Treatment() {
        if (DefaultFriendliness == null && NPCManager.MetaData != null) {
          DefaultFriendliness = NPCManager.MetaData.Treatment.Friendliness;
        }
        Friendliness = DefaultFriendliness;
      }

      public Treatment(FriendshipPoint[] points) {
        Friendliness = points;
      }
    }

    public class Relationships
    {
      public string Divorce { get; set; }
      public string Dating { get; set; }
      public string Engaged { get; set; }
      public string Marriage { get; set; }
      public SexStrings Pregnant { get; set; }
      public string Roomate { get; set; }
    }

    public class SexStrings{
      public string Male;
      public string Female;

      public string GetDescription(bool male) {
        return male ? Male : Female;
      }
    }

    public class FriendshipPoint
    {
      public int Points { get; set; }
      public string Description { get; set; }
    }
	}
}

