using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Connection
{
    public System.Guid from;
    public System.Guid to;
    public ConnectionRenderer renderer;
}


public class ConnectionManager : MonoBehaviour
{
    public System.Guid[] completeState = { };
    private System.Guid currentInstanceId;
    private Connection[] connections = { };
    private Dictionary<System.Guid, MemoryInstance> instances = new();

    public void Add(Connection conn)
    {
        connections = connections.Append(conn).ToArray();
    }

    public void DropCurrentConnection()
    {
        DropConnection(connections[^1]);
    }

    public Connection[] GetConnectionsOf(System.Guid id)
    {
        Connection[] connections = { };
        foreach (Connection connection in this.connections)
        {
            if (connection.from == id || connection.to == id)
            {
                connections = connections.Append(connection).ToArray();
            }
        }
        return connections;
    }

    public void DropConnection(Connection connection)
    {
        Destroy(connection.renderer.gameObject);
        List<Connection> list = connections.ToList();
        connections = list.Where(c => c.from != connection.from && c.to != connection.to).ToArray();
    }

    public Connection[] GetConnections()
    {
        return connections;
    }

    public void SetCurrentInstance(System.Guid id)
    {
        currentInstanceId = id;
    }

    public bool IsHolding()
    {
        if (connections.Length == 0)
        {
            return false;
        }
        return connections[^1].to == System.Guid.Empty;
    }

    public void ClearCurrentInstance()
    {
        currentInstanceId = System.Guid.Empty;
    }

    public MemoryInstance GetCurrentInstance()
    {
        return GetInstance(currentInstanceId);
    }

    public Connection GetCurrentConnection()
    {
        return connections[^1];
    }

    public MemoryInstance GetInstance(System.Guid id)
    {
        if (!instances.ContainsKey(id))
        {
            return null;
        }
        return instances[id];
    }

    public void Register(System.Guid id, MemoryInstance instance)
    {
        instances.Add(id, instance);
    }

    public bool HasConnection(System.Guid id)
    {
        foreach (var connection in connections)
        {
            if (connection.from == id || connection.to == id)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasCompleteConnection(System.Guid id)
    {
        foreach (var connection in connections)
        {
            if ((connection.from == id || connection.to == id) && connection.to != System.Guid.Empty)
            {
                return true;
            }
        }
        return false;
    }

    public Connection Create(System.Guid from, System.Guid to, Transform fallback)
    {
        GameObject obj = new("Connection", typeof(LineRenderer), typeof(ConnectionRenderer));
        ConnectionRenderer renderer = obj.GetComponent<ConnectionRenderer>();
        MemoryInstance start = GetInstance(from);
        renderer.start = start.transform;
        MemoryInstance end = GetInstance(to);
        if (end == null)
        {
            renderer.end = fallback;
        }
        else
        {
            renderer.end = end.transform;
        }
        obj.transform.SetParent(transform);

        Connection conn = new()
        {
            from = from,
            to = to,
            renderer = renderer,
        };
        return conn;
    }

    public bool CheckComplete()
    {
        return false;
    }
}

