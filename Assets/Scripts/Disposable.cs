using System;

public class Disposable : IDisposable
{
    public static IDisposable Create(KeyAction action)
    {
        return null;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}