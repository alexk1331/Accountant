using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Globalization;

namespace Accountant
{
    [DataContract]
    public class Week
    {
        [DataMember]
        public List<Day> days = new List<Day>();
        [DataMember]
        public DateTime mon;

        public Week(DateTime dayt)
        {
            mon = serchmon(dayt);
            fillweek(mon, 7);
        }

        public void refresh()
        {
            days.Clear();
            foreach (Day sm in Account.statistic.days)
            {
                if (sm.dat>=mon&&sm.dat<mon.AddDays(7))
                {
                    days.Add(sm);
                }
            }
        }
        public DateTime Sort
        {
            get
            {
                return mon;
            }
        }

        public string Monyear
        {
            get
            {
                CultureInfo ci = new CultureInfo("ru-RU");
                string s = mon.ToString("dd.MM.yyyy", ci)+" - "+mon.AddDays(6).ToString("dd.MM.yyyy", ci);

                return s;
            }
        }
        public double Outcome
        {
            get
            {
                double oc = 0;
                foreach (Day p in days)
                {
                    for (int i = 0; i < p.ppayments.Count; i++)
                    {
                        oc = oc + p.ppayments[i].cost;
                    }
                }
                return oc;
            }
        }

        public double Result
        {
            get
            {
                double res = 0;

                res = Totalincome - Outcome;
                return res;
            }
        }

        public double Totalincome
        {
            get
            {
                double res = 0;
                foreach (Day d in days)
                {
                    res = Cash + Card;
                }
                return res;
            }
        }

        public double Cash
        {
            get
            {
                double res = 0;
                foreach (Day d in days)
                {
                    res = res + d.income;
                }
                return res;
            }
        }

        public double Card
        {
            get
            {
                double res = 0;
                foreach (Day d in days)
                {
                    res = res + d.cardinc;
                }
                return res;
            }
        }

        public void fillweek(DateTime day, int count)
        {
            bool t = false;
            if(count==0)
            {
                return;
            }
            foreach (Day sm in Account.statistic.days)
            {
                if (day.Month == sm.dat.Month && day.Year == sm.dat.Year&&day.Day==sm.dat.Day)
                {
                    t = true;
                    days.Add(sm);
                    fillweek(day.AddDays(1), count - 1);
                    return;
                }
            }
            if(t==false)
            {
                days.Clear();
                MessageBox.Show("Konec");
            }
        }

        public DateTime serchmon(DateTime d)
        {
            if (d.DayOfWeek != DayOfWeek.Monday)
            {
                d = serchmon(d.AddDays(-1));
            }
            return d;
        }

    }
}
