using System;

namespace Net.Bluewalk.DotNetEnvironmentExtensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EnvironmentVariable : Attribute
    {
        public string Name { get; set; }
        public object Default { get; set; }
    }
}
