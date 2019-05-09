using System;

namespace Code
{
    public class MapEventEditorAttribute:Attribute
    {
        public string Name { get; private set; }
        public string Des { get; private set; }
        public int Type { get; private set; }

        public MapEventEditorAttribute(string name,int type,string des)
        {
            this.Name = name;
            this.Type = type;
            this.Des = des;
        }
    }
}