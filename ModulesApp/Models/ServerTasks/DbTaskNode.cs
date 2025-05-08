using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ModulesApp.Interfaces;
using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Services;
using System.Text.Json;

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
    public long LongVal1 { get; set; }
    public long LongVal2 { get; set; }
    public long LongVal3 { get; set; } = 0;
    public bool BoolVal1 { get; set; }

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
    protected bool Result { get; set; }

    public DbTaskNode(TaskNode node)
    {
        Type = node.Type;
        SubType = node.SubType;
        InputType = node.InputType;

        StringVal1 = node.StringVal1;
        StringVal2 = node.StringVal2;
        StringVal3 = node.StringVal3;
        DoubleVal1 = node.DoubleVal1;
        LongVal1 = node.LongVal1;
        LongVal2 = node.LongVal2;
        LongVal3 = node.LongVal3;
        BoolVal1 = node.BoolVal1;

        PositionX = node.PositionX;
        PositionY = node.PositionY;

        Order = node.Order;
    }
    public DbTaskNode()
    {
    }

    public virtual NodeValue GetValue(DbTaskLink link, ContextService context)
    {
        throw new NotImplementedException();
    }

    public virtual void Process(ContextService context)
    {
        throw new NotImplementedException();
    }

    protected static NodeValue ConvertFromJsonElement(JsonElement element, DbTaskNode node)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => new NodeValue.StringValue(element.GetString() ?? string.Empty),
            JsonValueKind.Number => new NodeValue.NumberValue(element.GetDouble()),
            JsonValueKind.True => new NodeValue.BooleanValue(true),
            JsonValueKind.False => new NodeValue.BooleanValue(false),
            JsonValueKind.Array => new NodeValue.ArrayValue(element.EnumerateArray().Select(e => ConvertFromJsonElement(e, node)).ToList()),
            _ => new NodeValue.InvalidValue($"Invalid value type: {element.ValueKind}, in node: {node.Order}"),
        };
    }

    protected static bool IsValidType(JsonElement element, NodeValueType type)
    {
        return type switch
        {
            NodeValueType.Any => true,
            NodeValueType.String => element.ValueKind == JsonValueKind.String,
            NodeValueType.Number => element.ValueKind == JsonValueKind.Number,
            NodeValueType.Boolean => element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False,
            NodeValueType.Array => element.ValueKind == JsonValueKind.Array,
            _ => false
        };
    }
}
