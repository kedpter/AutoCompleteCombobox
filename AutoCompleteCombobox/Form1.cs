using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoCompleteCombobox
{
    public partial class Form1 : Form
    {
        public class Student
        {
            public string Name { get; set; }
            public int Index { get; set; }
        }

        public Form1()
        {
            InitializeComponent();

            Student t = new Student();
            List<Student> students = new List<Student>()
            {
                new Student() {Name = "Alex", Index=0 },
                new Student() {Name = "John", Index=0 },
                new Student() {Name = "小明", Index=0 },
                new Student() {Name = "红红", Index=0 },
                new Student() {Name = "大明", Index=0 }
            };
            autoCompleteCombobox1.DataSource = students;
            autoCompleteCombobox1.DisplayMember = "Name";
            autoCompleteCombobox1.ValueMember = "Index";

            autoCompleteCombobox1.SelectedIndex = -1;

       }
    }
}
