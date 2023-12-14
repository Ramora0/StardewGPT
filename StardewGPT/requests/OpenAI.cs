using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Threading.Tasks;
using Netcode;
using StardewGPT.helpers;
using StardewGPT.requests;
using StardewModdingAPI;
using static StardewGPT.OpenAITypes;

namespace StardewGPT
{
  public class OpenAI
  {
    public static List<GPTFunction> GPTFunctions { get; set; }

    public static void InstantiateFunctions()
    {
      GPTFunctions = ChatManager.InstantiateFunctions();
    }

    public static async Task<GPTMessage> GetResponse(Action<string> setStatus)
    {
      try
      {
        Response response1 = await MakeRequest(setStatus, new APIParams
        {
          Messages = ChatManager.CurrentConversation.ToArray(),
          Functions = GPTFunctions.ToArray()
        });

        if (response1.Message.FunctionCall == null)
        {
          ChatManager.AddMessage(response1.Message);
          return response1.Message;
        }

        GPTFunctionResultMessage functionResult = FunctionCalling.CallFunction(response1.Message.FunctionCall);
        Logger.Log("FUNCTION RESPONSE: " + JsonSerializer.Serialize(functionResult));
        ChatManager.AddMessage(functionResult);

        setStatus.Invoke("Thinking harder...");
        Response response2 = await MakeRequest(setStatus, new APIParams
        {
          Messages = ChatManager.CurrentConversation.ToArray(),
        });

        ChatManager.AddMessage(response2.Message);
        return response2.Message;
      } catch {
        throw;
      }
    }

    public static async Task<Response> MakeRequest(Action<string> setStatus, APIParams param, int retry = 3)
    {
      try
      {
        Logger.Log("REQUEST: " + JsonSerializer.Serialize(param));
        using (HttpClient client = new HttpClient())
        {
          string url = "https://z92m33gnq2.execute-api.us-east-1.amazonaws.com/get-response?";
          url += "user-uuid=" + ModEntry.ID;
          var request = new HttpRequestMessage(HttpMethod.Post, url);

          StringContent content = new StringContent(JsonSerializer.Serialize(param), System.Text.Encoding.UTF8, "application/json");
          request.Content = content;

          var receivedData = await client.SendAsync(request);
          string responseString = await receivedData.Content.ReadAsStringAsync();
          Logger.Log("RESPONSE: " + responseString);
          var response = FormatResponse(responseString);

          Logger.Log("Cents left: " + response.Charge.UpdatedFreeCentsLeft);

          return response;
        }
      } catch {
        throw;
      }
    }

    public static async void GetSubscriptionStatus() {
      Logger.Log("REQUEST: Subscription status of "+ModEntry.ID);
      try
      {
        using (HttpClient client = new HttpClient())
        {
          string url = "https://z92m33gnq2.execute-api.us-east-1.amazonaws.com/get-subscription-status?";
          url += "user-uuid=" + ModEntry.ID;
          var request = new HttpRequestMessage(HttpMethod.Get, url);
          var response = await client.SendAsync(request);
          string responseString = await response.Content.ReadAsStringAsync();
          Logger.Log("RESPONSE: " + responseString);
          var subscriptionStatus = JsonSerializer.Deserialize<SubscriptionStatusResponse>(responseString);
        }
      } catch(Exception e) {
        Logger.Log("Error: " + e.ToString());
      }
    }

    public static Response FormatResponse(string rawResponse)
    {
      try
      {
        using (JsonDocument doc = JsonDocument.Parse(rawResponse))
        {
          if (doc.RootElement.TryGetProperty("error", out JsonElement errorElement))
          {
            throw new APIException(errorElement.GetString());
          }
        }
        APIResponse apiResponse = JsonSerializer.Deserialize<APIResponse>(rawResponse);
        return new Response(apiResponse);
      } catch {
        throw;
      }
    }

    public class APIException : Exception
    {
      public APIException(string message) : base(message) {}

      public string GetAbbreviatedError() {
        if (Message.IndexOf(':') == -1)
          return Message;
        return Message.Substring(0, Message.IndexOf(':'));
      }
    }
  }
}

