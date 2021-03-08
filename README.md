# Serializable Dictionary

## About This Package
This package is based off of the great work of SerializableDictionary from  Fredrik Ludvigsen (Steinbitglis) on the [Unity Community Wiki](http://wiki.unity3d.com/index.php/SerializableDictionary).

You can now serialize C# type `Dictionary` in Unity with no boilerplate per new type.

## Usage
```cs
using System.Collections.Generic;
using System.Linq;
 
using UnityEngine;
 
public class Example : MonoBehaviour
{
    public SerializableDictionary<string, string>     names;
    public SerializableDictionary<string, GameObject> gameObjects;
    public SerializableDictionary<string, Color>      colors;
}
```
