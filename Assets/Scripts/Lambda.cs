using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Lambda : MonoBehaviour
{
    public List<string> list;
    public List<string> result;
    void Start()
    {
        // var result = list.Select(Append); // リスト内のすべての要素に対して，「.txt」を付け加えてリストにする．
        // var result = list.Select(s => s + ".txt"); // ラムダ式でかいた場合．

        Read("hoge.txt", s => s + ".txt"); // ファイルを読み込み，各行に「.txt」を付け加えてリストにする．
        
        foreach (var item in result)
            Debug.Log(item);
    }

    private string Append(string s)
    {
        return s + ".txt";
    }

    public IEnumerable<string> Read(string path, Func<string, string> fx)
    {
        //var result = new List<string>();

        using (var reader = new StreamReader(path))
        {
            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLine();
                result.Add(fx(line)); // stringを受け取ってstringを返すメソッドを引数として取っている．
            }
        }

        return result;
    }
}
