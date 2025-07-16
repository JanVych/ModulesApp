using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Interfaces;
using ModulesApp.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModulesApp.Models.ServerTasks;

[Table("TaskNode")]
public abstract class DbTaskNode : IDbNode
{
    [Key]
    public long Id { get; set; }

    public NodeType Type { get; set; }
    public int SubType { get; set; }
    public NodeInputType InputType { get; set; } = NodeInputType.None;

    public string StringVal1 { get; set; } = string.Empty;
    public string StringVal2 { get; set; } = string.Empty;
    public string StringVal3 { get; set; } = string.Empty;
    public double DoubleVal1 { get; set; }
    public double DoubleVal2 { get; set; } = 0;
    public long LongVal1 { get; set; }
    public long LongVal2 { get; set; }
    public long LongVal3 { get; set; } = 0;
    public bool BoolVal1 { get; set; }
    public bool BoolVal2 { get; set; } = false;

    public double PositionX { get; set; } = 0;
    public double PositionY { get; set; } = 0;
    public int Order { get; set; }

    public ICollection<DbTaskLink> SourceLinks { get; set; } = [];
    public ICollection<DbTaskLink> TargetLinks { get; set; } = [];

    public long TaskId { get; set; }
    [ForeignKey("TaskId")]
    public DbTask Task { get; set; } = default!;

    [NotMapped]
    public NodeValue Value { get; set; } = new NodeValue.Waiting();

    [NotMapped]
    public bool IsProcessed { get; private set; } = false;

    public DbTaskNode(TaskNode node)
    {
        Type = node.Type;
        SubType = node.SubType;
        InputType = node.InputType;

        StringVal1 = node.StringVal1;
        StringVal2 = node.StringVal2;
        StringVal3 = node.StringVal3;
        DoubleVal1 = node.DoubleVal1;
        DoubleVal2 = node.DoubleVal2;
        LongVal1 = node.LongVal1;
        LongVal2 = node.LongVal2;
        LongVal3 = node.LongVal3;
        BoolVal1 = node.BoolVal1;
        BoolVal2 = node.BoolVal2;

        PositionX = node.PositionX;
        PositionY = node.PositionY;

        Order = node.Order;
    }
    public DbTaskNode(){}

    public virtual NodeValue GetValue(DbTaskLink link, ContextService context)
    {
        if (Value.Type == NodeValueType.Waiting)
        {
            if (IsProcessed)
            {
                Value = new NodeValue.InvalidValue($"In node {Order}, process error: cycle detetcted!");
            }
            else
            {
                IsProcessed = true;
                Process(context);
                IsProcessed = false;
            }    
        }
        return Value;
    }

    public virtual void Process(ContextService context)
    {
        throw new NotImplementedException();
    }

    public NodeValue GetInputValue(ContextService context, PortPositionAlignment position, string portName = "")
    {
        DbTaskLink? link = TargetLinks.FirstOrDefault(l => l.TargetPositionAlignment == position);
        if (link == null)
        {
            return new NodeValue.InvalidValue($"In node: {Order}, no {portName} input value!");
        }
        return link.GetValue(context);
    }

    public NodeValue GetInputLeftValue(ContextService context)
    {
        if (InputType == NodeInputType.Double)
        {
            return GetInputValue(context, PortPositionAlignment.Top, "left");
        }
        else
        {
            return GetInputValue(context, PortPositionAlignment.Center, "left");
        }
    }
}
