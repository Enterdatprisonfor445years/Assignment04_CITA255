using System;
using System.Collections.Generic;
using System.Text;

namespace QuestLog
{
    public class Quest
    {
        public string Title { get; set; } = string.Empty;
        public string Giver { get; set; } = string.Empty;
        public int Reward { get; set; }
    }
}
