using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace ModulesApp.Components.ServerTasks.Ports;

public class TaskPort : PortModel
{
    public int Order { get; set; }
    public bool Input { get; set; } = false;
    public bool Data { get; set; } = false;

    public TaskPort(NodeModel parent, bool input, int order, Point? position = null, Size? size = null, bool data = false) 
        : base(parent, input ? PortAlignment.Left : PortAlignment.Right, position, size)
    {
        Input = input;
        Order = order;
        Data = data;
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
}
