using ModulesApp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ModulesApp.Models.Dasboards.Entities;

public partial class DbLineChart24hEntity :DbDashboardEntity
{
    public enum DataFrequencyType
    {
        EveryMinute = 1,
        Every5Minutes = 5,
        Every10Minutes = 10,
        Every30Minutes = 30,
        EveryHour = 60,
    }
    public class SeriesData
    {
        public decimal Value { get; set; }
        public DateTime Time { get; set; }
    }

    public class Series
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Queue<SeriesData> Data { get; set; } = new();
        public List<SeriesData> ReducedData { get; set; } = [];

        public bool AddDataPoint(decimal value, int sampleFrequencyMinutes)
        {
            var dateTime = DateTime.Now;
            Data.Enqueue(new SeriesData { Value = value, Time = dateTime });
            while (Data.Count > 0 && (dateTime - Data.Peek().Time).TotalHours > 24)
            {
                Data.Dequeue();
            }
            if (ReducedData.Count != 0)
            {
                var x = (dateTime - ReducedData.Last().Time).TotalMinutes;
            }
            if (ReducedData.Count == 0 || (dateTime - ReducedData.Last().Time).TotalMinutes >= sampleFrequencyMinutes)
            {
                ReducedData.RemoveAll(x => (dateTime - x.Time).TotalHours > 24);

                var cutoff = dateTime.AddMinutes(- sampleFrequencyMinutes);
                var newValue = Data
                    .Where(d => d.Time >= cutoff)
                    .Select(d => d.Value)
                    .DefaultIfEmpty(0M)
                    .Average();

                ReducedData.Add(new SeriesData { Value = newValue, Time = dateTime });
                return true;
            }
            return false;
        }
    }

    [NotMapped]
    public List<Series> SeriesList { get; set; } = [];
    [NotMapped]
    public string Title { get; set; }  = string.Empty;
    [NotMapped]
    public DataFrequencyType SampleFrequency { get; set; } = DataFrequencyType.EveryMinute;
    [NotMapped]
    public int RoundValuesTo { get; set; } = 2;

    [GeneratedRegex(@"Value(\d+)")]
    private static partial Regex ValueRegex();

    public event Func<Task>? UpdateAsync;

    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        Match match = ValueRegex().Match(key);
        if (match.Success)
        {
            var serieId = int.Parse(match.Groups[1].Value);
            var valueDecimal = DataConvertor.ToDecimal(value);
            Series? serie;

            if(toDatabse)
            {
                serie = TryToLoadSeries(serieId);
            }
            else
            {
                serie = SeriesList.FirstOrDefault(x => x.Id == serieId);
            }
            if (Data.TryGetValue("SampleFrequency", out var sf))
            {
                var sampleFrequency = DataConvertor.ToInt32(sf);
                if (serie != null && serie.AddDataPoint(valueDecimal, sampleFrequency))
                {
                    UpdateAsync?.Invoke();
                }
            }
            
            if(serie != null && toDatabse)
            {
                Data[$"Series{serie.Id}"] = serie;
            }  
        }
        else
        {
            if (key == "Title")
            {
                Title = DataConvertor.ToString(value); 
            }
            else if (key == "SampleFrequency")
            {
                SampleFrequency = (DataFrequencyType)DataConvertor.ToInt32(value);
            }
            else if (key == "RoundValuesTo")
            {
                RoundValuesTo = DataConvertor.ToInt32(value);
            }
            if (toDatabse && key != "SeriesIds")
            {
                Data[key] = value;
            }
        }
    }

    public override void LoadState()
    {
        List<int> seriesIds = [];
        if (Data.TryGetValue("Title", out var t))
        {
            Title = DataConvertor.ToString(t);
        }
        if (Data.TryGetValue("SampleFrequency", out var sf))
        {
            SampleFrequency = (DataFrequencyType)DataConvertor.ToInt32(sf);
        }
        if (Data.TryGetValue("RoundValuesTo", out var rv))
        {
            RoundValuesTo = DataConvertor.ToInt32(rv);
        }
        if (Data.TryGetValue("SeriesIds", out var sIds))
        {
            seriesIds = DataConvertor.ToList<int>(sIds);
        }
        foreach (var id in seriesIds)
        {
            TryToLoadSeries(id);
        }
    }

    public override void SaveToData()
    {
        Data["Title"] = Title;
        Data["SampleFrequency"] = (int)SampleFrequency;
        Data["RoundValuesTo"] = RoundValuesTo;
        Data["SeriesIds"] = SeriesList.Select(x => x.Id);

        foreach (var s in SeriesList)
        {
            Data[$"Series{s.Id}"] = s;
        }
    }

    private Series? TryToLoadSeries(long id)
    {
        if (Data.TryGetValue($"Series{id}", out var obj))
        {
            if (obj is Series serie)
            {
                SeriesList.Add(serie);
                return serie;
            }
            else if (obj is JsonElement jsonSeries)
            {
                var nSerie  = jsonSeries.Deserialize<Series>();
                if (nSerie != null)
                {
                    SeriesList.Add(nSerie);
                    return nSerie;
                }
            }
        }
        return null;
    }
}