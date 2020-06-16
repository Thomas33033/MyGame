using System.Collections.Generic;

public class GyObjectPool<T> where T : class, new()
{
    private Stack<T> m_objectStack = new Stack<T>();

    public T New()
    {
        lock (m_objectStack)
        {
            return (m_objectStack.Count == 0) ? new T() : m_objectStack.Pop();
        }
    }

    public void Store(T t)
    {
        lock (m_objectStack)
        {
            m_objectStack.Push(t);
        }
    }
}

