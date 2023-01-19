using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WordGuessr.Database;

namespace WordGuessr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly MongoConnection Connection = new("mongodb://localhost", "WordGuessr");
    }
}