using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class Quote
{
    [JsonProperty("T")]
    public string T { get; set; }  // Type

    [JsonProperty("S")]
    public string S { get; set; }  // Symbol

    [JsonProperty("bx")]
    public string Bx { get; set; } // Bid Exchange

    [JsonProperty("bp")]
    public decimal Bp { get; set; } // Bid Price

    [JsonProperty("bs")]
    public int Bs { get; set; } // Bid Size

    [JsonProperty("ax")]
    public string Ax { get; set; } // Ask Exchange

    [JsonProperty("ap")]
    public decimal Ap { get; set; } // Ask Price

    [JsonProperty("as")]
    public int As { get; set; } // Ask Size

    [JsonProperty("c")]
    public List<string> C { get; set; } // Conditions

    [JsonProperty("z")]
    public string Z { get; set; } // Tape

    [JsonProperty("t")]
    public DateTime Tt { get; set; } // Timestamp
}