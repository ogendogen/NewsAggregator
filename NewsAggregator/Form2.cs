using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace NewsAggregator
{
    public partial class Form2 : Form
    {
        private DB db;
        public Form2(DB db)
        {
            InitializeComponent();
            foreach (var source in db.getAllSources())
            {
                if (source.Name == null) continue;
                comboBox1.Items.Add(source);
            }
            this.db = db;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;
            Source source = (Source)comboBox1.SelectedItem;
            this.textBox1.Text = source.Category;
            this.textBox2.Text = source.URL;
            this.textBox3.Text = source.Name;
        }

        private void button1_Click(object sender, EventArgs e) // dodawanie
        {
            try
            {
                if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox3.Text)) throw new Exception("Jedno z pól jest puste!");
                string url = textBox2.Text;
                if (!Regex.IsMatch(url, @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9]\.[^\s]{2,})")) throw new Exception("Format URL jest niepoprawny!");

                Source source = new Source();
                source.Category = textBox1.Text;
                source.URL = textBox2.Text;
                source.Name = textBox3.Text;
                source.LastBuildDate = DateTime.Now.ToString("ddd, dd MMM yy HH:mm:ss", new CultureInfo("en-US"));
                db.addNewSource(source);
                MessageBox.Show("Kanał dodany!", "Powodzenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (MongoWriteException)
            {
                MessageBox.Show("Taka nazwa już istnieje!", "Uwaga!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("Kanał dodany!", "Powodzenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e) // usuwanie
        {
            try
            {
                if (comboBox1.SelectedItem == null) throw new Exception("Wybierz kanał z listy!");
                DialogResult result = MessageBox.Show("Czy na pewno chcesz usunąć ten kanał ?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;

                db.removeSource((Source)comboBox1.SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
