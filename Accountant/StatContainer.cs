using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Accountant
{
    [DataContract]
    public class StatContainer
    {
        [DataMember]
        public List<StatMonth> months = new List<StatMonth>();
        [DataMember]
        public List<Week> weeks = new List<Week>();
        [DataMember]
        public List<Day> days = new List<Day>();

        public StatContainer()
        {

        }

        public DateTime getmin()
        {
            DateTime dt = DateTime.Now;
                foreach(Day sm in days)
                {
                    if(dt>sm.dat)
                    {
                        dt = new DateTime(sm.dat.Year, sm.dat.Month, sm.dat.Day);
                    }
                }
            return dt;
        }

        public void update()
        {
            foreach(StatMonth m in months)
            {
                m.refresh();
            }
            foreach(Week w in weeks)
            {
                w.refresh();
            }
            months.Sort((x, y) =>
    x.monyear.CompareTo(y.monyear));

        }

        public StatContainer clone()
        {
            StatContainer sc = new StatContainer();
            sc.months = this.months;
            sc.weeks = this.weeks;
            sc.days = this.days;
            return sc;
        }
    }
}
