
using System;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json;

namespace calculator
{
    class MainViewModel : INotifyPropertyChanged
    {
        string path = $@"C:\Users\{Environment.UserName}\history.txt";
        public ICommand IButtonClicked { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _expression = "";
        private int _historyVisibility = 0;
        private ObservableCollection<string> _recentExpressions = new ObservableCollection<string>();
        public ICommand SolveCommand { get; }

        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                if (value == _expression)
                    return;
                _expression = value;
                OnPropertyChanged(nameof(Expression));
            }
        }
        public int HistoryVisibility
        {
            get
            {
                return _historyVisibility;
            }
            set
            {
                if (_historyVisibility != value)
                    _historyVisibility = value;
                OnPropertyChanged(nameof(HistoryVisibility));
            }
        }

        public ObservableCollection<string> RecentExpressions
        {
            get
            {
                return _recentExpressions;
            }
        }

        public void LoadHistory(string path)
        {
            foreach (var str in System.IO.File.ReadAllLines(path))
            {
                _recentExpressions.Add(str);
            }
        }

        public void HistoryReadDB()
        {
            using (var connection = new System.Data.SQLite.SQLiteConnection($"Data Source=C:/Users/usaro/OneDrive/Рабочий стол/калькулятор/calculator/history.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "select * from history";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string exp = Convert.ToString(reader["value"]);
                    _recentExpressions.Add(exp);
                }
            }
        }

        public void HistoryWriteDB(string exp)
        {
            using (var connection = new System.Data.SQLite.SQLiteConnection($"Data Source=C:/Users/usaro/OneDrive/Рабочий стол/калькулятор/calculator/history.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "Insert into history values " + "(\"" + exp + "\")"; 
                command.ExecuteReader();
            }
        }


        public MainViewModel()
        {
#if DBHISTORY
            HistoryReadDB();
#else
            if (System.IO.File.Exists(path))
            {
                LoadHistory(path);
            }
            else
            {
                System.IO.File.Create(path);
            }
#endif
            IButtonClicked = new RelayCommand<string>(x =>
            {
                if (x == "=")
                {
                    try
                    {
                        Calculator calc = new Calculator();
                        string historyExpression = Expression;
                        Expression = Expression.Replace('×', '*');
                        Expression = Expression.Replace('÷', '/');
                        double result = calc.Calculate(Expression);
                        Expression = result.ToString();
                        historyExpression += " = " + Expression;
                        RecentExpressions.Add(historyExpression);
                        HistoryWriteDB(historyExpression);
                    }
                    catch (Exception e)
                    {
                        Expression = "Error";
                    }
                }
                else if (x == "history")
                {
                    if (HistoryVisibility == 0)
                        HistoryVisibility = 1000;
                    else
                        HistoryVisibility = 0;
                }
                else if (x == "clear_history")
                {
#if DBHISTORY
                    using (var connection = new System.Data.SQLite.SQLiteConnection($"Data Source=C:/Users/usaro/OneDrive/Рабочий стол/калькулятор/calculator/history.db"))
                    {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = "Delete from history";
                        command.ExecuteReader();
                        _recentExpressions.Clear();
                    }
#else
                    _recentExpressions.Clear();
                    System.IO.File.Delete(path);
                    System.IO.File.Create(path);
#endif
                }
                else if (x == "C")
                {
                    Expression = "";
                }
                else if (x == "⌫")
                {
                    if (Expression.Length != 0)
                        Expression = Expression.Substring(0, Expression.Length - 1);
                }
                else
                {
                    if (Expression == "0" && x == "0")
                    {

                    }
                    else if (Expression.Contains(",") && x == ",")
                    {
                        string str = Expression;
                        int index = str.Length - 1;
                        while (char.IsDigit(str[index]))
                        {
                            index--;
                        }
                        if (str[index] != ',')
                            Expression += x;
                    }
                    else if ((x == "+" || x == "-" || x == "×" || x == "÷") && Expression.Length != 0)
                    {
                        char c = Expression[Expression.Length - 1];
                        if (c == x[0])
                        {

                        }
                        else if (c == '+' || c == '-' || c == '×' || c == '÷')
                        {
                            Expression = Expression.Substring(0, Expression.Length - 1) + x;
                        }
                        else if (c == '(')
                        {

                        }
                        else
                        {
                            Expression += x;
                        }

                    }
                    else
                    {
                        Expression += x;
                    }

                }

            }, x => string.IsNullOrWhiteSpace(x) == false);
        }

    }
}