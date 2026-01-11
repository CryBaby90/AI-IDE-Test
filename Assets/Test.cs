using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//定义一个接口，包含一个init方法，和一个dispose方法
public interface ITest
{
    void Init();
    void Update(); 
    void FixedUpdate();
    void Dispose();
}

//定义一个测试类，实现ITest接口
public class Test : ITest
{
    public void Init()
    {
        Debug.Log("Init");
    }

    public void Update()
    {
        Debug.Log("Update");
    }
    
    public void FixedUpdate()
    {
        Debug.Log("FixedUpdate");
    }
    public float Add(float a, float b, float c)
    {
        return (a + b) * c;
    }

    public void Dispose()
    {
        Debug.Log("Dispose");
    }
}
