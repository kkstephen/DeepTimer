using System;
using System.ComponentModel;
using UnitODB;

namespace DeepCore
{
    public class Team : OdbEntity
    { 
        public string Name { get; set; }
        public string CarNo {  get; set; }
    }
}
