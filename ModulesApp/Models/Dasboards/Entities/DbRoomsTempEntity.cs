using ModulesApp.Helpers;
using MudBlazor;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.Dasboards.Entities;

public class DbRoomsTempEntity : DbDashboardEntity
{
    public class Room
    {
        public decimal? CurrentTemp { get; set; }
        public decimal TargetTemp { get; set; } = 20;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = Icons.Material.Filled.Living;
        public bool Disabled { get; set; } = false;
    }

    [NotMapped]
    public List<Room> Rooms { get; set; } = [];
    [NotMapped]
    public string Title { get; set; } = string.Empty;

    public override void UpdateState(string key, object? value, bool toDatabse)
    {
        if (toDatabse)
        {
            Data[key] = value;
            return;
        }
        if (key == "RoomsNames")
        {
            var list = DataConvertor.ToList<string>(value);

            for(int i = 0; i < list.Count; i++)
            {
                if(i < Rooms.Count)
                {
                    Rooms[i].Name = list[i] ?? string.Empty;
                }
                else
                {
                    Rooms.Add(new Room { Name = list[i] ?? string.Empty });
                }
            }
        }
        else if(key == "CurrentTemps")
        {
            var list = DataConvertor.ToList<decimal?>(value);
            for(int i = 0; i < list.Count; i++)
            {
                if(i < Rooms.Count)
                {
                    Rooms[i].CurrentTemp = list[i];
                }
            }
        }
        else if(key == "TargetTemps")
        {
            var list = DataConvertor.ToList<decimal>(value);
            for(int i = 0; i < list.Count; i++)
            {
                if(i < Rooms.Count)
                {
                    Rooms[i].TargetTemp = list[i];
                }
            }
        }
        else if(key == "RoomsIcons")
        {
            var list = DataConvertor.ToList<string>(value);
            for(int i = 0; i < list.Count; i++)
            {
                if(i < Rooms.Count)
                {
                    Rooms[i].Icon = list[i] ?? string.Empty;
                }
            }
        }
        else if(key == "Title")
        {
            Title = DataConvertor.ToString(value) ?? string.Empty;
        }
    }

    public override void SaveToData()
    {
        Data["RoomsNames"] = Rooms.Select(i => i.Name).ToList();
        Data["CurrentTemps"] = Rooms.Select(i => i.CurrentTemp).ToList();
        Data["TargetTemps"] = Rooms.Select(i => i.TargetTemp).ToList();
        Data["RoomsIcons"] = Rooms.Select(i => i.Icon).ToList();
        Data["Title"] = Title;
    }

    public override void LoadState()
    {
        if(Data.TryGetValue("Title", out var title))
        {
            Title = DataConvertor.ToString(title) ?? string.Empty;
        }
        if (Data.TryGetValue("RoomsNames", out var roomsNames) &&
               Data.TryGetValue("CurrentTemps", out var currentTemps) &&
               Data.TryGetValue("TargetTemps", out var targetTemps) &&
               Data.TryGetValue("RoomsIcons", out var roomsIcons))
        {
            var roomsNamesList = DataConvertor.ToList<string>(roomsNames);
            var currentTempsList = DataConvertor.ToList<decimal?>(currentTemps);
            var targetTempsList = DataConvertor.ToList<decimal>(targetTemps);
            var roomsIconsList = DataConvertor.ToList<string>(roomsIcons);

            var count = roomsNamesList.Count;
            for (int i = 0; i < count; i++)
            {
                Rooms.Add(new Room
                {
                    Name = roomsNamesList.ElementAtOrDefault(i) ?? string.Empty,
                    CurrentTemp = currentTempsList.ElementAtOrDefault(i),
                    TargetTemp = targetTempsList.ElementAtOrDefault(i),
                    Icon = roomsIconsList.ElementAtOrDefault(i) ?? string.Empty
                });
            }

        }
    }
}
