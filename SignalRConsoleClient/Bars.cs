using Newtonsoft.Json;
using System;

public class Bar
{
    [JsonProperty("T")]
    public string T { get; set; }  // Type

    [JsonProperty("S")]
    public string S { get; set; }  // Symbol

    [JsonProperty("o")]
    public decimal O { get; set; } // Open

    [JsonProperty("h")]
    public decimal H { get; set; } // High

    [JsonProperty("l")]
    public decimal L { get; set; } // Low

    [JsonProperty("c")]
    public decimal C { get; set; } // Close

    [JsonProperty("v")]
    public long V { get; set; } // Volume

    [JsonProperty("t")]
    public DateTime Tt { get; set; } // Timestamp

    // Optional additional fields
    [JsonProperty("n")]
    public int N { get; set; } // Number of trades

    [JsonProperty("vw")]
    public decimal Vw { get; set; } // Volume-weighted average price
}