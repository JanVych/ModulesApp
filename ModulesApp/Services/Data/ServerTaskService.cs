using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Microsoft.EntityFrameworkCore;
using ModulesApp.Components.ServerTasks.Nodes;
using ModulesApp.Components.ServerTasks.Ports;
using ModulesApp.Data;
using ModulesApp.Interfaces;
using ModulesApp.Models;
using ModulesApp.Models.BackgroundServices;
using ModulesApp.Models.ServerTasks;
using ModulesApp.Models.ServerTasks.Nodes;

namespace ModulesApp.Services.Data;

public class ServerTaskService
{
    private readonly ServerContextService _serverContext;
    private readonly IDbContextFactory<SQLiteDb> _dbContextFactory;

    public ServerTaskService(IDbContextFactory<SQLiteDb> dbContextFactory, ServerContextService serverContext)
    {
        _dbContextFactory = dbContextFactory;
        _serverContext = serverContext;
    }

    public async Task Delete(DbTask task)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();
    }

    public async Task<List<DbTask>> GetListAsync()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Tasks
            .Include(t => t.Module)
            .Include(t => t.BackgroundService)
            .ToListAsync();
    }

    public async Task<List<DbTask>> GetListAsync(DbModule module)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Tasks
            .Where(t => t.ModuleId == module.Id)
            .Include(t => t.Module)
            .ToListAsync();
    }

    public async Task<List<DbTask>> GetListAsync(DbBackgroundService service)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        return await context.Tasks
            .Where(t => t.BackgroundServiceId == service.Id)
            .Include(t => t.BackgroundService)
            .ToListAsync();
    }

    public async Task<List<DbTaskNode>> GetAllTaskNodes(DbTask task)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.TaskNodes
            .Where(n => n.TaskId == task.Id)
            .Include(n => n.Task)
            .Include(n => n.SourceLinks)
            .Include(n => n.TargetLinks)
            .ToListAsync();
    }

    public async Task<List<DbTaskLink>> GetAllTaskLinks(long taskId)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return await context.TaskLinks
            .Where(l => l.Source.TaskId == taskId)
            .Include(l => l.Source)
            .Include(l => l.Target)
            .ToListAsync();
    }

    public async Task AddAsync(DbTask task)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        await context.Tasks.AddAsync(task);
        await context.SaveChangesAsync();
    }

    public async Task Update(DbTask task)
    {
        using var context = _dbContextFactory.CreateDbContext();

        context.TaskNodes.RemoveRange(context.TaskNodes.Where(n => n.TaskId == task.Id));

        context.Tasks.Update(task);

        await context.SaveChangesAsync();
    }

    public async Task Save(DbTask dbTask, BlazorDiagram diagram)
    {
        var nodes = diagram.Nodes;
        var links = diagram.Links;

        List<DbTaskNode> dbNodes = [];
        List<DbTaskLink> dbLinks = [];

        foreach (TaskNode node in nodes)
        {
            if (node is ConditionNode conditionNode)
            {
                dbNodes.Add(new DbConditionNode(conditionNode));
            }
            else if (node is FromMessageNode fromMessageNode)
            {
                dbNodes.Add(new DbFromMessageNode(fromMessageNode));
            }
            else if (node is DataDisplayNode dataDisplayNode)
            {
                dbNodes.Add(new DbDataDisplayNode(dataDisplayNode));
            }
            else if (node is ValueNode staticDataNode)
            {
                dbNodes.Add(new DbValueNode(staticDataNode));
            }
            else if (node is SendMessageNode sendMessageNode)
            {
                dbNodes.Add(new DbSendMessageNode(sendMessageNode));
            }
            else if (node is ArrayOperationNode arrayOperationNode)
            {
                 dbNodes.Add(new DbArrayOperationNode(arrayOperationNode));
            }
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

        dbTask.Nodes = dbNodes;
        await Update(dbTask);
    }

    public async Task ProcessNodes(IServerContext serverContext , DbTask task)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var nodes = await GetAllTaskNodes(task);

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

    public async Task ProcessNodes(IServerContext serverContext, DbModule module)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var tasks  = await GetListAsync(module);
        foreach (var task in tasks)
        {
            var nodes = await GetAllTaskNodes(task);

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
    }

    public async Task ProcessNodes(DbBackgroundService service)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var tasks = await GetListAsync(service);
        foreach (var task in tasks)
        {
            var nodes = await GetAllTaskNodes(task);

            foreach (var node in nodes)
            {
                if ((node.Type == NodeType.DataDisplay || node.Type == NodeType.SendMessage) && node.Value.Type == NodeValueType.Waiting)
                {
                    //node.Process(serverContext);
                    var value = node.GetValue(null, _serverContext);
                    Console.WriteLine($"Node: {node.Order}, Value: {value}");
                }
            }
        }
    }
}

