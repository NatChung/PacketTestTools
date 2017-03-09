using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public static class Helper
{
    public static long MoveByte(long value, int pos)
    {
        if (pos != 0)  //移動 0 位時直接返回原值
        {
            int mask = 0x7fffffff; // int.MaxValue = 0x7FFFFFFF 整數最大值
            value >>= 1;           //無符號整數最高位不表示正負但操作數還有有符號的，有符號數右移1位，正數時高位補0，負數時高位補1
            value &= mask;         //和整數最大值進行邏輯與運算，運算後的結果為忽略表示正負值的最高位
            value >>= pos - 1;     //邏輯運算後的值無符號，對無符號的值直接做右移運算，計算剩下的位
        }
        return value;
    }
    public static void PutAll<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> anotherDic)
    {
        foreach (var option in anotherDic)
            dictionary[option.Key] = option.Value;
    }
    public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
        if (dictionary.ContainsKey(key))
            return dictionary[key];

        return default(TValue);
    }

    public static long GetLong(this IPAddress address)
    {
       return BitConverter.ToUInt32(address.GetAddressBytes(), 0);
    }
}