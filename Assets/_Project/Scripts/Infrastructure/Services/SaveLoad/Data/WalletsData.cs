using System;
using System.Collections.Generic;

[Serializable]
public class WalletsData : DictionarySerializeContainer<int, long>
{
    public WalletsData(Dictionary<int, long> dictionary) : base(dictionary)
    {
    }
}