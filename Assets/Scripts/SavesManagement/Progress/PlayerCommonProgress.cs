using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.SavesManagement
{
    public class PlayerCommonProgress
    {
        public int MetaCurrency { get; set; }
        public int HireCurrencyLimit { get; set; } //(used to hire characters for Roguelike Run)
        public ISet<CharacterTemplate> UnlockedCharacters { get; set; }
        //public HashSet<ItemData> KnownItems
    }
}
