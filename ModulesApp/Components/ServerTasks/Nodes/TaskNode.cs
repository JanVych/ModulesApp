using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Services;
using MudBlazor;

namespace ModulesApp.Components.ServerTasks.Nodes;

public abstract class TaskNode : NodeModel, IDbNode
{
    public NodeType Type { get; set; }
    public int SubType { get; set; }
    public NodeInputType InputType { get; set; } = NodeInputType.None;

    public readonly ContextService _context;

    public double PositionX => Position.X;
    public double PositionY => Position.Y;

    public string StringVal1 { get; set; } = string.Empty;
    public string StringVal2 { get; set; } = string.Empty;
    public string StringVal3 { get; set; } = string.Empty;
    public double DoubleVal1 { get; set; }
    public double DoubleVal2 { get; set; }
    public bool BoolVal1 { get; set; }
    public bool BoolVal2 { get; set; }
    public long LongVal1 { get; set; }
    public long LongVal2 { get; set; }
    public long LongVal3 { get; set; }

    // not in db
    public bool ShowIdentifier { get; set; } = false;

    public TaskNode(ContextService context, Point? position = null) : base(position)
    {
        _context = context;
    }

    public TaskNode(ContextService context, DbTaskNode dbNode) :base(new Point(dbNode.PositionX, dbNode.PositionY))
    {
        _context = context;
        Order = dbNode.Order;
        Type = dbNode.Type;
        SubType = dbNode.SubType;
        InputType = dbNode.InputType;
        
        StringVal1 = dbNode.StringVal1;
        StringVal2 = dbNode.StringVal2;
        StringVal3 = dbNode.StringVal3;
        DoubleVal1 = dbNode.DoubleVal1;
        DoubleVal2 = dbNode.DoubleVal2;
        LongVal1 = dbNode.LongVal1;
        LongVal2 = dbNode.LongVal2;
        LongVal3 = dbNode.LongVal3;
        BoolVal1 = dbNode.BoolVal1;
        BoolVal2 = dbNode.BoolVal2;
    }

    public void RemoveAllInputPorts()
    {
        foreach (var port in Ports.ToList())
        {
            if (port is TaskPort taskPort && taskPort.Input)
            {
                taskPort.RemoveAllLinks();
                RemovePort(port);
            }
        }
    }

    public void RemoveAllOutputPorts()
    {
        foreach (var port in Ports.ToList())
        {
            if (port is TaskPort taskPort && !taskPort.Input)
            {
                taskPort.RemoveAllLinks();
                RemovePort(port);
            }
        }
    }

    public static string GertNodeButtonStyle(NodeType type)
    {
        var style = $"background-color: var({GetNodeColorString(type)}); color: var(--mud-palette-surface);";
        //if (type is NodeType.FromMessage or NodeType.FromAny or NodeType.Value
        //    or NodeType.DateTime or NodeType.DataDisplay or NodeType.SendMessage
        //    or NodeType.ArithmeticSaturation)
        //{
        //    style += " color: var(--mud-palette-surface);";
        //}
        return style;
    }

    public static string GetNodeColorString(NodeType type)
    {
        return type switch
        {
            NodeType.FromMessage => "--mud-palette-primary-darken",
            NodeType.FromAny => "--mud-palette-primary-darken",
            NodeType.Value => "--mud-palette-primary-darken",
            NodeType.DateTime => "--mud-palette-primary-darken",
            NodeType.DataDisplay => "--mud-palette-success-darken",
            NodeType.SendMessage => "--mud-palette-success-darken",
            NodeType.Condition => "--mud-palette-info-darken",
            NodeType.ArrayOperation => "--mud-palette-tertiary-darken",
            NodeType.ArithmeticOperation => "--mud-palette-tertiary-darken",
            NodeType.ConvertTo => "--mud-palette-tertiary-darken",
            NodeType.BooleanOperation => "--mud-palette-tertiary-darken",
            NodeType.ArithmeticSaturation => "--mud-palette-tertiary-darken",
            _ => "--mud-palette-info-darken"
        };
    }


    public static string GetNodeIconString(NodeType type)
    {
        return type switch
        {
            NodeType.FromMessage => Icons.Material.Filled.Message,
            NodeType.FromAny => Icons.Material.Filled.Message,
            NodeType.Value => Icons.Material.Filled.Numbers,
            NodeType.DateTime => Icons.Material.Filled.CalendarMonth,
            NodeType.DataDisplay => Icons.Material.Filled.DashboardCustomize,
            NodeType.SendMessage => Icons.Material.Filled.Send,
            NodeType.Condition => string.Empty,
            NodeType.ArrayOperation => Icons.Material.Filled.DataArray,
            NodeType.ArithmeticSaturation => Icons.Material.Filled.VerticalAlignCenter,
            _ => string.Empty
        };
    }
}