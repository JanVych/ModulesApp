using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using ModulesApp.Interfaces;
using ModulesApp.Models.ServerTasks;

namespace ModulesApp.Components.ServerTasks.Ports;

public class TaskPort : PortModel
{
    public NodeValueType DataType { get; set; }
    public bool Input { get; set; } = false;
    public PortPositionAlignment PositionAlignment { get; set; }

    public TaskPort(NodeModel parent, bool input, PortPositionAlignment positionAlignment, Point? position = null, Size? size = null, NodeValueType dataType = NodeValueType.Any) 
        : base(parent, input ? PortAlignment.Left : PortAlignment.Right, position, size)
    {
        DataType = dataType;
        Input = input;
        PositionAlignment = positionAlignment;
    }

    public override bool CanAttachTo(ILinkable other)
    {
        if (!base.CanAttachTo(other))
        {  
            return false; 
        }

        if (other is not TaskPort otherPort)
        {
            return false;
        }

        ////not data port, change link style
        //if (!otherPort.DataType)
        //{
        //    if (Links.FirstOrDefault() is LinkModel link)
        //    {
        //        link.Color = "grey";
        //        link.Width = 2;
        //    }
        //}
        
        // Input ports can have only one link
        if (otherPort.Input && otherPort.Links.Count != 0)
        {
            return false;
        }
        if (Input && Links.Count != 1)
        {
            return false;
        }

        // Only link ins with outs
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
