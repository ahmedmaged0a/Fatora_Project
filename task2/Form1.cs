using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Net.Configuration;
using System.Windows.Forms;

namespace task2
{
    public partial class Form1 : Form
    {
        private SqlConnection con; 
        private SqlDataAdapter da ;
        private DataSet ds = new DataSet();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string sq = "Data Source = localhost ; initial catalog=db2; integrated security=true";
                con = new SqlConnection(sq);
            
                con.Open();
            }
            catch (Exception exception)
            {
                con.ChangeDatabase("db2");
            }
           
            
           da = new SqlDataAdapter("select * from fatora",con);
           SqlCommandBuilder CommandBuilder = new SqlCommandBuilder(da);
           da.Fill(ds, "fatora");  
           
           da = new SqlDataAdapter("select * from sanf",con);
           da.Fill(ds, "sanf");  
           
           dataGridView1.DataSource = ds.Tables["fatora"];
           DataTable t = new DataTable("t1");
           ds.Tables.Add(t);
           DataColumn c0 = new DataColumn("name",typeof(string));
           DataColumn c1 = new DataColumn("quantity",typeof(int));
           DataColumn c2 = new DataColumn("price",typeof(float));
           ds.Tables["t1"].Columns.Add(c0);
           ds.Tables["t1"].Columns.Add(c1);
           ds.Tables["t1"].Columns.Add(c2);
           dataGridView2.DataSource = ds.Tables["t1"];
        }

        public void display_data1()
        {
            
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from fatora";
                cmd.ExecuteNonQuery();
                DataTable dt  = new DataTable();
                da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dataGridView1.DataSource = dt;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(ds.Tables.Contains("fatora"))
                ds.Tables["fatora"].Clear();
            da = new SqlDataAdapter("select id , name , date , total , count from fatora where date >='"+ dateTimePicker2.Value.Date +"'and date <='"+dateTimePicker3.Value.Date+"'order by date",con);
            SqlCommandBuilder c = new SqlCommandBuilder(da);
            da.Fill(ds, "fatora");
            dataGridView1.DataSource = ds.Tables["fatora"];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                float total=0;
                DataRow r = ds.Tables["fatora"].NewRow();
                r["id"] = int.Parse(textBox1.Text);
                r["name"] = textBox2.Text;
                r["date"] = Convert.ToString(dateTimePicker1.Value.Date);
                r["count"] = ds.Tables["t1"].Rows.Count;
                
                SqlCommand cmd = new SqlCommand($"insert into fatora(id,name,date,count) values({r["id"]}, '{r["name"]}' , '{r["date"]}', {r["count"]})",con);
                cmd.ExecuteNonQuery();

                foreach (DataRow  r1 in ds.Tables["t1"].Rows)
                {
                    DataRow r2 = ds.Tables["sanf"].NewRow();

                    r2["name"] = r1["name"];
                    r2["quantity"] = r1["quantity"];
                    r2["price"] = r1["price"];
                    r2["total"] =  (Convert.ToInt32(r1["quantity"]) * Convert.ToSingle(r1["price"]));
                    r2["ID"] = int.Parse(textBox1.Text);
                    total += Convert.ToSingle(r2["total"]);
                    ds.Tables["sanf"].Rows.Add(r2);
                    SqlCommand cmd2 =
                        new SqlCommand(
                            $"insert into sanf values('{r2["name"]}',{r2["quantity"]},{r2["price"]},{r2["total"]},{r2["ID"]})",
                            con);
                    cmd2.ExecuteNonQuery();
                }

                r["total"] =total;
                SqlCommand cmd3 = new SqlCommand($"update fatora set total={total} where id={r["id"]}",con);
                cmd3.ExecuteNonQuery();
                display_data1();
                ds.Tables["t1"].Clear();
                MessageBox.Show("fatora added successfuly");
                dataGridView2.DataSource = ds.Tables["t1"];
                textBox4.Text = r["count"].ToString();
                textBox3.Text = r["total"].ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("There is an Error Fatora not added\n" + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                bool flag = false;
                if (ds.Tables.Contains("t1"))
                    ds.Tables["t1"].Clear();
                
                
                foreach (DataRow dr in ds.Tables["fatora"].Rows)
                {
                    if (Convert.ToInt32(dr["id"]) == int.Parse(textBox1.Text))
                    {
                        flag = true;
                        textBox2.Text = dr["name"].ToString();
                        textBox3.Text = dr["total"].ToString();
                        textBox4.Text = dr["count"].ToString();
                        dateTimePicker1.Text = dr["date"].ToString();
                    }

                }
                foreach (DataRow dr in ds.Tables["sanf"].Rows)
                {
                    if (Convert.ToInt32(dr["ID"]) == int.Parse(textBox1.Text))
                    {

                        DataRow r2 = ds.Tables["t1"].NewRow();
                        r2["name"] = dr["name"];
                        r2["quantity"] = dr["quantity"];
                        r2["price"] = dr["price"];
                        ds.Tables["t1"].Rows.Add(r2);
                        dataGridView2.DataSource = ds.Tables["t1"];
                    }

                }
                
                if (flag == false)
                {
                    MessageBox.Show("This Fatora not found");
                }
                }
            catch (Exception ex)

            {
                MessageBox.Show("This Fatora not found \n" + ex.Message);
            }
        }
    }
}
