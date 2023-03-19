using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Convert2Morse : MonoBehaviour
{
    public string currText;
    private string previousText;
    public List<char> temp = new List<char> ();
    public List<string> result = new List<string>();
    public Text t;
    public InputField inputField;
    void Start()
    {
    }
    private void Update()
    {
        currText = inputField.text;
        if (currText == previousText)
            return;

        temp.Clear ();
        result.Clear ();

        temp.AddRange(currText);
        for (int i = 0; i < temp.Count; i++)
            this.ConvertChar2Morse(temp[i]);
        t.text = string.Join(" ", result.ToArray());
    }
    void ConvertChar2Morse(char c)
    {
        switch (c)
        {
            case 'A': case 'イ': case 'ィ': result.Add("・-");
                break;
            case 'ロ': result.Add("・-・-");
                break;
            case 'B': case 'ハ': result.Add("-・・・");
                break;
            case 'C': case 'ニ': result.Add("-・-・");
                break;
            case 'D': case 'ホ': result.Add("-・・");
                break;
            case 'E': case 'ヘ': result.Add("・");
                break;
            case 'ト': result.Add("・・-・・");
                break;
            case 'F': case 'チ': result.Add("・・-・");
                break;
            case 'G': case 'リ': result.Add("--・");
                break;
            case 'H': case 'ヌ': result.Add("・・・・");
                break;
            case 'ル': result.Add("-・--・");
                break;
            case 'I':  result.Add("・・");
                break;
            case 'J': case 'ヲ': result.Add("・---");
                break;
            case 'K': case 'ワ': result.Add("-・-");
                break;
            case 'L': case 'カ': result.Add("・-・・");
                break;
            case 'M': case 'ヨ': case 'ョ': result.Add("--");
                break;
            case 'N': case 'タ': result.Add("-・");
                break;
            case 'O': case 'レ': result.Add("---");
                break;
            case 'ソ': result.Add("---・");
                break;
            case 'P': case 'ツ': case 'ッ': result.Add("・--・");
                break;
            case 'Q': case 'ネ': result.Add("--・-");
                break;
            case 'R': case 'ナ': result.Add("・-・");
                break;
            case 'S': case 'ラ': result.Add("・・・");
                break;
            case 'T': case 'ム': result.Add("-");
                break;
            case 'U': case 'ウ': case 'ゥ': result.Add("・・-");
                break;
            case 'ヰ': result.Add("・-・・-");
                break;
            case 'ノ': result.Add("・・--");
                break;
            case 'オ': case 'ォ': result.Add("・-・・・");
                break;
            case 'V': case 'ク': result.Add("・・・-");
                break;
            case 'W': case 'ヤ': case 'ャ': result.Add("・--");
                break;
            case 'X': case 'マ': result.Add("-・・-");
                break;
            case 'Y': case 'ケ': result.Add("-・--");
                break;
            case 'Z': case 'フ': result.Add("--・・");
                break;

            case 'コ': result.Add("----");
                break;
            case 'エ': case 'ェ': result.Add("-・---");
                break;
            case 'テ': result.Add("・-・--");
                break;
            case 'ア':　case 'ァ': result.Add("--・--");
                break;
            case 'サ': result.Add("-・-・-");
                break;
            case 'キ': result.Add("-・-・・");
                break;
            case 'ユ': case 'ュ': result.Add("-・・--");
                break;
            case 'メ': result.Add("-・・・-");
                break;
            case 'ミ': result.Add("・・-・-");
                break;
            case 'シ': result.Add("--・-・");
                break;
            case 'ヱ': result.Add("・--・・");
                break;
            case 'ヒ': result.Add("--・・-");
                break;
            case 'モ': result.Add("-・・-・");
                break;
            case 'セ': result.Add("・---・");
                break;
            case 'ス': result.Add("---・-");
                break;
            case 'ン': result.Add("・-・-・");
                break;

            case 'ガ': result.Add("・-・・ ・・");
                break;
            case 'ギ': result.Add("-・-・・ ・・");
                break;
            case 'グ': result.Add("・・・- ・・");
                break;
            case 'ゲ': result.Add("-・-- ・・");
                break;
            case 'ゴ': result.Add("---- ・・");
                break;

            case 'ザ':
                result.Add("-・-・- ・・");
                break;
            case 'ジ':
                result.Add("--・-・ ・・");
                break;
            case 'ズ':
                result.Add("---・- ・・");
                break;
            case 'ゼ':
                result.Add("・---・ ・・");
                break;
            case 'ゾ':
                result.Add("---・ ・・");
                break;

            case 'ダ':
                result.Add("-・ ・・");
                break;
            case 'ヂ':
                result.Add("・・-・ ・・");
                break;
            case 'ヅ':
                result.Add("・--・ ・・");
                break;
            case 'デ':
                result.Add("・-・-- ・・");
                break;
            case 'ド':
                result.Add("・・-・・ ・・");
                break;

            case 'バ':
                result.Add("-・・・ ・・");
                break;
            case 'ビ':
                result.Add("--・・- ・・");
                break;
            case 'ブ':
                result.Add("--・・ ・・");
                break;
            case 'ベ':
                result.Add("・ ・・");
                break;
            case 'ボ':
                result.Add("-・・ ・・");
                break;

            case 'パ':
                result.Add("-・・・ ・・－－・");
                break;
            case 'ピ':
                result.Add("--・・- ・・－－・");
                break;
            case 'プ':
                result.Add("--・・ ・・－－・");
                break;
            case 'ペ':
                result.Add("・ ・・－－・");
                break;
            case 'ポ':
                result.Add("-・・ ・・－－・");
                break;
            /*
            case '゛': return new List<char> { m.O, m.O };
                case '゜': return new List<char> { m.O, m.O, m._, m._, m.O };

                case '1': return new List<char> { m.O, m._, m._, m._, m._ };
                case '2': return new List<char> { m.O, m.O, m._, m._, m._ };
                case '3': return new List<char> { m.O, m.O, m.O, m._, m._ };
                case '4': return new List<char> { m.O, m.O, m.O, m.O, m._ };
                case '5': return new List<char> { m.O, m.O, m.O, m.O, m.O };
                case '6': return new List<char> { m._, m.O, m.O, m.O, m.O };
                case '7': return new List<char> { m._, m._, m.O, m.O, m.O };
                case '8': return new List<char> { m._, m._, m._, m.O, m.O };
                case '9': return new List<char> { m._, m._, m._, m._, m.O };
                case '0': return new List<char> { m._, m._, m._, m._, m._ };

                //欧文記号
                case '.': return new List<char> { m.O, m._, m.O, m._, m.O, m._ };
                case ',': return new List<char> { m._, m._, m.O, m.O, m._, m._ };
                case '?': return new List<char> { m.O, m.O, m._, m._, m.O, m.O };
                case '!': return new List<char> { m._, m.O, m._, m.O, m._, m._ };
                case '-': return new List<char> { m._, m.O, m.O, m.O, m.O, m._ };
                case '/': return new List<char> { m._, m.O, m.O, m._, m.O };
                case '@': return new List<char> { m.O, m._, m._, m.O, m._, m.O };
                case '(': return new List<char> { m._, m.O, m._, m._, m.O };
                case ')': return new List<char> { m._, m.O, m._, m._, m.O, m._ };

                //和文記号
                case 'ー': return new List<char> { m.O, m._, m._, m.O, m._ };
                case '、': return new List<char> { m.O, m._, m.O, m._, m.O, m._ };

                //スペース
                case '　':
                case ' ': return m.SPACE_BETWN_WORDS;
                */
        }
    }
}                 