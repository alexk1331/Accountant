using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Accountant
{

    public static class Account
    {

        public static string name = "savefile";//Properties.Settings.Default.accountname;

        public static StatContainer statistic=new StatContainer();

        public static List<Payment> payments = new List<Payment>();

        

        static Account()
        {
            
        }

        public static void addpayment(Payment p)
        {
            bool t = false;
            int i = 0;
            foreach (Payment pay in payments)
            {
                if (p.name == pay.name)
                {  
                    MessageBox.Show("This payment already exists and ll be replaced");//dobavit zdes dialog podtverzdeniya! VAZHNO!
                    payments[i] = p;
                    t = true;
                    break;
                }
                if(pay.code==p.code)
                {
                    p.code++;
                    addpayment(p);
                    break;
                }
                i++;
            }
            if(!t)
            {
                payments.Add(p);
            }
        }

        public static void editpayment(Payment p)
        {
            int i = -1;
            foreach (Payment pay in payments)
            {
                i++;
                if (p.code == pay.code)
                {
                    MessageBox.Show("This payment ll be replaced");//dobavit zdes dialog podtverzdeniya! VAZHNO!
                    payments[i] = p;
                    break;
                }
            }
            
        }

        public static void deletepayment(Payment p)
        {
            int i = 0;
            foreach (Payment pay in payments)
            {
                if (p.name == pay.name)
                {
                    MessageBox.Show("Payment will be deleted");//dobavit zdes dialog podtverzdeniya! VAZHNO!
                    payments.Remove(payments[i]);
                    break;
                }
                i++;
            }
        }

        public static void paypayment(Payment p)
        {
            int i = 0;
            foreach (Payment pay in payments)
            {
                if (p.name == pay.name)
                {
                    MessageBox.Show("Payment confirmed");//dobavit zdes dialog podtverzdeniya! VAZHNO!
                    break;
                }
                i++;
            }
            paypaymentadd(i);
            
            if(payments[i].repeated==true)
            {
                payments[i].payact();
            }
            else
            {
                payments.Remove(payments[i]);
            }
        }

        public static void paypaymentadd(int k)
        {
            bool t = false;
            Payment np = payments[k].clone();
            foreach (Day sm in statistic.days)
            {
                if (sm.dat.Month == payments[k].dayp.Month && sm.dat.Year == payments[k].dayp.Year && sm.dat.Day == payments[k].dayp.Day)
                {
                    np.payed = true;
                    sm.ppayments.Add(np);
                    t = true;
                }
            }
            if (!t)
            {
                DateTime my = new DateTime(payments[k].dayp.Year, payments[k].dayp.Month, 1);
                StatMonth stm = new StatMonth(my);
                statistic.months.Add(stm);
                paypaymentadd(k);
            }
        }

        }
}
