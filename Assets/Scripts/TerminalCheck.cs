using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalCheck : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] Terminals;
    private Dictionary<string, int> terminalAsk = new Dictionary<string, int>();
   
    void Start()
    {
        foreach (GameObject o in Terminals)
        {
            terminalAsk.Add(o.name, 0);
        }
    }

    // Update is called once per frame


    void TryUseTerminal(string name)
    {
        ++terminalAsk[name];
    }
    void LeaveTerminal(string name)
    {
        terminalAsk[name] = 0;
    }

    public bool CheckTerminal(string name)
    {
        bool canWork = false;
        if (terminalAsk[name] == 1)
        {
            canWork = true;
        }
        else
        {
            canWork = false;
        }
        return canWork;
    }

}
