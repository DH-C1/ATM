using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [System.Serializable]
    public class UserData
    {
        public string userName;
        public int cash;
        public int bankBalance;

        public UserData(string name, int cash, int bankBalance)
        {
            this.userName = name;
            this.cash = cash;
            this.bankBalance = bankBalance;
        }
    }
