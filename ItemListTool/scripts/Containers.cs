using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Godot;
using Newtonsoft.Json;

namespace ZomboidItemListTool.Containers
{
    [XmlRoot("List of Items")]
    public class ZomboidItemsList{
        [XmlElement("Mod")]
        public List<ZomboidMod> Mods {get; set;}

        public ZomboidItemsList(){
            Mods = new();
        }
    }
    [XmlRoot("Mods")]
    public class ZomboidMod {
        public string Name {get; set;}
        public string WorkshopID {get; set;}
        [XmlElement("Items")]
        public List<ZomboidItem> Items {get; set;}

        public ZomboidMod(){
            Items = new();
        }
    }
    [XmlRoot("Items")]
    public class ZomboidItem{
        public string Name {get; set;}
        public string Module {get; set;}
        [XmlElement("Stats")]
        public List<ZomboidStat> Stats {get; set;}

        public ZomboidItem(){
            Stats = new();
        }
    }

    [XmlRoot("Stats")]
    public class ZomboidStat{
        public string Stat {get; set;}
        public string Value {get; set;}
    }
}