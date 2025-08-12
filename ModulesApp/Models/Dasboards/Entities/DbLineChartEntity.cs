using ModulesApp.Helpers;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace ModulesApp.Models.Dasboards.Entities;

public partial class DbLineChartEntity :DbDashboardEntity
{
    public class Series
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; } = 0;
        public List<SeriesData> Data { get; set; } = [];
        public void AddDataWithMovingAverage(decimal valueY, DateTime valueX, int windowSize = 6)
        {
            int startIndex = Math.Max(0, Data.Count - (windowSize - 1));
            var recentPoints = Data.Skip(startIndex).Select(d => d.ValueY).ToList();

            recentPoints.Add(valueY);
            decimal smoothedValue = recentPoints.Average();

            Data.Add(new SeriesData
            {
                ValueY = smoothedValue,
                ValueX = valueX
            });
        }
    }
    public class SeriesData
    {
        public decimal ValueY { get; set; } = 0;
        public DateTime ValueX { get; set; } = DateTime.Now;
    }

    public string Title = string.Empty;

    public int RoundValueTo = 0;
    public int MovingAverageWindowSize = 6;

    public EntityChartType ChartType = EntityChartType.MovingAverage24Hours;

    [NotMapped]
    public List<Series> SeriesList { get; set; } = [];

    public event Func<Task>? UpdateAsync;


    [GeneratedRegex(@"Value(\d+)")]
    private static partial Regex ValueRegex();

    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        Match match = ValueRegex().Match(key);
        if (match.Success)
        {
            int serieId = int.Parse(match.Groups[1].Value);
            var valueDecimal = DataConvertor.ToDecimal(value);
            var timestamp =  DateTime.Now;


            if (toDatabse)
            {
                if (Data.TryGetValue($"SeriesDataX{serieId}", out var dataX))
                {
                    var dataXList = DataConvertor.ToList<DateTime>(dataX);
                    dataXList.Add(timestamp);
                    Data[$"SeriesDataX{serieId}"] = dataXList;
                }
                if (Data.TryGetValue($"SeriesDataY{serieId}", out var datY))
                {
                    var dataYList = DataConvertor.ToList<decimal>(datY);
                    dataYList.Add(valueDecimal);
                    Data[$"SeriesDataY{serieId}"] = dataYList;
                }
            }
            else
            {
                var serie = SeriesList.FirstOrDefault(x => x.Id == serieId);
                if (serie != null)
                {
                    var threshold = DateTime.Now.AddHours(-24);
                    serie.Data.RemoveAll(x => x.ValueX < threshold);
                    serie.AddDataWithMovingAverage(valueDecimal, timestamp, MovingAverageWindowSize);
                    UpdateAsync?.Invoke();
                }
            }
        }
        else
        {
            if (Data.TryGetValue("MovingAverageWindowSize", out var ma))
            {
                MovingAverageWindowSize = DataConvertor.ToInt32(ma);
            }
            if (Data.TryGetValue("RoundValueTo", out var rv))
            {
                RoundValueTo = DataConvertor.ToInt32(rv);
            }
            if (Data.TryGetValue("Title", out var t))
            {
                Title = DataConvertor.ToString(t);
            }
        }
    }

    public override void LoadState()
    {
        List<int> SeriesIds = [];
        if (Data.TryGetValue("Title", out var t))
        {
            Title = DataConvertor.ToString(t);
        }
        if (Data.TryGetValue("Type", out var ty))
        {
            ChartType = (EntityChartType)DataConvertor.ToInt32(ty);
        }
        if (Data.TryGetValue("SeriesIds", out var sn))
        {
            SeriesIds = DataConvertor.ToList<int>(sn);
        }
        if (Data.TryGetValue("MovingAverageWindowSize", out var ma))
        {
            MovingAverageWindowSize = DataConvertor.ToInt32(ma);
        }
        if (Data.TryGetValue("RoundValueTo", out var rv))
        {
            RoundValueTo = DataConvertor.ToInt32(rv);
        }


        foreach (var numebr in SeriesIds)
        {
            if (Data.TryGetValue($"SeriesName{numebr}", out var name))
            {
                if (Data.TryGetValue($"SeriesDataX{numebr}", out var dataX))
                {
                    if (Data.TryGetValue($"SeriesDataY{numebr}", out var dataY))
                    {

                        var list = new List<SeriesData>();
                        var xValues = DataConvertor.ToList<DateTime>(dataX);
                        var threshold = DateTime.Now.AddHours(-24);
                        xValues.RemoveAll(x => x < threshold);

                        var yValues = DataConvertor.ToList<decimal>(dataY);

                        for (int i = 0; i < xValues.Count; i++)
                        {
                            list.Add(new SeriesData
                            {
                                ValueX = xValues[i],
                                ValueY = yValues[i]
                            });
                        }
                        SeriesList.Add(new Series
                        {
                            Id = numebr,
                            Name = DataConvertor.ToString(name),
                            Data = list
                        });
                    }
                        
                }
            }
        }
    }

    public override void SaveToData()
    {
        Data["Title"] = Title;
        Data["SeriesIds"] = SeriesList.Select(x => x.Id);
        Data["Type"] = (int)ChartType;
        Data["MovingAverageWindowSize"] = MovingAverageWindowSize;
        Data["RoundValueTo"] = RoundValueTo;

        foreach (var s in SeriesList)
        {
            Data[$"SeriesName{s.Id}"] = s.Name;
            Data[$"SeriesDataX{s.Id}"] = s.Data.Select(x => x.ValueX);
            Data[$"SeriesDataY{s.Id}"] = s.Data.Select(x => x.ValueY);
        }
    }
}