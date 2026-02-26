using Server.Utility;

namespace Server.ServiceRealisation;

public static class ServiceManager
{
    private static bool _isCreated;
    private static readonly List<object> Services = [];

    public static void InitAll()
    {
        foreach (var service in Services)
        {
            var type = service.GetType();
            if (!type.IsAssignableTo(typeof(IInitializable))) 
                continue;
            
            ((IInitializable) service).Init();
        }
    }

    public static void StartAll()
    {
        foreach (var service in Services)
        {
            var type = service.GetType();
            if (!type.IsAssignableTo(typeof(IStartable))) 
                continue;

            async Task Start()
            {
                try
                {
                    await ((IStartable)service).Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            _ = Start();
        }
    }
    
    public static void CreateAll()
    {
        if (_isCreated) 
            return;
        
        _isCreated = true;

        var types = new List<Type>();
        
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass) 
                    continue;
                
                if (!Attribute.IsDefined(type, typeof(ServiceAttribute))) 
                    continue;

                DependencyManager.Container.Register(type);
                types.Add(type);
            }
        }
        
        DependencyManager.Container.ResolveAll();

        foreach (var ctor in types)
        {
            var service = DependencyManager.Container.Resolve(ctor);
            Services.Add(service);
        }
    }
}