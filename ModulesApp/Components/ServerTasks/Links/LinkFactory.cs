using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using ModulesApp.Components.ServerTasks.Ports;

namespace ModulesApp.Components.ServerTasks.Links;

public class LinkFactory
{
    public class CustomLinkModel : LinkModel
    {
        public CustomLinkModel(Anchor source, Anchor target) : base(source, target)
        {
            if (target is not PositionAnchor)
            {
                throw new NotImplementedException();
            }
        }
    }

    public static LinkModel CustomLinkFactory(Diagram diagram, ILinkable source, Anchor targetAnchor)
    {
        Console.WriteLine(targetAnchor);
        Anchor source2;
        string color;
        int widht = 1;

        if (source is not TaskPort port2)
        {
            throw new NotImplementedException();
        }
        source2 = new SinglePortAnchor(port2);

        color = "var(--mud-palette-text-secondary)";
        //if (port2.Data)
        //{
        //    color = "var(--mud-palette-text-secondary)";
        //    widht = 2;
        //}
        //else
        //{
        //    color = "var(--mud-palette-text-disabled)";
        //    widht = 2;
        //}

        if(targetAnchor is not PositionAnchor)
        {
            throw new NotImplementedException();
        }
        return new CustomLinkModel(source2, targetAnchor)
        {
            Color = color,
            Width = widht
        };
    }
}
