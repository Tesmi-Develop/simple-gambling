using System.Net;
using Hypercube.Utilities.Dependencies;
using Server.ServiceRealisation;
using Server.Services.DTOHandlers;
using Server.Services.Network;
using Server.Utility;
using Shared;

// ReSharper disable FunctionNeverReturns

namespace Server;

public static class Program
{
    private const string DataName = "Data.json";
    private const int Port = 8000;
    private static readonly DependenciesContainer Container = DependencyManager.Container;
    
    public static void Main()
    {
        PrepareEventBus();
        ServiceManager.CreateAll();
        PrepareDataTracker();
        PrepareNetworkServer();
        
        ServiceManager.InitAll();
        ServiceManager.StartAll();
        
        Console.WriteLine("Server started");
        FreezeThread();
    }

    private static void PrepareEventBus()
    {
        Container.Register<EventBus>();
    }
    
    private static void PrepareNetworkServer()
    {
        var server = Container.Resolve<NetworkServerService>();
        server.Setup(IPAddress.Any, Port);
    }
    
    private static void PrepareDataTracker()
    {
        var dataTracker = Container.Resolve<DataTrackerService>();
        dataTracker.AttachToFile(DataName);
        dataTracker.LoadFromFile(DataName);
    }

    private static void FreezeThread()
    {
        while (true)
        {
            Thread.Sleep(10);
        }
    }
}