using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows;

namespace Accountant
{
    [DataContract]
    public class Payment
    {
        [DataMember]
        public int code;
        [DataMember]
        public string name;
        [DataMember]
        public double cost;
        [DataMember]
        public DateTime dayp;
        [DataMember]
        public bool payed;
        [DataMember]
        public bool repeated;
        [DataMember]
        public int type;//0-permanent, 1-changing 
        [DataMember]
        public string comments;
        [DataContract]
        public struct interval
        {
            [DataMember]
            public int days;
            [DataMember]
            public int month;
            [DataMember]
            public int years;
        }
        [DataContract]
        public struct inform
        {
            [DataMember]
            public double costforpoint;
            [DataMember]
            public double prevstat;
            [DataMember]
            public double curstat;
        }

        [DataMember]
        public inform inf;
        [DataMember]
        public interval interv;   
         
        public Payment() { }
        public Payment(string n, DateTime dp, int t, double cfp, bool rep, interval inter, string com)
        {
            if(Account.payments.Count==0)
            {
                code = 0;
            }
            else
            {
                code = Account.payments.Last().code + 1;
            }
            name = n;
            dayp = dp;
            type = t;
            inf.costforpoint = cfp;
            repeated = rep;
            payed = false;
            interv = inter;
            if (repeated==false)
            {
                interv.days = 0;
                interv.month = 0;
                interv.years = 0;
            }
            inf.curstat = 1;
            inf.prevstat = 0;
            cost = Math.Round(inf.costforpoint * (inf.curstat - inf.prevstat), 2);
            comments = com;
        }

        public Payment(string n, DateTime dp, int t, bool rep, interval inter, string com, double cfp, double ps, double cs)
        {
            if (Account.payments.Count == 0)
            {
                code = 0;
            }
            else
            {
                int c = 0;
                foreach(Payment pa in Account.payments)
                {
                    if(pa.code>c)
                    {
                        c = pa.code;
                    }
                }
                code = c+1;
            }
            name = n;
            dayp = dp;
            type = t;
            repeated = rep;
            payed = false;
            interv = inter;
            if (repeated == false)
            {
                interv.days = 0;
                interv.month = 0;
                interv.years = 0;
            }
            inf.costforpoint = cfp;
            inf.prevstat = ps;
            inf.curstat = cs;
            cost = Math.Round(inf.costforpoint * (inf.curstat - inf.prevstat), 2);
            comments = com;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public DateTime Date
        {
            get
            {
                return dayp;
            }
        }

        public double Cost
        {
            get
            {
                return cost;
            }
        }

        public Payment clone()
        {
            Payment n = new Payment();
            n.code = this.code;
            n.comments = this.comments;
            n.cost = this.cost;
            n.dayp = this.dayp;
            n.inf = this.inf;
            n.interv = this.interv;
            n.name = this.name;
            n.payed = this.payed;
            n.repeated = this.repeated;
            n.type = this.type;
            
            return n;
        }
        public void payact()
        {
            dayp=dayp.AddDays(interv.days);
            dayp=dayp.AddMonths(interv.month);
            dayp=dayp.AddYears(interv.years);
            if(type==1)
            {
                inf.prevstat = inf.curstat;
            }
            
        }
    }
}
