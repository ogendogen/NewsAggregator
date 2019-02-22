using System;
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
            List<Article> list = db.getAllArticlesByCategory("Biznes");
            list.ForEach(x => Debug.WriteLine(x.Content + "ddd"));
        }
    }
}
