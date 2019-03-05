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
using System.IO;

namespace NewsAggregator
{
    public partial class Form1 : Form
    {
        private DB db = new DB();
        private FileManager fileManager;
        private Dictionary<string, List<string>> allArticles = new Dictionary<string, List<string>>();
        public Form1()
        {
            InitializeComponent();
            var categories = db.getAllCategories().Where(x => x != null).ToList();
            fileManager = new FileManager(categories);
            this.comboBox1.Items.AddRange(categories.ToArray()); // add categories
            foreach (var category in categories)
            {
                List<string> articles = fileManager.getAllArticlesTitlesByCategory(category);
                allArticles.Add(category, articles);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Synchronizer.synchronize(db, fileManager);
            synchronizeUI();
            MessageBox.Show("Synchronizacja zakończona!", "Powodzenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedItem == null) throw new Exception("Nie wybrano kategorii!");
                if (listBox1.SelectedItem == null) throw new Exception("Nie wybrano artykułu!");
                string title = listBox1.SelectedItem.ToString();
                string category = comboBox1.SelectedItem.ToString();
                string content = fileManager.getArticleContentByTitleAndCategory(title, category);

                string file_to_open = fileManager.moveFileToTemp(title, content);
                Process.Start(file_to_open);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);   
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            List<string> currentArticles = allArticles[comboBox1.Text];
            foreach (var currentArticle in currentArticles)
            {
                listBox1.Items.Add(currentArticle);
            }
        }

        private void synchronizeUI()
        {
            var categories = db.getAllCategories().Where(x => x != null).ToList();
            allArticles.Clear();
            foreach (var category in categories)
            {
                List<string> articles = fileManager.getAllArticlesTitlesByCategory(category);
                allArticles.Add(category, articles);
            }

            try
            {
                listBox1.Items.Clear();
                List<string> currentArticles = allArticles[comboBox1.Text];
                foreach (var currentArticle in currentArticles)
                {
                    listBox1.Items.Add(currentArticle);
                }
            }
            catch(KeyNotFoundException)
            {
                return;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private void kanałyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2(db);
            form.Show();
        }
    }
}
