using System;

namespace Shared
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PathCustomHandler : Attribute
    {
        public string MethodName { get; }
        public PathCustomHandler(string methodName) => MethodName = methodName;
    }
}