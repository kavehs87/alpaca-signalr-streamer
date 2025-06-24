using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Parquet;
using Parquet.Data;
using System.Linq;
using Parquet.Schema;

public class AlpacaWebSocketClient
{
    private readonly List<Quote> _quotesBuffer = new List<Quote>();
    private readonly List<Bar> _barsBuffer = new List<Bar>();
    private readonly string _parquetFilePath = "./output/tick_data.parquet"; // Adjust this path

    public async Task HandleIncomingMessage(string jsonData)
    {
        var settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        var messageType = JsonConvert.DeserializeObject<dynamic>(jsonData)[0].T;

        if (messageType == "q")
        {
            var quotes = JsonConvert.DeserializeObject<List<Quote>>(jsonData, settings);
            foreach (var quote in quotes)
            {
                await HandleQuoteAsync(quote);
            }
        }
        else if (messageType == "b")
        {
            var bars = JsonConvert.DeserializeObject<List<Bar>>(jsonData, settings);
            foreach (var bar in bars)
            {
                await HandleBarAsync(bar);
            }
        }
    }

    public async Task HandleQuoteAsync(Quote quote)
    {
        _quotesBuffer.Add(quote);
        if (_quotesBuffer.Any() && ShouldWriteHourlyData(_quotesBuffer.First().Tt))
        {
            await WriteParquetFileAsync();
            _quotesBuffer.Clear();
        }

        Console.WriteLine($"Quotes Buffer Size: {_quotesBuffer.Count}");
    }

    public async Task HandleBarAsync(Bar bar)
    {
        _barsBuffer.Add(bar);
        if (_barsBuffer.Any() && ShouldWriteHourlyData(_barsBuffer.First().Tt))
        {
            await WriteParquetFileAsync();
            _barsBuffer.Clear();
        }

        Console.WriteLine($"Bars Buffer Size: {_barsBuffer.Count}");
    }

    private bool ShouldWriteHourlyData(DateTime firstTimestamp)
    {
        Console.WriteLine($"First Timestamp: {firstTimestamp}, Current Time: {DateTime.UtcNow}");
        if (firstTimestamp == DateTime.MinValue)
        {
            return false;
        }

        return (DateTime.UtcNow - firstTimestamp).TotalMinutes >= 60;
    }

    private async Task WriteParquetFileAsync()
    {
        string directory = Path.GetDirectoryName(_parquetFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Write quotes data
        if (_quotesBuffer.Any())
        {
            string quotesFilePath = Path.Combine(directory, $"quotes_{DateTime.UtcNow:yyyyMMddHHmmss}.parquet");

            using (Stream fileStream = File.Create(quotesFilePath))
            {
                var tickerField = new DataField<string>("Ticker");

                var bidExchangeField = new DataField<string>("BidExchange");
                var bidPriceField = new DataField<decimal>("BidPrice");
                var bidSizeField = new DataField<int>("BidSize");

                var askExchangeField = new DataField<string>("AskExchange");
                var askPriceField = new DataField<decimal>("AskPrice");
                var askSizeField = new DataField<int>("AskSize");

                var timestampField = new DataField<DateTime>("Timestamp");

                var schema = new ParquetSchema(tickerField, bidExchangeField, bidPriceField, bidSizeField, askExchangeField, askPriceField, askSizeField, timestampField);

                using (ParquetWriter parquetWriter = await ParquetWriter.CreateAsync(schema, fileStream))
                {
                    using (ParquetRowGroupWriter groupWriter = parquetWriter.CreateRowGroup())
                    {
                        await groupWriter.WriteColumnAsync(new DataColumn(tickerField, _quotesBuffer.Select(q => q.S).ToArray()));

                        await groupWriter.WriteColumnAsync(new DataColumn(bidExchangeField, _quotesBuffer.Select(q => q.Bx).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(bidPriceField, _quotesBuffer.Select(q => q.Bp).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(bidSizeField, _quotesBuffer.Select(q => q.Bs).ToArray()));

                        await groupWriter.WriteColumnAsync(new DataColumn(askExchangeField, _quotesBuffer.Select(q => q.Ax).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(askPriceField, _quotesBuffer.Select(q => q.Ap).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(askSizeField, _quotesBuffer.Select(q => q.As).ToArray()));

                        await groupWriter.WriteColumnAsync(new DataColumn(timestampField, _quotesBuffer.Select(q => q.Tt).ToArray()));
                    }
                }
            }

            Console.WriteLine("Quotes Parquet file written successfully.");
        }

        // Write bars data
        if (_barsBuffer.Any())
        {
            string barsFilePath = Path.Combine(directory, $"bars_{DateTime.UtcNow:yyyyMMddHHmmss}.parquet");

            using (Stream fileStream = File.Create(barsFilePath))
            {
                var tickerField = new DataField<string>("Ticker");
                var openField = new DataField<decimal>("Open");
                var highField = new DataField<decimal>("High");
                var lowField = new DataField<decimal>("Low");
                var closeField = new DataField<decimal>("Close");
                var volumeField = new DataField<long>("Volume");
                var timestampField = new DataField<DateTime>("Timestamp");

                var schema = new ParquetSchema(tickerField, openField, highField, lowField, closeField, volumeField,
                    timestampField);

                using (ParquetWriter parquetWriter = await ParquetWriter.CreateAsync(schema, fileStream))
                {
                    using (ParquetRowGroupWriter groupWriter = parquetWriter.CreateRowGroup())
                    {
                        await groupWriter.WriteColumnAsync(new DataColumn(tickerField,
                            _barsBuffer.Select(b => b.S).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(openField,
                            _barsBuffer.Select(b => b.O).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(highField,
                            _barsBuffer.Select(b => b.H).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(lowField,
                            _barsBuffer.Select(b => b.L).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(closeField,
                            _barsBuffer.Select(b => b.C).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(volumeField,
                            _barsBuffer.Select(b => b.V).ToArray()));
                        await groupWriter.WriteColumnAsync(new DataColumn(timestampField,
                            _barsBuffer.Select(b => b.Tt).ToArray()));
                    }
                }
            }

            Console.WriteLine("Bars Parquet file written successfully.");
        }
    }
}