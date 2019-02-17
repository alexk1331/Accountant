using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Globalization;

namespace Accountant
{
    [DataContract]
    public class StatMonth
    {
        [DataMember]
        public DateTime monyear;
        [DataMember]
        public List<Day> days = new List<Day>();

        public StatMonth() { }
        public StatMonth(DateTime dayt)
        {
            monyear = new DateTime(dayt.Year, dayt.Month, 1);
            update();
        }  

        public void update()
        {
            days.Clear();
            DateTime dt = new DateTime(monyear.Year, monyear.Month, 1);
            while (dt.Month == monyear.Month)
            {
                Day d = new Day(dt);
                days.Add(d);
                Account.statistic.days.Add(d);
                dt = dt.AddDays(1);
            }
            weekcreate();
        }

        public void refresh()
        {
            days.Clear();
            DateTime dt = new DateTime(monyear.Year, monyear.Month, 1);
                foreach(Day d in Account.statistic.days)
                {
                    if(d.dat.Month==dt.Month&&d.dat.Year==dt.Year)
                    {
                        days.Add(d);
                    }
                }
        }

        public void weekcreate()
        {
            DateTime startdat = DateTime.Now;
            
            foreach(Day d in Account.statistic.days)
            {
                if(d.dat<startdat)
                {
                    startdat = new DateTime(d.dat.Year, d.dat.Month, d.dat.Day);
                }
            }
            while(startdat.DayOfWeek!=DayOfWeek.Monday)
            {
                startdat = startdat.AddDays(1);
            }
            while(startdat.AddDays(6)<=DateTime.Now)
            {
                if(!Account.statistic.weeks.Exists(xx => xx.mon == startdat))
                {
                    Week w = new Week(startdat);
                    Account.statistic.weeks.Add(w);
                }
                startdat = startdat.AddDays(7);
            }
        }

        public DateTime Sort
        {
            get
            {
                return monyear;
            }
        }

        public string Monyear
        {
            get
            {
                CultureInfo ci = new CultureInfo("ru-RU");
                string s= monyear.ToString("MMMM yyyy", ci);
                
                return s;
            }
        }

        public double Outcome
            {
            get
            {
                double oc = 0;
                foreach(Day p in days)
                {
                    for(int i=0; i<p.ppayments.Count;i++)
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
                double res=0;
                foreach(Day d in days)
                {
                    res = res + d.income+d.cardinc;
                }
                res = res - Outcome;
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

        

    }
}
