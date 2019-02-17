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
    [DataContract]
    public class AccountHelper
    {
        [DataMember]
        public string name;
        [DataMember]
        public StatContainer statistic = new StatContainer();
        [DataMember]
        public List<Payment> payments = new List<Payment>();

        public AccountHelper()
        {
            
        }

        public void updatingthis()
        {
            name = Account.name;
            statistic = Account.statistic;
            payments = Account.payments;
        }

        public void updatingacc()
        {
            Account.name=name;
            Account.statistic=statistic;
            Account.payments=payments;
        }


        public void save(AccountHelper ah)//sohranenie
        {
            ah.updatingthis();
            string path = Directory.GetCurrentDirectory() + @"\Save";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if(!File.Exists(path + @"\" + "backup_" + Account.name + ".json"))
            {
                File.Create(path + @"\" + "backup_" + Account.name + ".json");
            }
            try
            {
                DataContractJsonSerializer saver = new DataContractJsonSerializer(typeof(AccountHelper));
                using (FileStream fs = new FileStream((path + @"\" + Account.name + ".json"), FileMode.Create))
                {
                    saver.WriteObject(fs, ah);
                    fs.Close();
                    fs.Dispose();
                }
                
            }
            catch
            {
                MessageBox.Show("В процессе сохранения что-то пошло не так. Последняя операция не была сохранена.");
                FileInfo finf1 = new FileInfo(path + @"\" + "backup_" + Account.name + ".json");
                finf1.CopyTo(path + @"\" + Account.name + ".json", true);
            }
            finally
            {
                
                FileInfo finf1 = new FileInfo(path + @"\" + Account.name + ".json");
                finf1.CopyTo(path + @"\" + "backup_" + Account.name + ".json", true);
            }
            
            
        }

        public static AccountHelper load(string nam)//zagruzka
        {
            string path = Directory.GetCurrentDirectory() + @"\Save";
            string pathfile = path + @"\" + nam + ".json";
            if(!File.Exists(pathfile))
            {
                AccountHelper ahtemp = new AccountHelper();
                ahtemp.updatingthis();
                ahtemp.name = Properties.Settings.Default.accountname;
                ahtemp.save(ahtemp);
            }
            DataContractJsonSerializer loader = new DataContractJsonSerializer(typeof(AccountHelper));
            using (FileStream fs = new FileStream((pathfile), FileMode.Open))
            {
                AccountHelper loaded = (AccountHelper)loader.ReadObject(fs);
                if(loaded.name=="")
                {
                    loaded.name= Properties.Settings.Default.accountname;
                }
                loaded.updatingacc();
                return loaded;
            }
        }

        static public void delete(string nam)//udalenie personazha
        {
            string path = Directory.GetCurrentDirectory() + @"\Save\" + nam + ".json";
            File.Delete(path);
        }
    }
}

