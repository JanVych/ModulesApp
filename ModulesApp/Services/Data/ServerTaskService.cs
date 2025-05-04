using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Microsoft.EntityFrameworkCore;
using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Data;
using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;
using ModulesApp.Models.Dasboards;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Models.ServerTasks.Nodes;

namespace ModulesApp.Services.Data;

public class ServerTaskService
{
    private readonly IDbContextFactory<SQLiteDbContext> _dbContextFactory;

    public ServerTaskService(IDbContextFactory<SQLiteDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task DeleteAsync(DbTask task)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
    }

    public async Task<List<DbTask>> GetAllTaskaAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Tasks
            .Include(t => t.Module)
            .Include(t => t.BackgroundService)
            .Include(t => t.DashboardEntity)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<List<DbTask>> GetTasksAsync(DbModule module)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Tasks
            .Where(t => t.ModuleId == module.Id)
            .Include(t => t.Module)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<List<DbTask>> GetTasksAsync(DbBackgroundService service)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Tasks
            .Where(t => t.BackgroundServiceId == service.Id)
            .Include(t => t.BackgroundService)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<List<DbTask>> GetTasksAsync(DbDashboardEntity entity)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Tasks
            .Where(t => t.DashboardEntityId == entity.Id)
            .Include(t => t.DashboardEntity)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<List<DbTaskNode>> GetNodesAsync(DbTask task)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.TaskNodes
            .Where(n => n.TaskId == task.Id)
            .Include(n => n.Task)
            .Include(n => n.SourceLinks)
            .Include(n => n.TargetLinks)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<List<DbTaskLink>> GetLinksAsync(DbTask task)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.TaskLinks
            .Where(l => l.Source.TaskId == task.Id)
            .Include(l => l.Source)
            .Include(l => l.Target)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task AddAsync(DbTask task)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        await context.Tasks.AddAsync(task);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DbTask task)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.TaskNodes.RemoveRange(context.TaskNodes.Where(n => n.TaskId == task.Id));
        context.Tasks.Update(task);
        await context.SaveChangesAsync();
    }

    public async Task SaveDiagramAsync(DbTask task, BlazorDiagram diagram)
    {
        var nodes = diagram.Nodes;
        var links = diagram.Links;

        List<DbTaskNode> dbNodes = [];
        List<DbTaskLink> dbLinks = [];

        foreach (TaskNode node in nodes)
        {
            dbNodes.Add(node.Type switch
            {
                NodeType.Condition => new DbConditionNode((ConditionNode)node),
                NodeType.FromMessage => new DbFromMessageNode((FromMessageNode)node),
                NodeType.DataDisplay => new DbDataDisplayNode((DataDisplayNode)node),
                NodeType.Value => new DbValueNode((ValueNode)node),
                NodeType.SendMessage => new DbSendMessageNode((SendMessageNode)node),
                NodeType.ArrayOperation => new DbArrayOperationNode((ArrayOperationNode)node),
                NodeType.ArithmeticOperation => new DbArithmeticOperationNode(node),
                NodeType.ConvertTo => new DbConvertToNode((ConvertToNode)node),
                NodeType.DateTime => new DbDateTimeNode((DateTimeNode)node),
                NodeType.FromAny => new DbFromAnyNode((FromAnyNode)node),
                _ => throw new ArgumentException($"Unsupported node type: {node.Type}")
            });
        }

        var dbNodeLookup = dbNodes.ToDictionary(node => node.Order);

        foreach (var link in links)
        {
            if (link.Source is SinglePortAnchor sourceAnchor && link.Target is SinglePortAnchor targetAnchor &&
                sourceAnchor.Port is TaskPort sourcePort && targetAnchor.Port is TaskPort targetPort)
            {
                dbNodeLookup.TryGetValue(sourcePort.Parent.Order, out var source);
                dbNodeLookup.TryGetValue(targetPort.Parent.Order, out var target);

                if (source != null && target != null)
                {
                    DbTaskLink newLink;
                    if (sourcePort.Input)
                    {
                        newLink = new(targetPort, target, sourcePort, source);
                        source.TargetLinks.Add(newLink);
                        target.SourceLinks.Add(newLink);
                    }
                    else
                    {
                        newLink = new(sourcePort, source, targetPort, target);
                        source.SourceLinks.Add(newLink);
                        target.TargetLinks.Add(newLink);
                    }

                    dbLinks.Add(newLink);
                }
            }
        }
        task.Nodes = dbNodes;
        await UpdateAsync(task);
    }

    public async Task ExecuteTaskAsync(ContextService serverContext , DbTask task)
    {
        var nodes = await GetNodesAsync(task);

        foreach (var node in nodes)
        {
            if ((node.Type == NodeType.DataDisplay || node.Type == NodeType.SendMessage) && node.Value.Type == NodeValueType.Waiting)
            {
                //node.Process(serverContext);
                var value = node.GetValue(null, serverContext);
                Console.WriteLine($"Node: {node.Order}, Value: {value}");
            }
        }
    }

    public async Task ExecuteTasksAsync(ContextService serverContext, IEnumerable<DbTask> tasks)
    {
        foreach (var task in tasks)
        {
            await ExecuteTaskAsync(serverContext, task);
        }
    }

    public async Task ExecuteTasksAsync(ContextService serverContext, DbModule module)
    {
        var tasks  = await GetTasksAsync(module);
        await ExecuteTasksAsync(serverContext, tasks);
    }

    public async Task ExecuteTasksAsync(ContextService serverContext, DbBackgroundService service)
    {
        var tasks = await GetTasksAsync(service);
        await ExecuteTasksAsync(serverContext, tasks);
    }

    public async Task ExecuteTasksAsync(ContextService serverContext, DbDashboardEntity entity)
    {
        var tasks = await GetTasksAsync(entity);
        await ExecuteTasksAsync(serverContext, tasks);
    }
}

