using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<(T, int)> elements = new List<(T, int)>(); //元素->优先级
    public int Count { get { return elements.Count; } }

    public void Push(T item,int priority)
    {
        elements.Add((item, priority));
    }

    public T Pop()
    {
        if (elements.Count == 0) throw new System.InvalidOperationException("队列为空");

        int index = 0;
        for(int i=0; i<elements.Count; i++)
        {
            if (elements[i].Item2 < elements[index].Item2) index = i;
        }

        T res = elements[index].Item1;
        elements.RemoveAt(index);
        return res;
    }

    public bool Contains(T item)
    {
        foreach(var element in elements)
        {
            if(element.Item1.Equals(item)) return true;
        }
        return false;
    }
    
    public void UpdatePriority(T item,int newPriority)
    {
        for(int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Item1.Equals(item))
            {
                elements[i] = (item, newPriority);
                return;
            }
        }
    }
}
