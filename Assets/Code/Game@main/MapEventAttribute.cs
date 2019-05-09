using BDFramework.Mgr;

namespace Code
{
    public class MapEventAttribute:ManagerAtrribute
    {
        public string Des { get; private set; }

        public MapEventAttribute(string Id,string des="") : base(Id)
        {
            Des = des;
        }
    }
}