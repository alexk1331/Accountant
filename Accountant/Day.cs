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
    public class Day
    {
        [DataMember]
        public DateTime dat;
        [DataMember]
        public double income;
        [DataMember]
        public double cardinc;
        [DataMember]
        public List<Payment> ppayments = new List<Payment>();

        public Day(DateTime d)
        {
            dat = new DateTime(d.Year, d.Month, d.Day);
            income = 0;
            cardinc = 0;
        }

        public DateTime Sort
        {
            get
            {
                return dat;
            }
        }

        public string Monyear
        {
            get
            {
                string s = dat.ToString("dd.MM.yyyy");
                return s;
            }
        }
        public double Outcome
        {
            get
            {
                double oc = 0;

                    for (int i = 0; i < ppayments.Count; i++)
                    {
                        oc = oc + ppayments[i].cost;
                    }
                return oc;
            }
        }

        public double Result
        {
            get
            {
                double res = income + cardinc;
                res = res - Outcome;
                return res;
            }
        }

        public double Totalincome
        {
            get
            {
                double res = income+cardinc;
                return res;
            }
        }

        public double Cash
        {
            get
            {
                return income;
            }
        }

        public double Card
        {
            get
            {
                return cardinc;
            }
        }

    }
}
