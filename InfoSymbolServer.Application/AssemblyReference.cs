using System.Reflection;

namespace InfoSymbolServer.Application;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
