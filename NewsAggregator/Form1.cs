﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace NewsAggregator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DB db = new DB();
            
            //List<Article> list = db.getAllArticlesSinceDateByCategory(new DateTime(2019, 2, 22), "Sport");
            //List<Article> list = db.getAllArticlesByCategory("Najważniejsze");
            //var categories = db.getAllArticlesSinceDate(new DateTime(2019, 02, 22));
        }
    }
}
