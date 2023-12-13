using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using StardewValley;
namespace StardewGPT
{
	public class OpenAITypes
	{

    public class GPTFunction
    {
      [JsonPropertyName("name")]
      public string Name { get; set; }

      [JsonPropertyName("description")]
      public string Description { get; set; }

      [JsonPropertyName("parameters")]
      public Parameters Parameters { get; set; }
    }

    public class Parameters
    {
      [JsonPropertyName("type")]
      public string Type { get; set;  } = "object";

      [JsonPropertyName("properties")]
      public Dictionary<string, Property> Properties { get; set; } = new Dictionary<string, Property>();

      [JsonPropertyName("required")]
      public List<string> Required { get; set; } = new List<string>();
    }

    public class Property
    {
      [JsonPropertyName("type")]
      public string Type { get; set; }

      [JsonPropertyName("enum")]
      public string[] Enum { get; set; }
    }
    
    public class GPTMessage
    {
      [JsonPropertyName("role")]
      public string Role { get; set; }

      [JsonPropertyName("content")]
      public string Content { get; set; }

      [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
      [JsonPropertyName("name")]
      public string Name { get; set; }

      [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
      [JsonPropertyName("function_call")]
      public FunctionCall FunctionCall { get; set; }

      public GPTMessage() {}
      
      public GPTMessage(string role, string content) {
        Role = role;
        Content = content;
      }
    }

    public class GPTAssistantMessage : GPTMessage {
      public GPTAssistantMessage(string content) : base("assistant", content) { }
    }

    public class GPTUserMessage : GPTMessage {
      public GPTUserMessage(string content) : base("user", content) { }
    }

    public class GPTSystemMessage : GPTMessage {
      public GPTSystemMessage(string content) : base("system", content) {}
    }

    public class GPTFunctionCallMessage : GPTMessage {
      public GPTFunctionCallMessage(FunctionCall functionCall) : base("function_call", null) {
        FunctionCall = functionCall;
      }
    }

    public class GPTFunctionResultMessage : GPTMessage {
      public GPTFunctionResultMessage(string functionResult, string name) : base("function", functionResult) {
        Name = name;
      }
    }

    public class Choice
    {
      [JsonPropertyName("index")]
      public int Index { get; set; }

      [JsonPropertyName("message")]
      public GPTMessage Message { get; set; }

      [JsonPropertyName("finish_reason")]
      public string FinishReason { get; set; }
    }

    public class FunctionCall
    {
      [JsonPropertyName("name")]
      public string Name { get; set; }

      [JsonPropertyName("arguments")]
      public string Arguments { get; set; }
    }

    public class Usage
    {
      [JsonPropertyName("prompt_tokens")]
      public int PromptTokens { get; set; }

      [JsonPropertyName("completion_tokens")]
      public int CompletionTokens { get; set; }

      [JsonPropertyName("total_tokens")]
      public int TotalTokens { get; set; }
    }

    public class APIResponse
    {
      [JsonPropertyName("choices")]
      public Choice[] Choices { get; set; }

      [JsonPropertyName("usage")]
      public Usage Usage { get; set; }

      [JsonPropertyName("charge")]
      public Charge Charge { get; set; }
    }

    public class Response
    {
      public GPTMessage Message { get; set; }

      public Usage Usage { get; set; }
      public Charge Charge { get; set; }

      public Response(APIResponse apiResponse)
      {
        Message = apiResponse.Choices[0].Message;
        Usage = apiResponse.Usage;
        Charge = apiResponse.Charge;
      }
    }

    public class Charge
    {
      [JsonPropertyName("prompt_cost")]
      public decimal PromptCost { get; set; }

      [JsonPropertyName("completion_cost")]
      public decimal CompletionCost { get; set; }

      [JsonPropertyName("total_cost")]
      public decimal TotalCost { get; set; }

      [JsonPropertyName("updated_free_cents_left")]
      public decimal UpdatedFreeCentsLeft { get; set; }
    }

    public class SubscriptionStatusResponse{
      public string Status { get; set; }

      [JsonPropertyName("cents")]
      public decimal Cents { get; set; }
    }

    public class APIParams {
      [JsonPropertyName("model")]
      public string Model { get; set; } = "gpt-3.5-turbo-1106";

      [JsonPropertyName("messages")]
      public GPTMessage[] Messages { get; set; }

      [JsonPropertyName("temperature")]
      public int Temperature { get; set; } = 1;

      [JsonPropertyName("max_tokens")]
      public int MaxTokens { get; set; } = 256;

      [JsonPropertyName("top_p")]
      public int TopP { get; set; } = 1;

      [JsonPropertyName("frequency_penalty")]
      public int FrequencyPenalty { get; set; } = 0;

      [JsonPropertyName("presence_penalty")]
      public int PresencePenalty { get; set; } = 0;

      [JsonPropertyName("functions")]
      [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
      public GPTFunction[] Functions { get; set; }
    }
	}
}

