using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace ModulesApp.Components.ServerTasks.Ports;

public enum PortPositionAlignment
{
    Start,
    Center,
    End,
}

public enum PortDataType
{
    None,
    String,
    Number,
    Boolean,
    Array,
    Json,
}

public class TaskPort : PortModel
{
    public bool Data { get; set; }
    public bool Input { get; set; } = false;
    public PortPositionAlignment PositionAlignment { get; set; }

    public TaskPort(NodeModel parent, bool input, PortPositionAlignment positionAlignment, Point? position = null, Size? size = null, bool data = false) 
        : base(parent, input ? PortAlignment.Left : PortAlignment.Right, position, size)
    {
        Data = data;
        Input = input;
        PositionAlignment = positionAlignment;
    }

    public override bool CanAttachTo(ILinkable other)
    {
        if (!base.CanAttachTo(other))
            return false;

        if (other is not TaskPort otherPort)
            return false;

        //not data port, change link style
        if (!otherPort.Data)
        {
            if (Links.FirstOrDefault() is LinkModel link)
            {
                link.Color = "grey";
                link.Width = 2;
            }
        }
        
        // Input ports can have only one link
        if (otherPort.Input && otherPort.Links.Count != 0)
            return false;
        if (Input && Links.Count != 1)
            return false;

        // Only link Ins with Outs
        return Input != otherPort.Input;
    }

    //public void GetTargetPort()
    //{
    //    if(Input && Links.Count != 0)
    //    {
    //        if(Links[0].Source is SinglePortAnchor anchor)
    //        {
    //            var targetPort = anchor.Port as DiagramPort;
    //        }
    //    }
    //}

    public void RemoveAllLinks()
    {
        foreach (var link in Links.ToList())
        {
            link.Diagram?.Links.Remove(link);
        }
    }
}
