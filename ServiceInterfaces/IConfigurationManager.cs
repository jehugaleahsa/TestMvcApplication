using System;

namespace ServiceInterfaces
{
    public interface IConfigurationManager
    {
        string ConnectionString { get; }
    }
}
