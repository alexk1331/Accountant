using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Reflection;



namespace Accountant
{
    class MainWindowViewModel
    {

        Grid title;
        Grid calendar = new Grid();
        ContentControl maintab;
        public DateTime currdate;
        AccountHelper mainah;

        public MainWindowViewModel()
        {

        }

        public MainWindowViewModel(Grid t, ContentControl c, DateTime cd)
        {
            title = t;
            maintab = c;
            currdate = cd;
            mainah = new AccountHelper();
            mainah = AccountHelper.load(Properties.Settings.Default.accountname);
            monthdaraw();

        }

        public void calendartitle()
        {
            title.Children.Clear();
            title.RowDefinitions.Clear();
            title.ColumnDefinitions.Clear();
            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(0.2, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd);
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(0.1, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd1);
            ColumnDefinition cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd2);
            ColumnDefinition cd3 = new ColumnDefinition();
            cd3.Width = new GridLength(0.1, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd3);

            DatePicker nb = new DatePicker();
            nb.SelectedDate = currdate;
            Grid.SetColumn(nb, 0);
            nb.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(dpcal);
            title.Children.Add(nb);

            Button nb1 = new Button();
            nb1.Content = "<";
            Grid.SetColumn(nb1, 1);
            nb1.Click += new RoutedEventHandler(monthprev);
            title.Children.Add(nb1);
            Button nb2 = new Button();
            nb2.Content = ">";
            Grid.SetColumn(nb2, 3);
            nb2.Click += new RoutedEventHandler(monthnext);
            title.Children.Add(nb2);

            TextBlock tb = new TextBlock();
            tb.Text = (CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(currdate.Month)) + " " + currdate.Year;
            Grid.SetColumn(tb, 2);
            tb.TextAlignment = TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            title.Children.Add(tb);
        }

        private void dpcal(object sender, EventArgs e)
        {
            DatePicker dp = (DatePicker)sender;
            currdate = (DateTime)dp.SelectedDate;
            monthdaraw();
        }

        public void monthdaraw()
        {
            calendartitle();
            clearcalititl(0);

            adddayofweek();


            DateTime dt = new DateTime(currdate.Year, currdate.Month, 1);
            int dow = (int)dt.AddDays(-1).DayOfWeek;
            int dim = DateTime.DaysInMonth(dt.Year, dt.Month);
            dim = dim + dow;
            if (dow != 0)
            {
                dt = dt.AddDays(-dow);
            }
            int week = 1;
            for (int i = 0; i <= dim;)
            {
                RowDefinition rd = new RowDefinition();
                calendar.RowDefinitions.Add(rd);
                for (int j = 0; j < 7; j++)
                {
                    Button nb = new Button();
                    Grid.SetRow(nb, week);
                    Grid.SetColumn(nb, j);
                    nb.Content = dt.Day;
                    nb.HorizontalContentAlignment = HorizontalAlignment.Left;
                    nb.VerticalContentAlignment = VerticalAlignment.Top;
                    nb.Click += new RoutedEventHandler(calendarbutclick);
                    if (dt.Month == currdate.Month)
                    {
                        nb.Background = Brushes.White;
                    }
                    else
                    {
                        nb.IsEnabled = false;
                    }
                    foreach (Payment p in Account.payments)//payments otobrazhenie-------------VAZNO!!!
                    {
                        if (dt == p.dayp)
                        {
                            nb.Content = nb.Content.ToString() + "\n" + p.name;
                            nb.Background = Brushes.Red;
                        }
                    }
                    foreach(Day d in Account.statistic.days)
                    {
                        if(d.dat.Day==dt.Day&&d.dat.Month==dt.Month&&d.dat.Year==dt.Year)
                        {
                            for(int q=0;q<d.ppayments.Count;q++)
                            {
                                nb.Content= nb.Content.ToString() + "\n" + d.ppayments[q].name;
                                if(nb.Background==Brushes.Red)
                                {
                                    LinearGradientBrush gradientBrush = new LinearGradientBrush(Color.FromRgb(237, 134, 134), Color.FromRgb(111, 185, 238), new Point(0.5, 0), new Point(0.5, 1));
                                    nb.Background = gradientBrush;
                                }
                                else
                                {
                                    nb.Background = Brushes.SkyBlue;
                                }
                            }
                        }
                    }

                    calendar.Children.Add(nb);
                    dt = dt.AddDays(1);
                    i++;
                }
                week++;
            }
            maintab.Content = calendar;
        }

        private void calendarbutclick(object sender, EventArgs e)
        {
            Button s = (Button)sender;
            string d = "";
            string temp = s.Content.ToString();
            int k = temp.Length;
            if(k>2)
            {
                k = 2;
            }
            for(int i=0; i<k;i++)
            {
                d = d + temp[i];
            }
            DateTime ndt = new DateTime(currdate.Year, currdate.Month, Int32.Parse(d));
            calebdarbutdraw(ndt);
        }

        private void calebdarbutdraw(DateTime dt)
        {
            clearcalititl(2);

            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(0.2, GridUnitType.Star);
            title.ColumnDefinitions.Add(c1);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(1, GridUnitType.Star);
            title.ColumnDefinitions.Add(c2);
            ColumnDefinition c3 = new ColumnDefinition();
            c3.Width = new GridLength(0.2, GridUnitType.Star);
            title.ColumnDefinitions.Add(c3);

            TextBlock tb = new TextBlock();
            CultureInfo culture = new CultureInfo("ru-RU"); 
            tb.Text = culture.DateTimeFormat.GetDayName(dt.DayOfWeek) + " " + dt.ToString(culture.DateTimeFormat.LongDatePattern, culture);
            tb.TextAlignment = TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(tb, 1);
            title.Children.Add(tb);
            Button b = new Button();
            b.Content = "<";
            Grid.SetColumn(b, 0);
            b.Click += delegate (object sender, RoutedEventArgs e) { minusday(sender, e, dt); };
            title.Children.Add(b);
            
            Button b1 = new Button();
            b1.Content = ">";
            Grid.SetColumn(b1, 2);
            b1.Click += delegate (object sender, RoutedEventArgs e) { plusday(sender, e, dt); };
            title.Children.Add(b1);

            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(10, GridUnitType.Star);
            calendar.ColumnDefinitions.Add(cd);
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Star);
            calendar.ColumnDefinitions.Add(cd1);
            ColumnDefinition cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Star);
            calendar.ColumnDefinitions.Add(cd2);

            int x = 0;
            foreach (Payment pay in Account.payments)
            {
                if(pay.dayp==dt)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.MinHeight = 60;
                    rd.MaxHeight = 60;
                    calendar.RowDefinitions.Add(rd);
                    Button nb = new Button();
                    var marg1 = nb.Margin;
                    marg1.Top = 5;
                    nb.Margin = marg1;
                    nb.Name = "N" + pay.code.ToString();
                    nb.Content = pay.name;

                    Grid.SetRow(nb, x);
                    Grid.SetColumn(nb, 0);
                    nb.Click += new RoutedEventHandler(viewbutt);
                    calendar.Children.Add(nb);

                    Button nb1 = new Button();
                    Grid.SetRow(nb1, x);
                    nb1.Content = "Edit";
                    nb1.Name = "E" + pay.code.ToString();
                    Grid.SetColumn(nb1, 1);
                    var marg = nb1.Margin;
                    marg.Left = 5;
                    marg.Right = 5;
                    marg.Top = 5;
                    nb1.Margin = marg;
                    nb1.Click += new RoutedEventHandler(editingbutt);
                    calendar.Children.Add(nb1);

                    Button nb2 = new Button();
                    Grid.SetRow(nb2, x);
                    nb2.Content = "Delete";
                    nb2.Name = "D" + pay.code.ToString();
                    Grid.SetColumn(nb2, 2);
                    nb2.Margin = marg1;
                    nb2.Click += new RoutedEventHandler(deletingbutt);
                    calendar.Children.Add(nb2);
                    x = x + 1;
                }
            }
            Grid payedgrid = new Grid();
            foreach (Day pay in Account.statistic.days)
            {
                if (pay.dat == dt)
                {
                    for(int q=0; q<pay.ppayments.Count;q++)
                    {
                        RowDefinition rd = new RowDefinition();
                        rd.MinHeight = 60;
                        rd.MaxHeight = 60;
                        payedgrid.RowDefinitions.Add(rd);
                        Button nb = new Button();
                        var marg1 = nb.Margin;
                        marg1.Top = 5;
                        nb.Margin = marg1;
                        nb.Content = pay.ppayments[q].name;
                        Grid.SetRow(nb, q);
                        int t = q;
                        nb.Click +=  delegate (object sender, RoutedEventArgs e) { tablpayclik(sender, e, pay.ppayments[t]); }; ;
                        payedgrid.Children.Add(nb);
                    }
                }
            }


            RowDefinition rd1 = new RowDefinition();
            rd1.MinHeight = 60;
            calendar.RowDefinitions.Add(rd1);
            Expander exp = new Expander();
            Grid.SetRow(exp, calendar.RowDefinitions.Count - 1);
            exp.Header = "Оплаченные";
            exp.Content = payedgrid;
            calendar.Children.Add(exp);
            ScrollViewer sv = new ScrollViewer();
            sv.Content = calendar;
            Grid.SetRow(sv, 0);
            Grid cellgrid = new Grid();
            RowDefinition rod = new RowDefinition();
            rod.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition rod1 = new RowDefinition();
            rod1.Height = new GridLength(0.2, GridUnitType.Star);
            cellgrid.RowDefinitions.Add(rod);
            cellgrid.RowDefinitions.Add(rod1);
            cellgrid.Children.Add(sv);
            Grid incgrid = new Grid();
            ColumnDefinition cd11 = new ColumnDefinition();
            cd11.Width = new GridLength(0.3, GridUnitType.Star);
            incgrid.ColumnDefinitions.Add(cd11);
            ColumnDefinition cd12 = new ColumnDefinition();
            cd12.Width = new GridLength(1, GridUnitType.Star);
            incgrid.ColumnDefinitions.Add(cd12);
            ColumnDefinition cd23 = new ColumnDefinition();
            cd23.Width = new GridLength(1, GridUnitType.Star);
            incgrid.ColumnDefinitions.Add(cd23);

            TextBlock tbl = new TextBlock();
            tbl.Text = "Выторг";
            incgrid.Children.Add(tbl);
            Grid ng = new Grid();
            Grid.SetColumn(ng, 1);
            RowDefinition rod2 = new RowDefinition();
            rod2.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition rod3 = new RowDefinition();
            rod3.Height = new GridLength(1, GridUnitType.Star);
            ng.RowDefinitions.Add(rod2);
            ng.RowDefinitions.Add(rod3);

            ColumnDefinition cd111 = new ColumnDefinition();
            cd111.Width = new GridLength(0.6, GridUnitType.Star);
            ng.ColumnDefinitions.Add(cd111);
            ColumnDefinition cd121 = new ColumnDefinition();
            cd121.Width = new GridLength(1, GridUnitType.Star);
            ng.ColumnDefinitions.Add(cd121);
            TextBox tbo = new TextBox();
            Grid.SetColumn(tbo, 1);
            tbo.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            Button but = new Button();
            Grid.SetColumn(but, 2);
            but.Content = "Подтвердить";
            incgrid.Children.Add(but);
            TextBox tbo1 = new TextBox();
            Grid.SetColumn(tbo1, 1);
            Grid.SetRow(tbo1, 1);
            tbo1.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            TextBlock tbl1 = new TextBlock();
            tbl1.Text = "Наличка: ";
            ng.Children.Add(tbl1);
            TextBlock tbl2 = new TextBlock();
            tbl2.Text = "Безнал: ";
            Grid.SetRow(tbl2, 1);
            ng.Children.Add(tbl2);
            
            if (Account.statistic.months.Count==0||!Account.statistic.months.Exists(xx=> xx.monyear==new DateTime(dt.Year, dt.Month, 1)))
            {
                DateTime my = new DateTime(dt.Year, dt.Month, 1);
                StatMonth stm = new StatMonth(my);
                Account.statistic.months.Add(stm);
                saveacc();
            }
            Day stmd = new Day(dt);
            for (int j=0;j<Account.statistic.days.Count;j++)
            {
                if (Account.statistic.days[j].dat.Month == dt.Month && Account.statistic.days[j].dat.Year == dt.Year && Account.statistic.days[j].dat.Day == dt.Day)
                {
                            tbo.Text = Account.statistic.days[j].income.ToString();
                            tbo1.Text = Account.statistic.days[j].cardinc.ToString();
                            stmd = Account.statistic.days[j];
                            break;   
                }
            }
            
            but.Click += delegate (object sender, RoutedEventArgs e) { setdayincome(sender, e, stmd, tbo.Text, tbo1.Text); };
            ng.Children.Add(tbo);
            ng.Children.Add(tbo1);
            incgrid.Children.Add(ng);
            Grid.SetRow(incgrid, 1);
            cellgrid.Children.Add(incgrid);
            maintab.Content = cellgrid;


        }

        private void tablpayclik(object sender, EventArgs e, Payment p)
        {
            clearcalititl(0);
            calendar.Children.Add(viewgridfin(p));
        }
        public void setdayincome(object sender, EventArgs e, Day da, string t, string tc)//mozhet zapretit menyat vytorg na slishkom starih datah?
        {
            da.income = double.Parse(t.Replace('.', ','));//proverit chotb ne bilo problem s tochoi-zapytoi-вроде нет
            da.cardinc = double.Parse(tc.Replace('.', ','));

            for (int j = 0; j < Account.statistic.days.Count; j++)
            {
                if (Account.statistic.days[j].dat.Month == da.dat.Month && Account.statistic.days[j].dat.Year == da.dat.Year&&Account.statistic.days[j].dat.Day==da.dat.Day)
                {
                    Account.statistic.days[j] = da;
                    break;
                }
            }
            saveacc();
        }
        private void minusday(object sender, EventArgs e, DateTime d)
        {
            calebdarbutdraw(d.AddDays(-1));
        }
        private void plusday(object sender, EventArgs e, DateTime d)
        {
            calebdarbutdraw(d.AddDays(1));
        }

        private void adddayofweek()
        {
            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(0.5, GridUnitType.Star);
            calendar.RowDefinitions.Add(rd);
            DateTime d = new DateTime(2018, 10, 1);
            for (int i = 0; i < 7; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                calendar.ColumnDefinitions.Add(cd);
                Button mon = new Button();
                Grid.SetRow(mon, 0);
                Grid.SetColumn(mon, i);
                mon.Content = CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(d.DayOfWeek);
                d = d.AddDays(1);
                calendar.Children.Add(mon);
            }

        }

        public void datechange(int newdt)
        {
            currdate = currdate.AddMonths(newdt);
            monthdaraw();
        }

        private void monthprev(object sender, EventArgs e)
        {
            datechange(-1);
        }
        private void monthnext(object sender, EventArgs e)
        {
            datechange(1);
        }

        public void saveacc()
        {
            mainah.save(mainah);
        }

        public void eventlistdraw()
        {
            calendar.Children.Clear();
            calendar.RowDefinitions.Clear();
            calendar.ColumnDefinitions.Clear();

            title.Children.Clear();
            title.RowDefinitions.Clear();
            title.ColumnDefinitions.Clear();
            TextBlock tb = new TextBlock();
            tb.Text = "Список существующих задач";
            tb.TextAlignment = TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            title.Children.Add(tb);



            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(10, GridUnitType.Star);
            calendar.ColumnDefinitions.Add(cd);
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Star);
            calendar.ColumnDefinitions.Add(cd1);
            ColumnDefinition cd2 = new ColumnDefinition();
            cd2.Width = new GridLength(1, GridUnitType.Star);
            calendar.ColumnDefinitions.Add(cd2);
            RowDefinition rd1 = new RowDefinition();
            rd1.MinHeight = 50;
            rd1.MaxHeight = 50;
            calendar.RowDefinitions.Add(rd1);
            Button nb11 = new Button();
            nb11.Content = "Добавить новое +";
            Grid.SetRow(nb11, 0);
            Grid.SetColumn(nb11, 0);
            nb11.Click += new RoutedEventHandler(addnewbutt);
            calendar.Children.Add(nb11);


            int x = 1;
            foreach (Payment pay in Account.payments)
            {
                RowDefinition rd = new RowDefinition();
                rd.MinHeight = 60;
                rd.MaxHeight = 60;
                calendar.RowDefinitions.Add(rd);
                Button nb = new Button();
                var marg1 = nb.Margin;
                marg1.Top = 5;
                nb.Margin = marg1;
                nb.Name = "N"+pay.code.ToString();
                nb.Content = pay.name;
                Grid.SetRow(nb, x);
                Grid.SetColumn(nb, 0);
                nb.Click += new RoutedEventHandler(viewbutt);
                calendar.Children.Add(nb);

                Button nb1 = new Button();
                Grid.SetRow(nb1, x);
                nb1.Content = "Edit";
                nb1.Name = "E"+pay.code.ToString() ;
                Grid.SetColumn(nb1, 1);
                var marg = nb1.Margin;
                marg.Left = 5;
                marg.Right = 5;
                marg.Top = 5;
                nb1.Margin = marg;
                nb1.Click += new RoutedEventHandler(editingbutt);
                calendar.Children.Add(nb1);

                Button nb2 = new Button();
                Grid.SetRow(nb2, x);
                nb2.Content = "Delete";
                nb2.Name = "D"+pay.code.ToString() ;
                Grid.SetColumn(nb2, 2);
                nb2.Margin = marg1;
                nb2.Click += new RoutedEventHandler(deletingbutt);
                calendar.Children.Add(nb2);
                x = x + 1;

            }
            ScrollViewer sv= new ScrollViewer();
            sv.Content = calendar;
            maintab.Content = sv;

        }

        private void deletingbutt(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            foreach (Payment pay in Account.payments)
            {
                if (("D"+pay.code.ToString())==b.Name)
                {
                    Account.deletepayment(pay);
                    mainah.save(mainah);
                    eventlistdraw();
                    break;
                }
            }
        }

        private void editingbutt(object sender, EventArgs e)
        {
            title.Children.Clear();
            title.RowDefinitions.Clear();
            title.ColumnDefinitions.Clear();

            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(0.2, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd);
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd1);
            Button s = (Button)sender;

            Button b = new Button();
            b.Content = "Назад";
            b.Click += new RoutedEventHandler(backbaddnew);
            title.Children.Add(b);
            TextBlock tb = new TextBlock();
            string n = s.Name.Substring(1);

            Payment pay = new Payment();
            foreach (Payment pa in Account.payments)
            {
                if (pa.code == Int32.Parse(n))
                {
                    pay = pa;
                }
            }
            tb.Text = "Изменение задачи: "+ pay.name;
            Grid.SetColumn(tb, 1);
            tb.TextAlignment = TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            title.Children.Add(tb);
            calendar.Children.Clear();
            calendar.RowDefinitions.Clear();
            calendar.ColumnDefinitions.Clear();

            RowDefinition cold = new RowDefinition();
            cold.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition cold1 = new RowDefinition();
            cold1.Height = new GridLength(0.1, GridUnitType.Star);
            calendar.RowDefinitions.Add(cold);
            calendar.RowDefinitions.Add(cold1);

            Button but = new Button();
            Grid.SetRow(but, 1);
            but.Name = "S"+n;
            but.Content = "Сохранить";
            but.Click += new RoutedEventHandler(saveedited);
            calendar.Children.Add(but);

            Grid adgrid = editgridcreate(pay);

            ScrollViewer sv = new ScrollViewer();
            Grid.SetRow(sv, 0);
            sv.Content = adgrid;
            calendar.Children.Add(sv);
            maintab.Content = calendar;
        }

        private Grid editgridcreate(Payment p)
        {
            Grid adgrid = new Grid();
            for (int i = 0; i < 3; i++)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(0.5, GridUnitType.Star);

                adgrid.RowDefinitions.Add(rd);
            }
            for (int i = 0; i < 3; i++)
            {
                RowDefinition rd = new RowDefinition();
                adgrid.RowDefinitions.Add(rd);
            }
            Grid sp = new Grid();//i want to add more func here thats why i need grid
            Grid.SetRow(sp, 0);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(0.3, GridUnitType.Star);
                sp.ColumnDefinitions.Add(cd);
                sp.ColumnDefinitions.Add(cd1);
            }

            var marg = sp.Margin;
            marg.Top = 5;
            sp.Margin = marg;
            TextBox tbox = new TextBox();
            tbox.Name = "pname";
            tbox.Text = p.name;
            tbox.MaxLength = 99;
            sp.Children.Add(tbox);
            adgrid.Children.Add(sp);

            Grid sp1 = new Grid();
            Grid.SetRow(sp1, 1);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(0.5, GridUnitType.Star);
                sp1.ColumnDefinitions.Add(cd);
                sp1.ColumnDefinitions.Add(cd1);
            }
            sp1.Margin = marg;
            TextBox tbox1 = new TextBox();
            tbox1.Name = "pcfp";
            tbox1.Text = p.inf.costforpoint.ToString();
            tbox1.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            sp1.Children.Add(tbox1);
            TextBlock tb1 = new TextBlock();
            Grid.SetColumn(tb1, 1);
            tb1.Text = "Цена услуги(за единицу)";
            sp1.Children.Add(tb1);
            adgrid.Children.Add(sp1);

            Grid gr3 = new Grid();
            Grid.SetRow(gr3, 2);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(0.5, GridUnitType.Star);
                gr3.ColumnDefinitions.Add(cd);
                gr3.ColumnDefinitions.Add(cd1);
            }
            gr3.Margin = marg;

            TextBlock tb2 = new TextBlock();
            Grid.SetColumn(tb2, 1);
            Grid.SetColumn(tb2, 1);
            tb2.Text = "Дата платежа";
            gr3.Children.Add(tb2);
            DatePicker dp = new DatePicker();
            dp.Name = "pdayp";
            dp.SelectedDate = p.dayp;
            gr3.Children.Add(dp);
            adgrid.Children.Add(gr3);

            Grid gr4 = new Grid();
            Grid.SetRow(gr4, 3);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                gr4.ColumnDefinitions.Add(cd);
                gr4.ColumnDefinitions.Add(cd1);
            }
            gr4.Margin = marg;
            Grid gr41 = new Grid();
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                gr41.ColumnDefinitions.Add(cd);
                gr41.ColumnDefinitions.Add(cd1);
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Star);
                RowDefinition rd1 = new RowDefinition();
                gr41.RowDefinitions.Add(rd);
                gr41.RowDefinitions.Add(rd1);
            }
            Grid.SetColumn(gr41, 1);
            TextBox tbox411 = new TextBox();
            tbox411.Name = "pcurstat";
            tbox411.Text = p.inf.curstat.ToString();
            tbox411.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr41.Children.Add(tbox411);
            TextBlock tb411 = new TextBlock();
            Grid.SetColumn(tb411, 1);
            tb411.Text = "Новые показатели";
            gr41.Children.Add(tb411);
            TextBox tbox412 = new TextBox();
            tbox412.Name = "pprevstat";
            tbox412.Text = p.inf.prevstat.ToString();
            Grid.SetRow(tbox412, 1);
            tbox412.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr41.Children.Add(tbox412);
            TextBlock tb412 = new TextBlock();
            Grid.SetColumn(tb412, 1);
            Grid.SetRow(tb412, 1);
            tb412.Text = "Старые показатели";
            gr41.Children.Add(tb412);
            gr4.Children.Add(gr41);
  
            GroupBox gbox = new GroupBox();
            gbox.Header = "Тип оплаты";
            RadioButton rb = new RadioButton();
            rb.Name = "ptype0";
            RadioButton rb1 = new RadioButton();
            rb1.Name = "ptype1";
            rb.Content = "Постоянный";
            rb1.Content = "По количеству";
            rb.IsChecked = true;
            StackPanel stp = new StackPanel();
            rb.GroupName = "types";
            rb1.GroupName = "types";
            if (p.type == 0)
            {
                rb.IsChecked = true;
                gr41.Visibility = Visibility.Collapsed;
            }
            else
            {
                rb1.IsChecked = true;
                gr41.Visibility = Visibility.Visible;
            }
            rb.Checked += new RoutedEventHandler(npcheckbox);
            rb1.Checked += new RoutedEventHandler(npcheckbox);
            stp.Children.Add(rb);
            stp.Children.Add(rb1);
            gbox.Content = stp;
            gr4.Children.Add(gbox);
            adgrid.Children.Add(gr4);

            Grid gr5 = new Grid();
            Grid.SetRow(gr5, 4);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(0.5, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                gr5.ColumnDefinitions.Add(cd);
                gr5.ColumnDefinitions.Add(cd1);
            }
            gr5.Margin = marg;

            Grid gr51 = new Grid();
            Grid.SetColumn(gr51, 1);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(0.3, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                gr51.ColumnDefinitions.Add(cd);
                gr51.ColumnDefinitions.Add(cd1);
                RowDefinition rd = new RowDefinition();
                RowDefinition rd1 = new RowDefinition();
                RowDefinition rd2 = new RowDefinition();
                gr51.RowDefinitions.Add(rd);
                gr51.RowDefinitions.Add(rd1);
                gr51.RowDefinitions.Add(rd2);
            }
            CheckBox cb = new CheckBox();
            cb.Name = "prepeated";
            cb.Content = "Повторяющееся";
            if(p.repeated==true)
            {
                cb.IsChecked = true;
                gr51.Visibility = Visibility.Visible;
            }
            else
            {
                cb.IsChecked = false;
                gr51.Visibility = Visibility.Collapsed;
            }
            cb.Checked += new RoutedEventHandler(repcb);
            cb.Unchecked += new RoutedEventHandler(repcb);
            gr5.Children.Add(cb);
            TextBox tbox511 = new TextBox();
            tbox511.Name = "pdays";
            tbox511.Text = p.interv.days.ToString();
            tbox511.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr51.Children.Add(tbox511);
            TextBlock tb511 = new TextBlock();
            Grid.SetColumn(tb511, 1);
            tb511.Text = "Дней";
            gr51.Children.Add(tb511);
            TextBox tbox512 = new TextBox();
            tbox512.Name = "pmonth";
            tbox512.Text = p.interv.month.ToString();
            Grid.SetRow(tbox512, 1);
            tbox512.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr51.Children.Add(tbox512);
            TextBlock tb512 = new TextBlock();
            Grid.SetColumn(tb512, 1);
            Grid.SetRow(tb512, 1);
            tb512.Text = "Месяцев";
            gr51.Children.Add(tb512);
            TextBox tbox513 = new TextBox();
            tbox513.Name = "pyear";
            tbox513.Text = p.interv.years.ToString();
            Grid.SetRow(tbox513, 2);
            tbox513.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr51.Children.Add(tbox513);
            TextBlock tb513 = new TextBlock();
            Grid.SetColumn(tb513, 1);
            Grid.SetRow(tb513, 2);
            tb513.Text = "Лет";
            gr51.Children.Add(tb513);
            gr5.Children.Add(gr51);
            adgrid.Children.Add(gr5);
     
            GroupBox gbox1 = new GroupBox();
            gbox1.Header = "Примечания";
            ScrollViewer sv = new ScrollViewer();
            TextBox tb11 = new TextBox();
            tb11.Width = calendar.Width;
            tb11.MaxHeight = 200;
            tb11.Name = "pcomments";
            tb11.TextWrapping = TextWrapping.Wrap;
            tb11.Text = p.comments;
            sv.Content = tb11;
            gbox1.Content = sv;
            Grid.SetRow(gbox1, 5);
            adgrid.Children.Add(gbox1);
            adgrid.Margin = new Thickness(5, 0, 5, 10);
            return adgrid;
        }

        private void saveedited(object sender, EventArgs e)
        {
            List<FrameworkElement> felist = new List<FrameworkElement>();
            felist = searchchild(calendar);
            string n = "";
            DateTime dp = new DateTime(1, 1, 1);
            int t = 0;
            bool rep = false;
            Payment.interval inter = new Payment.interval();
            double cfp = 0;
            double ps = 0;
            double cs = 0;
            string com = "";
            bool val = true;
            foreach (FrameworkElement el in felist)
            {
                if (el.Name.Length > 0 && el.Name[0] == 'p')
                {
                    switch (el.Name)
                    {
                        case "pname":
                            TextBox temp = (TextBox)el;
                            if (temp.Text != "")
                            {
                                n = temp.Text;
                            }
                            else
                            {
                                MessageBox.Show("Недопустимое имя!");
                                val = false;
                            }
                            break;
                        case "pcfp":
                            temp = (TextBox)el;
                            if (temp.Text != "")
                            {
                                cfp = double.Parse(temp.Text.Replace('.', ','));
                            }
                            else
                            {
                                MessageBox.Show("Некорректная цена!");
                                val = false;
                            }
                            break;
                        case "pdayp":
                            DatePicker dpick = (DatePicker)el;
                            if (dpick.SelectedDate != null)
                            {
                                dp = dpick.SelectedDate.Value;
                            }
                            else
                            {
                                MessageBox.Show("Не выбрана дата");
                                val = false;
                            }
                            break;
                        case "ptype0":
                            RadioButton rb = (RadioButton)el;
                            if (rb.IsChecked == true)
                            {
                                t = 0;
                            }
                            break;
                        case "ptype1":
                            rb = (RadioButton)el;
                            if (rb.IsChecked == true)
                            {
                                t = 1;
                            }
                            break;
                        case "pcurstat":
                            temp = (TextBox)el;
                            cs = double.Parse(temp.Text.Replace('.', ','));//remember this trick its important. to fix coma-dot issue
                            break;
                        case "pprevstat":
                            temp = (TextBox)el;
                            ps = double.Parse(temp.Text.Replace('.', ','));
                            break;
                        case "prepeated":
                            CheckBox cb = (CheckBox)el;
                            if (cb.IsChecked == true)
                            {
                                rep = true;
                            }
                            else
                            {
                                rep = false;
                            }
                            break;
                        case "pdays":
                            temp = (TextBox)el;
                            inter.days = Int32.Parse(temp.Text, CultureInfo.InvariantCulture);
                            break;
                        case "pmonth":
                            temp = (TextBox)el;
                            inter.month = Int32.Parse(temp.Text, CultureInfo.InvariantCulture);
                            break;
                        case "pyear":
                            temp = (TextBox)el;
                            inter.years = Int32.Parse(temp.Text, CultureInfo.InvariantCulture);
                            break;
                        case "pcomments":
                            temp = (TextBox)el;
                            com = temp.Text;
                            break;

                        default:
                            break;
                    }
                }
            }
            if (val == true)
            {
                Payment p=new Payment();

                Button s = (Button)sender;
                string n1 = s.Name.Substring(1);

                if (t == 0)
                {
                    p = new Payment(n, dp, t, cfp, rep, inter, com);
                }
                else
                {
                    p = new Payment(n, dp, t, rep, inter, com, cfp, ps, cs);
                }
                p.code = Int32.Parse(n1);
                Account.editpayment(p);
                mainah.save(mainah);
                viewbutt(s, e);
            }
        }

        private void viewbutt(object sender, EventArgs e)
        {
            clearcalititl(2);
            Payment p = new Payment();
            Button s = (Button)sender;
            foreach (Payment pay in Account.payments)
            {
                if ((pay.code.ToString()) == s.Name.Substring(1))
                {
                    p = pay;
                    break;
                }
            }


            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(0.2, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd);
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd1);
            ColumnDefinition cd123 = new ColumnDefinition();
            cd123.Width = new GridLength(0.2, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd123);
            string n = s.Name.Substring(1);
            Button b = new Button();
            b.Content = "Назад";
            b.Click += new RoutedEventHandler(backbaddnew);
            title.Children.Add(b);
            TextBlock tb = new TextBlock();
            tb.Text = p.name;
            Grid.SetColumn(tb, 1);
            tb.TextAlignment = TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            title.Children.Add(tb);
            Button b1 = new Button();
            b1.Content = "Оплатить";
            b1.Name = "P" + n;
            b1.Background = Brushes.LightGreen;
            Grid.SetColumn(b1, 2);
            b1.Click += new RoutedEventHandler(payb);
            title.Children.Add(b1);

            RowDefinition cold = new RowDefinition();
            cold.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition cold1 = new RowDefinition();
            cold1.Height = new GridLength(0.1, GridUnitType.Star);
            calendar.RowDefinitions.Add(cold);
            calendar.RowDefinitions.Add(cold1);

            Grid sp = new Grid();
            ColumnDefinition cd2 = new ColumnDefinition();
            ColumnDefinition cd3 = new ColumnDefinition();
            sp.ColumnDefinitions.Add(cd2);
            sp.ColumnDefinitions.Add(cd3);
            Grid.SetRow(sp, 1);
            
            
            Button but = new Button();
            
            but.Content = "Удалить";
            but.Name = "D" + n;

            but.Click += new RoutedEventHandler(deletingbutt);
            Button but1 = new Button();
            Grid.SetColumn(but1, 1);
            but1.Content = "Изменить";
            but1.Name = "E" +n;
            but1.Click += new RoutedEventHandler(editingbutt);
            sp.Children.Add(but);
            sp.Children.Add(but1);
            calendar.Children.Add(sp);

            Grid adgrid = viewgridcreate(Int32.Parse(n));

            ScrollViewer sv = new ScrollViewer();
            Grid.SetRow(sv, 0);
            sv.Content = adgrid;
            calendar.Children.Add(sv);
            maintab.Content = calendar;
        }

        private void payb(object sender, EventArgs e)
        {
            Payment p = new Payment();
            Button s = (Button)sender;
            foreach (Payment pay in Account.payments)
            {
                if ((pay.code.ToString()) == s.Name.Substring(1))
                {
                    p = pay;
                    break;
                }
            }
            Account.paypayment(p);
            mainah.save(mainah);
            viewbutt(sender, e);
        }

        private Grid viewgridcreate(int cod)
        {
            Payment p = new Payment();
            foreach (Payment pay in Account.payments)
            {
                if (pay.code == cod)
                {
                    p = pay;
                    break;
                }
            }

            Grid g = viewgridfin(p);
            return g;

        }

        public Grid viewgridfin(Payment p)
        {
            Grid adgrid = new Grid();
            RowDefinition rdef = new RowDefinition();
            rdef.Height = new GridLength(0.5, GridUnitType.Star);
            adgrid.RowDefinitions.Add(rdef);
            rdef = new RowDefinition();
            rdef.Height = new GridLength(0.5, GridUnitType.Star);
            adgrid.RowDefinitions.Add(rdef);
            rdef = new RowDefinition();
            rdef.Height = new GridLength(1, GridUnitType.Star);
            adgrid.RowDefinitions.Add(rdef);
            rdef = new RowDefinition();
            rdef.Height = new GridLength(0.5, GridUnitType.Star);
            adgrid.RowDefinitions.Add(rdef);
            rdef = new RowDefinition();
            rdef.Height = new GridLength(1, GridUnitType.Star);
            adgrid.RowDefinitions.Add(rdef);

            Thickness marg= new Thickness(0, 10, 0 ,0);
            TextBlock tbox = new TextBlock();
            tbox.Name = "pname";
            tbox.Text = "Имя платежа: "+p.name;
            adgrid.Children.Add(tbox);
            TextBlock tb2 = new TextBlock();
            Grid.SetRow(tb2, 1);
            CultureInfo culture = new CultureInfo("ru-RU");
            tb2.Text = "Дата платежа: " + p.dayp.ToString(culture.DateTimeFormat.LongDatePattern, culture);
            adgrid.Children.Add(tb2);

            if(p.type==1)
            {
                GroupBox gb = new GroupBox();
                Grid ingrid = new Grid();
                Grid.SetRow(gb, 2);
                RowDefinition rd = new RowDefinition();
                ingrid.RowDefinitions.Add(rd);
                rd = new RowDefinition();
                ingrid.RowDefinitions.Add(rd);
                ColumnDefinition cd = new ColumnDefinition();
                ingrid.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                ingrid.ColumnDefinitions.Add(cd);
                TextBlock tb1 = new TextBlock();
                tb1.Text = "Цена за единицу: " + p.inf.costforpoint.ToString() + " грн.";
                Grid.SetRow(tb1, 1);
                ingrid.Children.Add(tb1);

                tb1 = new TextBlock();
                tb1.Text = "Цена общая: " + p.cost.ToString()+" грн.";
                Grid.SetRow(tb1, 0);
                ingrid.Children.Add(tb1);

                tb1 = new TextBlock();
                tb1.Text = "Предыдущие показатели: " + p.inf.prevstat.ToString();
                Grid.SetRow(tb1, 1);
                Grid.SetColumn(tb1, 1);
                ingrid.Children.Add(tb1);

                tb1 = new TextBlock();
                tb1.Text = "Новые показатели: " + p.inf.curstat.ToString();
                Grid.SetRow(tb1, 0);
                Grid.SetColumn(tb1, 1);
                ingrid.Children.Add(tb1);
                gb.Content = ingrid;
                adgrid.Children.Add(gb);
            }
            else
            {
                TextBlock tb1 = new TextBlock();
                tb1.Text = "Цена: " + p.cost.ToString() + " грн.";
                Grid.SetRow(tb1, 2);
                adgrid.RowDefinitions[2].Height = new GridLength(0.5, GridUnitType.Star);
                adgrid.Children.Add(tb1);
            }

            if(p.repeated==false)
            {
                TextBlock tb = new TextBlock();
                Grid.SetRow(tb, 3);
                tb.Text = "Разовый";
                adgrid.Children.Add(tb);
            }
            else
            {
                TextBlock tb = new TextBlock();
                Grid.SetRow(tb, 3);
                tb.Text = "Повторяется через каждые: "+p.interv.days+" дней, "+p.interv.month+" месяцев, "+p.interv.years+" лет.";
                adgrid.Children.Add(tb);
            }
            
            GroupBox gbox1 = new GroupBox();
            gbox1.Header = "Примечания";
            ScrollViewer sv = new ScrollViewer();
            TextBlock tb11 = new TextBlock();
            tb11.Text = p.comments;
            tb11.Width = calendar.Width;
            tb11.MaxHeight = 200;
            tb11.TextWrapping = TextWrapping.Wrap;
            sv.Content = tb11;
            gbox1.Content = sv;
            Grid.SetRow(gbox1, 4);
            adgrid.Children.Add(gbox1);
            adgrid.Margin = new Thickness(5, 0, 5, 10);
            return adgrid;
        }

        private void addnewbutt(object sender, EventArgs e)
        {
            title.Children.Clear();
            title.RowDefinitions.Clear();
            title.ColumnDefinitions.Clear();

            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(0.2, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd);
            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(1, GridUnitType.Star);
            title.ColumnDefinitions.Add(cd1);

            Button b = new Button();
            b.Content = "Назад";
            b.Click += new RoutedEventHandler(backbaddnew);
            title.Children.Add(b);
            TextBlock tb = new TextBlock();
            tb.Text = "Добавление новой задачи";
            Grid.SetColumn(tb, 1);
            tb.TextAlignment = TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            title.Children.Add(tb);
            calendar.Children.Clear();
            calendar.RowDefinitions.Clear();
            calendar.ColumnDefinitions.Clear();

            RowDefinition cold = new RowDefinition();
            cold.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition cold1 = new RowDefinition();
            cold1.Height = new GridLength(0.1, GridUnitType.Star);
            calendar.RowDefinitions.Add(cold);
            calendar.RowDefinitions.Add(cold1);

            Button but = new Button();
            Grid.SetRow(but, 1);
            but.Content = "Добавить";
            but.Click+= new RoutedEventHandler(bsavebaddnew);
            calendar.Children.Add(but);

            Grid adgrid = adgridcreate() ;
            
            ScrollViewer sv = new ScrollViewer();
            Grid.SetRow(sv, 0);
            sv.Content = adgrid;
            calendar.Children.Add(sv);
            maintab.Content = calendar;
        }

        private Grid adgridcreate()
        {
            Grid adgrid = new Grid();
            for(int i=0; i<3; i++)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(0.5, GridUnitType.Star);
                
                adgrid.RowDefinitions.Add(rd);
            }
            for (int i = 0; i < 3; i++)
            {
                RowDefinition rd = new RowDefinition();
                adgrid.RowDefinitions.Add(rd);
            }
            Grid sp = new Grid();//i want to add more func here thats why i need grid
            Grid.SetRow(sp, 0);
            for(int i=0; i<1;i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(0.3, GridUnitType.Star);
                sp.ColumnDefinitions.Add(cd);
                sp.ColumnDefinitions.Add(cd1);
            }
            
            var marg = sp.Margin;
            marg.Top = 5;
            sp.Margin = marg;
            TextBox tbox = new TextBox();
            tbox.Name = "pname";
            tbox.Text = "Имя платежа";
            tbox.MaxLength = 99;
            sp.Children.Add(tbox);
            adgrid.Children.Add(sp);

            Grid sp1 = new Grid();
            Grid.SetRow(sp1, 1);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(0.5, GridUnitType.Star);
                sp1.ColumnDefinitions.Add(cd);
                sp1.ColumnDefinitions.Add(cd1);
            }
            sp1.Margin = marg;
            TextBox tbox1 = new TextBox();
            tbox1.Name = "pcfp";
            tbox1.Text = "0";
            tbox1.PreviewTextInput+= new TextCompositionEventHandler(NumberValidationTextBox);
            sp1.Children.Add(tbox1);
            TextBlock tb1 = new TextBlock();
            Grid.SetColumn(tb1, 1);
            tb1.Text = "Цена услуги(за единицу)";
            sp1.Children.Add(tb1);
            adgrid.Children.Add(sp1);

            Grid gr3 = new Grid();
            Grid.SetRow(gr3, 2);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(0.5, GridUnitType.Star);
                gr3.ColumnDefinitions.Add(cd);
                gr3.ColumnDefinitions.Add(cd1);
            }
            gr3.Margin = marg;

            TextBlock tb2 = new TextBlock();
            Grid.SetColumn(tb2, 1);
            Grid.SetColumn(tb2, 1);
            tb2.Text = "Дата платежа";
            gr3.Children.Add(tb2);
            DatePicker dp = new DatePicker();
            dp.Name = "pdayp";
            gr3.Children.Add(dp);
            adgrid.Children.Add(gr3);

            Grid gr4 = new Grid();
            Grid.SetRow(gr4, 3);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                gr4.ColumnDefinitions.Add(cd);
                gr4.ColumnDefinitions.Add(cd1);
            }
            gr4.Margin = marg;
            Grid gr41 = new Grid();
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                gr41.ColumnDefinitions.Add(cd);
                gr41.ColumnDefinitions.Add(cd1);
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Star);
                RowDefinition rd1 = new RowDefinition();
                gr41.RowDefinitions.Add(rd);
                gr41.RowDefinitions.Add(rd1);
            }
            Grid.SetColumn(gr41, 1);
            TextBox tbox411 = new TextBox();
            tbox411.Name = "pcurstat";
            tbox411.Text = "1";
            tbox411.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr41.Children.Add(tbox411);
            TextBlock tb411 = new TextBlock();
            Grid.SetColumn(tb411, 1);
            tb411.Text = "Новые показатели";
            gr41.Children.Add(tb411);
            TextBox tbox412 = new TextBox();
            tbox412.Name = "pprevstat";
            tbox412.Text = "0";
            Grid.SetRow(tbox412, 1);
            tbox412.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr41.Children.Add(tbox412);
            TextBlock tb412 = new TextBlock();
            Grid.SetColumn(tb412, 1);
            Grid.SetRow(tb412, 1);
            tb412.Text = "Старые показатели";
            gr41.Children.Add(tb412);
            gr4.Children.Add(gr41);
            gr41.Visibility = Visibility.Collapsed;
            GroupBox gbox = new GroupBox();
            gbox.Header = "Тип оплаты";
            RadioButton rb = new RadioButton();
            rb.Name = "ptype0";
            RadioButton rb1 = new RadioButton();
            rb1.Name = "ptype1";
            rb.Content = "Постоянный";
            rb1.Content = "По количеству";
            rb.IsChecked = true;
            StackPanel stp = new StackPanel();
            rb.Checked += new RoutedEventHandler(npcheckbox);
            rb1.Checked += new RoutedEventHandler(npcheckbox);
            rb.GroupName = "types";
            rb1.GroupName = "types";
            stp.Children.Add(rb);
            stp.Children.Add(rb1);
            gbox.Content = stp;
            gr4.Children.Add(gbox);
            adgrid.Children.Add(gr4);

            Grid gr5 = new Grid();
            Grid.SetRow(gr5, 4);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(0.5, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                gr5.ColumnDefinitions.Add(cd);
                gr5.ColumnDefinitions.Add(cd1);
            }
            gr5.Margin = marg;
            
            Grid gr51 = new Grid();
            Grid.SetColumn(gr51, 1);
            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(0.3, GridUnitType.Star);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                gr51.ColumnDefinitions.Add(cd);
                gr51.ColumnDefinitions.Add(cd1);
                RowDefinition rd = new RowDefinition();
                RowDefinition rd1 = new RowDefinition();
                RowDefinition rd2 = new RowDefinition();
                gr51.RowDefinitions.Add(rd);
                gr51.RowDefinitions.Add(rd1);
                gr51.RowDefinitions.Add(rd2);
            }
            CheckBox cb = new CheckBox();
            cb.Name = "prepeated";
            cb.Content = "Повторяющееся";
            cb.IsChecked = true;
            cb.Checked += new RoutedEventHandler(repcb);
            cb.Unchecked+= new RoutedEventHandler(repcb);
            gr5.Children.Add(cb);
            TextBox tbox511 = new TextBox();
            tbox511.Name = "pdays";
            tbox511.Text = "0";
            tbox511.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr51.Children.Add(tbox511);
            TextBlock tb511 = new TextBlock();
            Grid.SetColumn(tb511, 1);
            tb511.Text = "Дней";
            gr51.Children.Add(tb511);
            TextBox tbox512 = new TextBox();
            tbox512.Name = "pmonth";
            tbox512.Text = "1";
            Grid.SetRow(tbox512, 1);
            tbox512.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr51.Children.Add(tbox512);
            TextBlock tb512 = new TextBlock();
            Grid.SetColumn(tb512, 1);
            Grid.SetRow(tb512, 1);
            tb512.Text = "Месяцев";
            gr51.Children.Add(tb512);
            TextBox tbox513 = new TextBox();
            tbox513.Name = "pyear";
            tbox513.Text = "0";
            Grid.SetRow(tbox513, 2);
            tbox513.PreviewTextInput += new TextCompositionEventHandler(NumberValidationTextBox);
            gr51.Children.Add(tbox513);
            TextBlock tb513 = new TextBlock();
            Grid.SetColumn(tb513, 1);
            Grid.SetRow(tb513, 2);
            tb513.Text = "Лет";
            gr51.Children.Add(tb513);
            gr5.Children.Add(gr51);
            adgrid.Children.Add(gr5);

            GroupBox gbox1 = new GroupBox();
            gbox1.Header = "Примечания";
            ScrollViewer sv = new ScrollViewer();
            TextBox tb11 = new TextBox();
            tb11.Width = calendar.Width;
            tb11.MaxHeight = 200;
            tb11.Name = "pcomments";
            tb11.TextWrapping = TextWrapping.Wrap;
            sv.Content = tb11;
            gbox1.Content = sv;
            Grid.SetRow(gbox1, 5);
            adgrid.Children.Add(gbox1);

            adgrid.Margin= new Thickness(5, 0, 5, 10);
            return adgrid;
        }

        private void repcb(object sender, EventArgs e)
        {
            CheckBox s = (CheckBox)sender;
            Grid g = (Grid)s.Parent;
            
            for (int i = 0; i < g.Children.Count; i++)
            {
                if (g.Children[i] is Grid)
                {
                    if(s.IsChecked==true)
                    {
                        g.Children[i].Visibility = Visibility.Visible;
                    }
                    else if(s.IsChecked==false)
                    {
                        g.Children[i].Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void npcheckbox(object sender, EventArgs e)
        {
            RadioButton s = (RadioButton)sender;
            StackPanel c =(StackPanel)s.Parent;
            
            FrameworkElement fm = (FrameworkElement)c.Parent;

            while(!(fm is Grid))
            {
                fm =(FrameworkElement)fm.Parent;
            }

            Grid g = (Grid)fm;
            if (s.Name == "ptype1")
            {
                for (int i = 0; i < g.Children.Count; i++)
                {
                    if(g.Children[i] is Grid)
                    {
                        g.Children[i].Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                for (int i = 0; i < g.Children.Count; i++)
                {
                    if (g.Children[i] is Grid)
                    {
                        g.Children[i].Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void bsavebaddnew(object sender, EventArgs e)
        {
            List<FrameworkElement> felist = new List<FrameworkElement>();
            felist = searchchild(calendar);
            string n="";
            DateTime dp=new DateTime(1, 1, 1);
            int t=0;
            bool rep=false;
            Payment.interval inter=new Payment.interval();
            double cfp=0;
            double ps=0;
            double cs=0;
            string com="";
            bool val = true;
            foreach (FrameworkElement el in felist)
            {
                if(el.Name.Length>0&&el.Name[0]=='p')
                {
                    switch (el.Name)
                    {
                        case "pname":
                            TextBox temp =(TextBox)el;
                            if(temp.Text!="")
                            {
                                n = temp.Text;
                            }
                            else
                            {
                                MessageBox.Show("Недопустимое имя!");
                                val = false;
                            }
                            break;
                        case "pcfp":
                            temp = (TextBox)el;
                            if (temp.Text != "")
                            {
                                cfp = Double.Parse(temp.Text, CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                MessageBox.Show("Некорректная цена!");
                                val = false;
                            }
                            break;
                        case "pdayp":
                            DatePicker dpick = (DatePicker)el;
                            if(dpick.SelectedDate!=null)
                            {
                                dp = dpick.SelectedDate.Value;
                            }
                            else
                            {
                                MessageBox.Show("Не выбрана дата");
                                val = false;
                            }
                            break;
                        case "ptype0":
                            RadioButton rb = (RadioButton)el;
                            if(rb.IsChecked==true)
                            {
                                t = 0;
                            }
                            break;
                        case "ptype1":
                            rb = (RadioButton)el;
                            if (rb.IsChecked == true)
                            {
                                t = 1;
                            }
                            break;
                        case "pcurstat":
                            temp = (TextBox)el;
                            cs = Double.Parse(temp.Text, CultureInfo.InvariantCulture);
                            break;
                        case "pprevstat":
                            temp = (TextBox)el;
                            ps = Double.Parse(temp.Text, CultureInfo.InvariantCulture);
                            break;
                        case "prepeated":
                            CheckBox cb = (CheckBox)el;
                            if (cb.IsChecked == true)
                            {
                                rep = true;
                            }
                            else
                            {
                                rep = false;
                            }
                            break;
                        case "pdays":
                            temp = (TextBox)el;
                            inter.days = Int32.Parse(temp.Text, CultureInfo.InvariantCulture);
                            break;
                        case "pmonth":
                            temp = (TextBox)el;
                            inter.month = Int32.Parse(temp.Text, CultureInfo.InvariantCulture);
                            break;
                        case "pyear":
                            temp = (TextBox)el;
                            inter.years = Int32.Parse(temp.Text, CultureInfo.InvariantCulture);
                            break;
                        case "pcomments":
                            temp = (TextBox)el;
                            com = temp.Text;
                            break;

                        default:
                            break;
                    }
                } 
            }
            if(val==true)
            {
                Payment p;
                if (t == 0)
                {
                    p = new Payment(n, dp, t, cfp, rep, inter, com);
                }
                else
                {
                    p = new Payment(n, dp, t, rep, inter, com, cfp, ps, cs);
                }
                Account.addpayment(p);
                mainah.save(mainah);
                eventlistdraw();
            }

        }
        
        public List<FrameworkElement> searchchild(FrameworkElement parent)
        {
            List<FrameworkElement> mainlist = new List<FrameworkElement>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                if(VisualTreeHelper.GetChild(parent, i) is FrameworkElement)
                {
                    FrameworkElement child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                    mainlist.Add(child);
                    if (VisualTreeHelper.GetChildrenCount(child) > 0)
                    {
                        mainlist.AddRange(searchchild(child));
                    }
                }
                
            }

                return mainlist;
        }

        private void backbaddnew(object sender, EventArgs e)
        {
            eventlistdraw();
        }

        public void statisticdraw()
        {
            Account.statistic.update();
            calendar.Children.Clear();
            calendar.RowDefinitions.Clear();
            calendar.ColumnDefinitions.Clear();

            title.Children.Clear();
            title.RowDefinitions.Clear();
            title.ColumnDefinitions.Clear();
            Grid stp = new Grid();
            ColumnDefinition cd = new ColumnDefinition();
            ColumnDefinition cd1 = new ColumnDefinition();
            ColumnDefinition cd2 = new ColumnDefinition();
            stp.ColumnDefinitions.Add(cd);
            stp.ColumnDefinitions.Add(cd1);
            stp.ColumnDefinitions.Add(cd2);
            ComboBox cbt = new ComboBox();
            TextBlock tb = new TextBlock();
            tb.Text = "Таблица: по месяцам";
            tb.Name = "t0";

            cbt.Items.Add(tb);
            TextBlock tb1 = new TextBlock();
            tb1.Text = "Таблица: по неделям";
            tb1.Name = "t1";
            cbt.Items.Add(tb1);
            TextBlock tb2 = new TextBlock();
            tb2.Text = "Таблица: по дням";
            tb2.Name = "t2";
            cbt.Items.Add(tb2);

            cbt.SelectedIndex = 0;
            //cbt.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(comboboxgeneral); 
            cbt.SelectionChanged += new SelectionChangedEventHandler(comboboxgeneral);//rabotaet tolko pri smene vybora. Dorabotat i prosto na click
            stp.Children.Add(cbt);

            Button bt = new Button();
            Grid.SetColumn(bt, 1);
            bt.Content = "Платежи";
            bt.Margin = new Thickness(10, 0, 10, 0);
            bt.Click += new RoutedEventHandler(paymentstat);
            stp.Children.Add(bt);

            Button bt1 = new Button();
            Grid.SetColumn(bt1, 2);
            bt1.Content = "Графики";
            bt1.Click += new RoutedEventHandler(grafclick);
            stp.Children.Add(bt1);

            title.Children.Add(stp);
            tablmonth(DateTime.Now, Account.statistic.getmin());
        }

        private void paymentstat(object sende, EventArgs er)
        {
            DataGrid dg = new DataGrid();
            dg = paygridcreate(dg, DateTime.Now, Account.statistic.getmin(),  "");
            statisticpaygrid(dg, Account.statistic.getmin(), DateTime.Now);
            dg.MouseDoubleClick += delegate (object sender, MouseButtonEventArgs e) { tablpaydblclik(sender, e); };
        }

        private void tablpaydblclik(object sender, EventArgs e)
        {
            DataGrid s = (DataGrid)sender;
            if(s.SelectedItem!=null)
            {
                Payment p = (Payment)s.SelectedItem;
                clearcalititl(0);
                calendar.Children.Add(viewgridfin(p));
            }
            
        }

        public void statisticpaygrid(DataGrid dg, DateTime star, DateTime en)
        {
            clearcalititl(0);
            Grid sgrid = new Grid();
            RowDefinition cd = new RowDefinition();
            RowDefinition cd1 = new RowDefinition();
            cd1.Height = new GridLength(0.3, GridUnitType.Star);
            sgrid.RowDefinitions.Add(cd);
            sgrid.RowDefinitions.Add(cd1);
            dg.IsReadOnly = true;
            sgrid.Children.Add(dg);
            Grid tablecontrolg = new Grid();
            Grid.SetRow(tablecontrolg, 1);
            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(1.5, GridUnitType.Star);
            tablecontrolg.RowDefinitions.Add(rd);
            rd = new RowDefinition();
            tablecontrolg.RowDefinitions.Add(rd);
            tablecontrolg.Margin = new Thickness(0, 10, 0, 0);
            ColumnDefinition cdef = new ColumnDefinition();
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            cdef.Width = new GridLength(0.2, GridUnitType.Star);
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            cdef.Width = new GridLength(0.2, GridUnitType.Star);
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            tablecontrolg.ColumnDefinitions.Add(cdef);
            Thickness m = new Thickness(0, 0, 10, 10);
            TextBlock tb = new TextBlock();
            tb.Text = "За период";
            tb.TextAlignment = TextAlignment.Center;
            TextBlock tb3 = new TextBlock();
            tb3.Text = "С: ";
            Grid.SetColumn(tb3, 1);
            tb3.TextAlignment = TextAlignment.Right;
            TextBlock tb2 = new TextBlock();
            Grid.SetColumn(tb2, 3);
            tb2.Text = "по:         ";
            tb2.TextAlignment = TextAlignment.Right;
            DatePicker start = new DatePicker();
            Grid.SetColumn(start, 2);
            start.Margin = m;
            start.SelectedDate = star;
            DatePicker end = new DatePicker();
            Grid.SetColumn(end, 4);
            end.Margin = m;
            end.SelectedDate = en;
            Button confirm = new Button();
            confirm.Content = "Вывести даные";
            
            Grid.SetColumn(confirm, 5);
            confirm.Margin = m;
            TextBlock tb33 = new TextBlock();
            tb33.Text = "Поиск по имени: ";
            tb33.TextAlignment = TextAlignment.Center;
            Grid.SetRow(tb33, 1);
            TextBox tbox = new TextBox();
            tbox.Margin = new Thickness(0, 0, 10, 0);
            Grid.SetRow(tbox, 1);
            Grid.SetColumn(tbox, 1);
            Grid.SetColumnSpan(tbox, 4);
            confirm.Click += delegate (object sender, RoutedEventArgs e) { tablsearchpay(sender, e, dg, (DateTime)end.SelectedDate, (DateTime)start.SelectedDate, tbox.Text); };
            tablecontrolg.Children.Add(tbox);
            tablecontrolg.Children.Add(tb);
            tablecontrolg.Children.Add(tb2);
            tablecontrolg.Children.Add(tb3);
            tablecontrolg.Children.Add(tb33);
            tablecontrolg.Children.Add(start);
            tablecontrolg.Children.Add(end);
            tablecontrolg.Children.Add(confirm);

            sgrid.Children.Add(tablecontrolg);
            calendar.Children.Add(sgrid);
        }

        private void tablsearchpay(object sender, EventArgs e, DataGrid dg, DateTime start, DateTime end, string txt)
        {
            dg = paygridcreate(dg, start, end, txt);
        }

        public DataGrid paygridcreate(DataGrid dg, DateTime start, DateTime end, string txt)
        {
            List<Payment> plist = new List<Payment>();
            foreach (Day m in Account.statistic.days)
            {
                if (m.dat <= start && m.dat >= end)
                {
                    for(int i=0; i<m.ppayments.Count; i++)
                    {
                        if(m.ppayments[i].name.ToLower().Contains(txt.ToLower())||txt=="")
                        {
                            plist.Add(m.ppayments[i]);
                        }
                    }
                }
            }
            dg.Columns.Clear();
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Имя";
            c2.Binding = new Binding("Name");
            dg.Columns.Add(c2);

            dg.AutoGenerateColumns = false;
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Дата";
            c1.Binding = new Binding("Date");
            c1.Binding.StringFormat = "dd.MM.yyyy";
            c1.SortDirection = ListSortDirection.Descending;
            dg.Columns.Add(c1);
            
            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Цена";
            c3.Binding = new Binding("Cost");
            dg.Columns.Add(c3);
            foreach (DataGridBoundColumn col in dg.Columns)
            {
                col.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            }
            dg.ItemsSource = plist;
            return dg;
        }

        public DataGrid tablegeneral()
        {
            DataGrid dg = new DataGrid();
            dg.AutoGenerateColumns = false;
            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Дата";
            c1.Binding = new Binding("Monyear");
            c1.SortMemberPath="Sort";
            c1.SortDirection = ListSortDirection.Ascending;
            
            dg.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Чистая\nприбыль";
            c2.Binding = new Binding("Result");
            dg.Columns.Add(c2);

            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Расходы";
            c3.Binding = new Binding("Outcome");
            dg.Columns.Add(c3);

            DataGridTextColumn c4 = new DataGridTextColumn();
            c4.Header = "Общий\nвыторг";
            c4.Binding = new Binding("Totalincome");
            dg.Columns.Add(c4);

            DataGridTextColumn c5 = new DataGridTextColumn();
            c5.Header = "Оплачено\nналичными";
            c5.Binding = new Binding("Cash");
            dg.Columns.Add(c5);

            DataGridTextColumn c6 = new DataGridTextColumn();
            c6.Header = "Оплачено\nкартой";
            c6.Binding = new Binding("Card");
            dg.Columns.Add(c6);
            foreach (DataGridBoundColumn col in dg.Columns)
            {
                col.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            }
            dg.IsReadOnly = true;
            
            return dg;
        }

        private void comboboxgeneral(object sender, EventArgs e)
        {
            ComboBox s = (ComboBox)sender;
            if(s.SelectedIndex==0)
            {
                tablmonth(DateTime.Now, Account.statistic.getmin());
            }
            else if(s.SelectedIndex == 1)
            {
                tablweek(DateTime.Now, Account.statistic.getmin());
            }
            else
            {
                tablday(DateTime.Now, Account.statistic.getmin());
            }
        }

        public void statisticgrid(DataGrid dg, DateTime star, DateTime en)
        {
            clearcalititl(0);
            Grid sgrid = new Grid();
            RowDefinition cd = new RowDefinition();
            RowDefinition cd1 = new RowDefinition();
            cd1.Height = new GridLength(0.3, GridUnitType.Star);
            sgrid.RowDefinitions.Add(cd);
            sgrid.RowDefinitions.Add(cd1);
            sgrid.Children.Add(dg);
            Grid tablecontrolg = new Grid();
            Grid.SetRow(tablecontrolg, 1);
            tablecontrolg.Margin = new Thickness(0, 10, 0, 0);
            ColumnDefinition cdef = new ColumnDefinition();
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            cdef.Width = new GridLength(0.2, GridUnitType.Star);
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            cdef.Width = new GridLength(0.2, GridUnitType.Star);
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            tablecontrolg.ColumnDefinitions.Add(cdef);
            cdef = new ColumnDefinition();
            tablecontrolg.ColumnDefinitions.Add(cdef);
            Thickness m = new Thickness(0, 0, 10, 10);
            TextBlock tb = new TextBlock();
            tb.Text = "За период";
            tb.TextAlignment = TextAlignment.Center;
            TextBlock tb3 = new TextBlock();
            tb3.Text = "С: ";
            Grid.SetColumn(tb3, 1);
            tb3.TextAlignment = TextAlignment.Right;
            TextBlock tb2 = new TextBlock();
            Grid.SetColumn(tb2, 3);
            tb2.Text = "по:         ";
            tb2.TextAlignment = TextAlignment.Right;
            DatePicker start = new DatePicker();
            Grid.SetColumn(start, 2);
            start.Margin = m;
            start.SelectedDate = star;
            DatePicker end = new DatePicker();
            Grid.SetColumn(end, 4);
            end.Margin = m;
            end.SelectedDate = en;
            Button confirm = new Button();
            confirm.Content = "Вывести даные";
            confirm.Click += delegate (object sender, RoutedEventArgs e) { tablsearchdate(sender, e, dg, (DateTime)end.SelectedDate, (DateTime)start.SelectedDate); };
            Grid.SetColumn(confirm, 5);
            confirm.Margin = m;

            tablecontrolg.Children.Add(tb);
            tablecontrolg.Children.Add(tb2);
            tablecontrolg.Children.Add(tb3);
            tablecontrolg.Children.Add(start);
            tablecontrolg.Children.Add(end);
            tablecontrolg.Children.Add(confirm);

            sgrid.Children.Add(tablecontrolg);
            calendar.Children.Add(sgrid);
        }

        private void tablsearchdate(object sender, EventArgs e, DataGrid dg, DateTime start, DateTime end)
        {
            if(dg.ItemsSource.GetType() == Account.statistic.months.GetType())
            {
                tablmonth(start, end);
            }
            else if(dg.ItemsSource.GetType() == Account.statistic.weeks.GetType())
            {
                tablweek(start, end);
            }
            else
            {
                tablday(start, end);
            }
        }

        private void tablmonth(DateTime st, DateTime en)
        {
            DataGrid dg = new DataGrid();
            dg=tablegeneral();
            List<StatMonth> ds = new List<StatMonth>();
            foreach(StatMonth m in Account.statistic.months)
            {
                if(m.monyear<=st&&m.monyear>=en)
                {
                    ds.Add(m);
                }
            }
            dg.ItemsSource = ds;
            dg.MouseDoubleClick += delegate (object sender, MouseButtonEventArgs e) { tablmonthdblclick(sender, e); };
            dgsortclick(dg);
            statisticgrid(dg, en, st);
            
        }

        private void tablmonthdblclick(object sendr, EventArgs er)
        {
            DataGrid s = (DataGrid)sendr;
            if (s.SelectedItem != null)
            {
                StatMonth sm = (StatMonth)s.SelectedItem;
                clearcalititl(0);
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(0.5, GridUnitType.Star);
                calendar.RowDefinitions.Add(rd);
                rd = new RowDefinition();
                calendar.RowDefinitions.Add(rd);

                Grid infgrid = new Grid();
                rd = new RowDefinition();
                infgrid.RowDefinitions.Add(rd);
                rd = new RowDefinition();
                infgrid.RowDefinitions.Add(rd);
                ColumnDefinition cd = new ColumnDefinition();
                infgrid.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                infgrid.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                infgrid.ColumnDefinitions.Add(cd);
                TextBlock tb = new TextBlock();
                tb.Text = "Дата: \n" + sm.Monyear;
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Чистая прибыль: \n" + sm.Result + " грн.";
                Grid.SetRow(tb, 1);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Общий выторг: \n" + sm.Totalincome + " грн.";
                Grid.SetColumn(tb, 1);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Расходы: \n" + sm.Outcome + " грн.";
                Grid.SetRow(tb, 1);
                Grid.SetColumn(tb, 1);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Выторг наличкой: \n" + sm.Cash + " грн.";
                Grid.SetColumn(tb, 2);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Выторг на карту: \n" + sm.Card + " грн.";
                Grid.SetRow(tb, 1);
                Grid.SetColumn(tb, 2);
                infgrid.Children.Add(tb);
                GroupBox gb = new GroupBox();
                gb.Content = infgrid;
                calendar.Children.Add(gb);

                TabControl tc = new TabControl();
                Grid.SetRow(tc, 1);

                TabItem d = new TabItem();
                d.Header = "Дни";
                d.Width = 200;
                DataGrid dg = tablegeneral();
                dg.ItemsSource = sm.days;
                dg.IsReadOnly = true;
                dg.MouseDoubleClick += delegate (object sender, MouseButtonEventArgs e) { tabldaydblclick(sender, e); };
                d.Content = dg;
                TabItem p = new TabItem();
                p.Header = "Платежи";
                DataGrid dg1 = new DataGrid();
                dg1 = paygridcreate(dg1, sm.monyear.AddMonths(1).AddDays(-1), sm.monyear, "");
                dg1.IsReadOnly = true;
                dg1.MouseDoubleClick += delegate (object sender, MouseButtonEventArgs e) { tablpaydblclik(sender, e); };
                p.Content = dg1;
                p.Width = 200;
                tc.Items.Add(d);
                tc.Items.Add(p);
                calendar.Children.Add(tc);
            }      
        }

        private void tablweek(DateTime st, DateTime en)
        {
            DataGrid dg = new DataGrid();
            dg = tablegeneral();
            List<Week> ds = new List<Week>();
            foreach (Week m in Account.statistic.weeks)
            {
                if (m.mon <= st && m.mon >= en)
                {
                    ds.Add(m);
                }
            }
            dg.ItemsSource = ds;
            dg.MouseDoubleClick += delegate (object sender, MouseButtonEventArgs e) { tablweekdblclick(sender, e); };
            dgsortclick(dg);
            statisticgrid(dg, en, st);
        }

        private void tablweekdblclick(object sendr, EventArgs er)
        {
            DataGrid s = (DataGrid)sendr;
            if (s.SelectedItem != null)
            {
                Week sm = (Week)s.SelectedItem;
                clearcalititl(0);
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(0.5, GridUnitType.Star);
                calendar.RowDefinitions.Add(rd);
                rd = new RowDefinition();
                calendar.RowDefinitions.Add(rd);

                Grid infgrid = new Grid();
                rd = new RowDefinition();
                infgrid.RowDefinitions.Add(rd);
                rd = new RowDefinition();
                infgrid.RowDefinitions.Add(rd);
                ColumnDefinition cd = new ColumnDefinition();
                infgrid.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                infgrid.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                infgrid.ColumnDefinitions.Add(cd);
                TextBlock tb = new TextBlock();
                tb.Text = "Дата: \n" + sm.Monyear;
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Чистая прибыль: \n" + sm.Result + " грн.";
                Grid.SetRow(tb, 1);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Общий выторг: \n" + sm.Totalincome + " грн.";
                Grid.SetColumn(tb, 1);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Расходы: \n" + sm.Outcome + " грн.";
                Grid.SetRow(tb, 1);
                Grid.SetColumn(tb, 1);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Выторг наличкой: \n" + sm.Cash + " грн.";
                Grid.SetColumn(tb, 2);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Выторг на карту: \n" + sm.Card + " грн.";
                Grid.SetRow(tb, 1);
                Grid.SetColumn(tb, 2);
                infgrid.Children.Add(tb);
                GroupBox gb = new GroupBox();
                gb.Content = infgrid;
                calendar.Children.Add(gb);

                TabControl tc = new TabControl();
                Grid.SetRow(tc, 1);

                TabItem d = new TabItem();
                d.Header = "Дни";
                d.Width = 200;
                DataGrid dg = tablegeneral();
                dg.ItemsSource = sm.days;
                dg.IsReadOnly = true;
                dg.MouseDoubleClick += delegate (object sender, MouseButtonEventArgs e) { tabldaydblclick(sender, e); };
                d.Content = dg;
                TabItem p = new TabItem();
                p.Header = "Платежи";
                DataGrid dg1 = new DataGrid();
                dg1 = paygridcreate(dg1, sm.mon.AddDays(6), sm.mon, "");
                dg1.IsReadOnly = true;
                dg1.MouseDoubleClick += delegate (object sender, MouseButtonEventArgs e) { tablpaydblclik(sender, e); };
                p.Content = dg1;
                p.Width = 200;
                tc.Items.Add(d);
                tc.Items.Add(p);
                calendar.Children.Add(tc);
            }
        }

        private void tablday(DateTime st, DateTime en)
        {
            DataGrid dg = new DataGrid();
            dg = tablegeneral();
            List<Day> ds = new List<Day>();
            foreach (Day m in Account.statistic.days)
            {
                if (m.dat <= st && m.dat >= en)
                {
                    ds.Add(m);
                }
            }
            dg.ItemsSource = ds;
            dg.MouseDoubleClick += delegate (object sender, MouseButtonEventArgs e) { tabldaydblclick(sender, e); };
            dgsortclick(dg);
            statisticgrid(dg, en, st);
        }

        private void tabldaydblclick(object sendr, EventArgs er)
        {
            DataGrid s = (DataGrid)sendr;
            if (s.SelectedItem != null)
            {
                Day sm = (Day)s.SelectedItem;
                clearcalititl(0);
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(0.5, GridUnitType.Star);
                calendar.RowDefinitions.Add(rd);
                rd = new RowDefinition();
                calendar.RowDefinitions.Add(rd);

                Grid infgrid = new Grid();
                rd = new RowDefinition();
                infgrid.RowDefinitions.Add(rd);
                rd = new RowDefinition();
                infgrid.RowDefinitions.Add(rd);
                ColumnDefinition cd = new ColumnDefinition();
                infgrid.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                infgrid.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                infgrid.ColumnDefinitions.Add(cd);
                TextBlock tb = new TextBlock();
                tb.Text = "Дата: \n" + sm.Monyear;
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Чистая прибыль: \n" + sm.Result + " грн.";
                Grid.SetRow(tb, 1);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Общий выторг: \n" + sm.Totalincome + " грн.";
                Grid.SetColumn(tb, 1);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Расходы: \n" + sm.Outcome + " грн.";
                Grid.SetRow(tb, 1);
                Grid.SetColumn(tb, 1);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Выторг наличкой: \n" + sm.Cash + " грн.";
                Grid.SetColumn(tb, 2);
                infgrid.Children.Add(tb);
                tb = new TextBlock();
                tb.Text = "Выторг на карту: \n" + sm.Card + " грн.";
                Grid.SetRow(tb, 1);
                Grid.SetColumn(tb, 2);
                infgrid.Children.Add(tb);
                GroupBox gb = new GroupBox();
                gb.Content = infgrid;
                calendar.Children.Add(gb);


                DataGrid dg1 = new DataGrid();
                dg1 = paygridcreate(dg1, sm.dat, sm.dat, "");
                dg1.IsReadOnly = true;
                dg1.MouseDoubleClick += delegate (object sender, MouseButtonEventArgs e) { tablpaydblclik(sender, e); };
                Grid.SetRow(dg1, 1);
                calendar.Children.Add(dg1);
            }
            
        }

        private void grafclick(object sender, EventArgs e)
        {
        }

        public void dgsortclick(DataGrid dg)
        {
            var performSortMethod = typeof(DataGrid)
                            .GetMethod("PerformSort",
                                       BindingFlags.Instance | BindingFlags.NonPublic);

            performSortMethod?.Invoke(dg, new[] { dg.Columns[0] });
        }


        public void clearcalititl(int i)
        {
            if(i==0||i==2)
            {
                calendar.Children.Clear();
                calendar.RowDefinitions.Clear();
                calendar.ColumnDefinitions.Clear();   
            }
            if(i==1||i==2)
            {
                title.Children.Clear();
                title.RowDefinitions.Clear();
                title.ColumnDefinitions.Clear();
            }
        }

    }
}
