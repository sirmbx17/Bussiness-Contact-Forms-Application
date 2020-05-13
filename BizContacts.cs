using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Forms
{
    public partial class BizContacts : Form
    {
       
        string connString = @"Data Source=DESKTOP-DK43NB3\SQLEXPRESS;Initial Catalog=AddressBook;Integrated Security=True;
                Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        SqlDataAdapter dataAdapter; // builds connection between program and database
        DataTable table;// holds table data from database that we can use to populate datagridview.
        SqlCommandBuilder commandBuilder;
        SqlConnection conn;
        string selstatement = "Select * from BizContacts";


        public BizContacts()
        {
            InitializeComponent();
        }

        private void BizContacts_Load(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0;// select first item in combox when form loads
            dataGridView1.DataSource = bindingSource1;//sets datagrid to get data from bidning source to display


            GetData(selstatement);
        }

        private void GetData(string selectCommand)
        {
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connString);
                table = new DataTable(); // create  new datatable object

                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                bindingSource1.DataSource = table; // set table as binding sources data source.
                dataGridView1.Columns[0].ReadOnly = true; // makes id column read only.
                
            }
            catch (SqlException X)
            {
                MessageBox.Show(X.Message); // displays error messages to user.
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SqlCommand command;
            string insert = @"insert into BizContacts(Date_Added , Company,Website,Title,First_Name,Last_Name,
                        Address,City,State,ZipCode,Mobile,Notes,Image)

                    values(@Date_Added,@Company,@Website,@Title,@First_Name,@Last_Name,@Address,@City,@State,@ZipCode,@Mobile,@Notes,@Image)";

            using ( conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    command = new SqlCommand(insert, conn);
                    command.Parameters.AddWithValue(@"Date_Added", dateTimePicker1.Value.Date);
                    command.Parameters.AddWithValue(@"Company", txtCompany.Text);
                    command.Parameters.AddWithValue(@"Website", txtWebsite.Text);
                    command.Parameters.AddWithValue(@"Title", txtTitle.Text);
                    command.Parameters.AddWithValue(@"First_Name", txtFirstName.Text);
                    command.Parameters.AddWithValue(@"Last_Name", txtLastName.Text);
                    command.Parameters.AddWithValue(@"Address", txtAddress.Text);
                    command.Parameters.AddWithValue(@"City", txtCity.Text);
                    command.Parameters.AddWithValue(@"State", txtState.Text);
                    command.Parameters.AddWithValue(@"ZipCode", txtZipCode.Text);
                    command.Parameters.AddWithValue(@"Mobile", txtMobile.Text);
                    command.Parameters.AddWithValue(@"Notes", txtNotes.Text);
                    if (openFlDlg.FileName != "")
                        command.Parameters.AddWithValue(@"Image", File.ReadAllBytes(openFlDlg.FileName)); // adds image by converting them to binary data.
                    else
                        command.Parameters.Add(@"Image", SqlDbType.VarBinary).Value = DBNull.Value;//allow for null entry in db.
                    command.ExecuteNonQuery();
                }
                catch(Exception y)
                {
                    MessageBox.Show(y.Message);
                }
                   
            }
            GetData(selstatement);
            dataGridView1.Update();
            }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();

            try
            {
                bindingSource1.EndEdit();
                dataAdapter.Update(table);
                MessageBox.Show("update was successful");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand(); //use updatecommand 

            try
            {
                bindingSource1.EndEdit(); // updates table 
                dataAdapter.Update(table); // updates database
                MessageBox.Show(" updated successfully"); // displays when  update is successful.
            }
            catch(Exception z)
            {
                MessageBox.Show(z.Message); // displays any error to user
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentCell.OwningRow;// select current row for reference
            string val = row.Cells["ID"].Value.ToString();// grab value of the ID field of selected record
            string fname = row.Cells["First_Name"].Value.ToString();// grab the value of fname field of selected record
            string lname = row.Cells["Last_Name"].Value.ToString();// grab the value of lname field of the selected record
            DialogResult result = MessageBox.Show("Do you want to delete record" + fname + "" + lname + ",record" + val, "Message"
                , MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            string delstmnt = @"Delete from BizContacts where id = '" + val + "'"; //deletes record with id value

            if(result==DialogResult.Yes)//runs when user wants to del record.
            {
                {
                    using (conn = new SqlConnection(connString))
                    {
                        try
                        {
                            conn.Open();//tries to open conn to db
                            SqlCommand connt = new SqlCommand(delstmnt, conn);
                            connt.ExecuteNonQuery();//executes sql query
                            GetData(selstatement);
                            dataGridView1.Update(); // update table to show current data
                        }
                        catch (Exception xx)
                        {
                            MessageBox.Show(xx.Message);// displays if not able to connect to db.
                        }

                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            switch (cboSearch.SelectedItem.ToString())
            {
                case "First Name":
                    GetData("select * from bizcontacts where lower(first_name) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
                case "Last Name":
                    GetData("select * from bizcontacts where lower(last_name) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
                case "Company":
                    GetData("select * from bizcontacts where lower(company) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
            }
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            openFlDlg.ShowDialog();
            pictureBox1.Load(openFlDlg.FileName);
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Form form = new Form();//create new form
            form.BackgroundImage = pictureBox1.Image;
            form.Size = pictureBox1.Image.Size;
            form.Show();
        }
    }
 }
     
       

     





      

     

       
      

          

  