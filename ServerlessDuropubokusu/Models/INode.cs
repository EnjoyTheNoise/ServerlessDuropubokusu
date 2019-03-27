using System;

namespace ServerlessDuropubokusu.Models
{
    public interface INode
    {
        Uri Uri { get; set; }
        string Name { get; set; }
    }
}
