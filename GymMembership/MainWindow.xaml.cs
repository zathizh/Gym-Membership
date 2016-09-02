using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using MahApps.Metro.Controls.Dialogs;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using GymMembership.Members;
using GymMembership.Payments;
using GymMembership.Measurements;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.ComponentModel;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace GymMembership
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private ObservableCollection<Member> members = new ObservableCollection<Member>();

        private ObservableCollection<Payment> payments = new ObservableCollection<Payment>();

        private ObservableCollection<Measurement> mesurements = new ObservableCollection<Measurement>();

        private string required = "*Required";

        public MainWindow()
        {
            InitializeComponent();

            monthCombo.SelectedValue = DateTime.Now.ToString("MMM").ToUpper();

            memberDataGrid.ItemsSource = members;
            selectionDataGrid.ItemsSource = members;
            paymentDataGrid.ItemsSource = payments;
            measurementDataGrid.ItemsSource = mesurements;
            updater();
            sourceCollectionUpdate();

            mContact.MaxLength = 10;
            eContact.MaxLength = 10;
            dContact.MaxLength = 10;

            regDate.SelectedDate = DateTime.Now;
            mGender.IsChecked = true;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                string selectedFileName = dlg.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);

                //bitmap.DecodePixelHeight = 100;
                //bitmap.DecodePixelWidth = 100;

                bitmap.EndInit();
                ImageControl.Source = bitmap;
            }
        }

        private int checker()
        {
            int check = 0;

            if (mFirstName.Text == "")
            {
                check = 1;
                mFirstNameAlert.Content = required;
            }
            if (mLastName.Text == "")
            {
                check = 1;
                mLastNameAlert.Content = required;

            }
            if (mContact.Text == "")
            {
                check = 1;
                mContactAlert.Content = required;
            }
            if (eName.Text == "" || eContact.Text == "")
            {
                check = 1;
                emgAlert.Content = required;
            }
            return check;
        }

        private void enableItems()
        {
            regDate.IsEnabled = true;
            mFirstName.IsEnabled = true;
            mLastName.IsEnabled = true;
            mAddr.IsEnabled = true;
            mContact.IsEnabled = true;
            mEmail.IsEnabled = true;
            mOccup.IsEnabled = true;
            mDob.IsEnabled = true;
            mGender.IsEnabled = true;
            eName.IsEnabled = true;
            eContact.IsEnabled = true;
            medications.IsEnabled = true;
            mStatus.IsEnabled = true;
            dName.IsEnabled = true;
            dContact.IsEnabled = true;
            expectation.IsEnabled = true;
        }

        private void disableItems()
        {
            regDate.IsEnabled = false;
            mFirstName.IsEnabled = false;
            mLastName.IsEnabled = false;
            mAddr.IsEnabled = false;
            mContact.IsEnabled = false;
            mEmail.IsEnabled = false;
            mOccup.IsEnabled = false;
            mDob.IsEnabled = false;
            mGender.IsEnabled = false;
            eName.IsEnabled = false;
            eContact.IsEnabled = false;
            medications.IsEnabled = false;
            mStatus.IsEnabled = false;
            dName.IsEnabled = false;
            dContact.IsEnabled = false;
            expectation.IsEnabled = false;
        }

        private void enableButtons()
        {
            BrowseButton.IsEnabled = true;
            addBtn.IsEnabled = true;
            editBtn.IsEnabled = true;
            saveBtn.IsEnabled = true;
            updateBtn.IsEnabled = true;
            deleteBtn.IsEnabled = true;
            cancelBtn.IsEnabled = true;
        }

        private void disableButtons()
        {
            BrowseButton.IsEnabled = false;
            addBtn.IsEnabled = false;
            editBtn.IsEnabled = false;
            saveBtn.IsEnabled = false;
            updateBtn.IsEnabled = false;
            deleteBtn.IsEnabled = false;
            cancelBtn.IsEnabled = false;
        }

        private void clearFlags()
        {
            mFirstNameAlert.Content = "";
            mLastNameAlert.Content = "";
            mContactAlert.Content = "";
            emgAlert.Content = "";
        }

        private void clearItems()
        {

            Image myImage3 = new Image();
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Images/placeholder_person.gif", UriKind.Relative);
            bi3.EndInit();

            ImageControl.Source = bi3;

            memID.Text = "";
            regDate.SelectedDate = DateTime.Now;

            mFirstName.Text = "";
            mLastName.Text = "";
            mAddr.Text = "";
            mContact.Text = "";
            mEmail.Text = "";
            mOccup.Text = "";
            mDob.Text = "";
            mGender.IsChecked = true;
            eName.Text = "";
            eContact.Text = "";
            medications.Text = "";
            mStatus.Text = "";
            dName.Text = "";
            dContact.Text = "";
            expectation.SelectedIndex = -1;
            expectation.Text = "";
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            disableButtons();
            BrowseButton.IsEnabled = true;
            saveBtn.IsEnabled = true;
            clearFlags();
            clearItems();
            enableItems();
            memberDataGrid.SelectedIndex = -1;
            memberDataGrid.IsEnabled = false;
            cancelBtn.IsEnabled = true;
            try
            {
                string query = "select IDENT_CURRENT('Members');";
                SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);
                sqlConnection.connection.Open();
                var identity = cmd.ExecuteScalar();

                memID.Text = string.Format("{0:00000}", (Convert.ToInt32(identity) + 1));
            }
            catch (Exception)
            {
                sqlConnection.connection.Close();
            }
            finally
            {
                sqlConnection.connection.Close();
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            clearFlags();
            clearItems();
            disableItems();
            disableButtons();
            memberDataGrid.IsEnabled = true;
            memberDataGrid.SelectedIndex = -1;
            selectionDataGrid.SelectedIndex = -1;
            addBtn.IsEnabled = true;
        }

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            clearFlags();
            int check = checker();
            if (check == 0)
            {
                disableButtons();
                try
                {
                    //string validate = "select count(*) from Members where mContact= '" + this.mContact.Text + "' ;";
                    string validate = "select count(*) from Members where mContact= @mContact";
                    SqlCommand cmd = new SqlCommand(validate, sqlConnection.connection);
                    cmd.Parameters.AddWithValue("@mContact", this.mContact.Text);


                    sqlConnection.connection.Open();
                    Int32 count = (Int32)cmd.ExecuteScalar();
                    sqlConnection.connection.Close();

                    if (count == 0)
                    {
                        int gender = (this.mGender.IsChecked == true) ? 1 : 0;
                        int year = Convert.ToInt32(this.regDate.SelectedDate.Value.Year.ToString());
                        /*
                        string query = "insert into Members (mContact, regDate, mFirstName, mLastName, mAddr, mEmail, mOccup, mDob, mGender, eName, eContact, medications, mStatus, dName, dContact) values ('" + this.mContact.Text + "', '" + this.regDate.Text + "', '" + this.mFirstName.Text + "', '" + this.mLastName.Text + "', '" + this.mAddr.Text + "', '" + this.mEmail.Text + "', '" + this.mOccup.Text + "', '" + this.mDob.Text + "', " + gender + ", '" + this.eName.Text + "', '" + this.eContact.Text + "', '" + this.medications.Text + "', '" + this.mStatus.Text + "', '" + this.dName.Text + "', '" + this.dContact.Text + "'); " +
                            "insert into Payments (mContact , year) values('" + this.mContact.Text + "', " + year + "); " +
                            "insert into Measure (mContact, year, measurements) values ('" + this.mContact.Text + "', " + year + ", 'Height'); " +
                            "insert into Measure (mContact, year, measurements) values ('" + this.mContact.Text + "', " + year + ", 'Weight'); " +
                            "insert into Measure (mContact, year, measurements) values ('" + this.mContact.Text + "', " + year + ", 'Chest'); " +
                            "insert into Measure (mContact, year, measurements) values ('" + this.mContact.Text + "', " + year + ", 'Waist Line'); " +
                            "insert into Measure (mContact, year, measurements) values ('" + this.mContact.Text + "', " + year + ", 'Over Belly Button'); " +
                            "insert into Measure (mContact, year, measurements) values ('" + this.mContact.Text + "', " + year + ", 'Right Arm'); " +
                            "insert into Measure (mContact, year, measurements) values ('" + this.mContact.Text + "', " + year + ", 'Right Leg'); ";
                        

                        string query = "insert into Members (mContact, regDate, mFirstName, mLastName, mAddr, mEmail, mOccup, mDob, mGender, eName, eContact, medications, mStatus, dName, dContact) values (@mContact, @regDate, @mFirstName, @mLastName, @mAddr, @mEmail, @mOccup, @mDob, @mGender, @eName, @eContact, @medications, @mStatus, @dName, @dContact); " +
                            "insert into Payments (mContact , year) values( @mContact, @year); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @Height); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @Weight); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @Chest); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @WaistLine); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @OverBellyButton); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @RightArm); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @RightLeg); ";
                        */

                        string query = "insert into Members (mContact, pic, regDate, mFirstName, mLastName, mAddr, mEmail, mOccup, mDob, mGender, eName, eContact, medications, mStatus, dName, dContact) values (@mContact, @pic, @regDate, @mFirstName, @mLastName, @mAddr, @mEmail, @mOccup, @mDob, @mGender, @eName, @eContact, @medications, @mStatus, @dName, @dContact, @expectation); " +
                            "insert into Payments (mContact , year) values( @mContact, @year); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @Height); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @Weight); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @Chest); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @WaistLine); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @OverBellyButton); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @RightArm); " +
                            "insert into Measure (mContact, year, measurements) values ( @mContact, @year, @RightLeg); ";

                        cmd.CommandText = query;
                        cmd.Connection = sqlConnection.connection;

                        cmd.Parameters.AddWithValue("@pic", ConvertToByteFromBitmapImage(this.ImageControl.Source as BitmapImage));
                        cmd.Parameters.AddWithValue("@regDate", this.regDate.Text);
                        cmd.Parameters.AddWithValue("@mFirstName", this.mFirstName.Text);
                        cmd.Parameters.AddWithValue("@mLastName", this.mLastName.Text);
                        cmd.Parameters.AddWithValue("@mAddr", this.mAddr.Text);
                        cmd.Parameters.AddWithValue("@mEmail", this.mEmail.Text);
                        cmd.Parameters.AddWithValue("@mOccup", this.mOccup.Text);
                        cmd.Parameters.AddWithValue("@mDob", this.mDob.Text);
                        cmd.Parameters.AddWithValue("@mGender", gender);
                        cmd.Parameters.AddWithValue("@eName", this.eName.Text);
                        cmd.Parameters.AddWithValue("@eContact", this.eContact.Text);
                        cmd.Parameters.AddWithValue("@medications", this.medications.Text);
                        cmd.Parameters.AddWithValue("@mStatus", this.mStatus.Text);
                        cmd.Parameters.AddWithValue("@dName", this.dName.Text);
                        cmd.Parameters.AddWithValue("@dContact", this.dContact.Text);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@Height", "Height");
                        cmd.Parameters.AddWithValue("@Weight", "Weight");
                        cmd.Parameters.AddWithValue("@Chest", "Chest");
                        cmd.Parameters.AddWithValue("@WaistLine", "Waist Line");
                        cmd.Parameters.AddWithValue("@OverBellyButton", "Over Belly Button");
                        cmd.Parameters.AddWithValue("@RightArm", "Right Arm");
                        cmd.Parameters.AddWithValue("@RightLeg", "Right Leg");
                        cmd.Parameters.AddWithValue("@expectation", this.expectation.Text);


                        sqlConnection.connection.Open();
                        cmd.ExecuteNonQuery();
                        sqlConnection.connection.Close();

                        disableItems();
                        showInfoDialog("Record has been Saved");

                        members.Add(new Member { memID = this.memID.Text, mContact = this.mContact.Text, mName = this.mFirstName.Text + " " + this.mLastName.Text });
                        memberDataGrid.SelectedItem = members.Where(x => x.mContact == this.mContact.Text).FirstOrDefault();
                        payments.Add(new Payment { memID = this.memID.Text, mContact = this.mContact.Text, mName = this.mFirstName.Text + " " + this.mLastName.Text, year = year });
                        paymentDataGrid.SelectedItem = payments.Where(x => x.mContact == this.mContact.Text).FirstOrDefault();
                        selectionDataGrid.SelectedItem = members.Where(x => x.mContact == this.mContact.Text).FirstOrDefault();
                    }
                    else if (count == 1)
                    {
                        var mySettings = new MetroDialogSettings()
                        {
                            AffirmativeButtonText = "Yes",
                            NegativeButtonText = "No",
                            FirstAuxiliaryButtonText = "Cancel",
                            ColorScheme = MetroDialogOptions.ColorScheme
                        };

                        MessageDialogResult result = await this.ShowMessageAsync("Info Message!", "Record for the current member has found !\nDo you want to edit this Record ?",
                            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

                        if (result == MessageDialogResult.Affirmative)
                        {
                            //string query = "select * from Members where mContact ='" + this.mContact.Text + "';";
                            string query = "select * from Members where mContact = @mContact";


                            cmd.CommandText = query;
                            cmd.Parameters.AddWithValue("@mContact", this.mContact.Text);

                            cmd.Connection = sqlConnection.connection;

                            sqlConnection.connection.Open();
                            SqlDataReader reader = cmd.ExecuteReader();
                            readDB(reader);
                            sqlConnection.connection.Close();

                            disableButtons();
                            disableItems();
                            editBtn.IsEnabled = true;
                            deleteBtn.IsEnabled = true;

                            memberDataGrid.SelectedItem = members.Where(x => x.mContact == this.mContact.Text).FirstOrDefault();
                            memberDataGrid.IsEnabled = true;
                        }

                    }
                }
                catch (Exception ex)
                {
                    string query = "delete from Members where mContact= @mContact; " +
                                   "delete from Payments where mContact= @mContact; ";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);
                    cmd.Parameters.AddWithValue("@mContact", this.mContact.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(ex.Message);
                    saveBtn.IsEnabled = true;
                }
                finally
                {
                    sqlConnection.connection.Close();
                }
            }
            cancelBtn.IsEnabled = true;
        }

        private void readDB(SqlDataReader reader)
        {
            reader.Read();



            var k = reader["pic"];
            byte[] byteArray= null;
            if (reader["pic"] != System.DBNull.Value)
            {
                byteArray = (byte[])reader["pic"];
            }

            if (byteArray != null)
            {
                ImageControl.Source = ConvertToBitmapImageFromByteArray(byteArray);
            }

            memID.Text = string.Format("{0:00000}", Convert.ToInt32(reader["memID"]));
            regDate.Text = reader["regDate"].ToString().Trim();
            mFirstName.Text = reader["mFirstName"].ToString().Trim();
            mLastName.Text = reader["mLastName"].ToString().Trim();
            mAddr.Text = reader["mAddr"].ToString().Trim();
            mContact.Text = reader["mContact"].ToString().Trim();
            mEmail.Text = reader["mEmail"].ToString().Trim();
            mOccup.Text = reader["mOccup"].ToString().Trim();
            mDob.Text = reader["mDob"].ToString().Trim();

            bool gender = Convert.ToInt32(reader["mGender"]) == 0 ? false : true;
            mGender.IsChecked = gender;

            eName.Text = reader["eName"].ToString().Trim();
            eContact.Text = reader["eContact"].ToString().Trim();
            medications.Text = reader["medications"].ToString().Trim();
            mStatus.Text = reader["mStatus"].ToString().Trim();
            dName.Text = reader["dName"].ToString().Trim();
            dContact.Text = reader["dContact"].ToString().Trim();
            expectation.SelectedValue = reader["expectation"].ToString().Trim();
        }

        private void updater()
        {
            try
            {
                int year = Convert.ToInt32(DateTime.Now.Year.ToString());
                //string query = "select mContact from Payments where year != " + year + " ;";
                string query = "select mContact from Payments where year != @year";

                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);
                cmd.Parameters.AddWithValue("@year", year);
                
                adapter.SelectCommand = cmd;
                DataSet creater = new DataSet();
                adapter.Fill(creater);

                foreach (DataRow row in creater.Tables[0].Rows)
                {
                    //query = "insert into Payments (mContact, year) values ('" + row[0].ToString().Trim() + "', " + year + ");";
                    query = "insert into Payments (mContact, year) values ( @mContact, @year)";
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@mContact", row[0].ToString().Trim());
                    cmd.Parameters.AddWithValue("@year", year);

                    //SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);
                    
                    sqlConnection.connection.Open();
                    cmd.ExecuteNonQuery();
                    sqlConnection.connection.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        private void sourceCollectionUpdate()
        {
            try
            {
                string query = "SELECT Payments.mContact, Members.memID, Members.mFirstName, Members.mLastName, Payments.year, Payments.jan, Payments.feb, Payments.mar, Payments.apr, Payments.may, Payments.jun, Payments.jul, Payments.aug, Payments.sep , Payments.oct, Payments.nov, Payments.dec FROM Payments inner join Members on Payments.mContact = Members.mContact";

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(query, sqlConnection.connection);
                DataSet userDataSet = new DataSet();

                adapter.Fill(userDataSet);

                foreach (DataRow row in userDataSet.Tables[0].Rows)
                {

                    string mContact = row[0].ToString().Trim();
                    int mem = Convert.ToInt32(row[1].ToString().Trim());
                    string mN = row[2].ToString().Trim() + " " + row[3].ToString().Trim();

                    Member member = new Member()
                    {
                        memID = string.Format("{0:00000}", mem),
                        mContact = mContact,
                        mName = mN
                    };
                    members.Add(member);

                    Payment payment = new Payment()
                    {
                        memID = string.Format("{0:00000}", mem),
                        mContact = mContact,
                        year = Convert.ToInt32(row[4].ToString().Trim()),
                        mName = mN,
                        jan = row[5].ToString(),
                        feb = row[6].ToString(),
                        mar = row[7].ToString(),
                        apr = row[8].ToString(),
                        may = row[9].ToString(),
                        jun = row[10].ToString(),
                        jul = row[11].ToString(),
                        aug = row[12].ToString(),
                        sep = row[13].ToString(),
                        oct = row[14].ToString(),
                        nov = row[15].ToString(),
                        dec = row[16].ToString()
                    };

                    payments.Add(payment);
                }
            }
            catch (Exception)
            {

            }
        }

        private async void showInfoDialog(string msg)
        {
            this.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            var controller = await this.ShowProgressAsync("Info Message", "\n\n" + msg);
            await Task.Delay(2750);
            await controller.CloseAsync();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {

            if (memberDataGrid.SelectedIndex == -1)
            {
                enableItems();
            }
            else
            {
                addBtn.IsEnabled = false;
                editBtn.IsEnabled = false;
                updateBtn.IsEnabled = true;
                deleteBtn.IsEnabled = true;
                BrowseButton.IsEnabled = true;
                enableItems();
                this.mContact.IsEnabled = false;
                memberDataGrid.IsEnabled = false;
            }
        }

        private async void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    FirstAuxiliaryButtonText = "Cancel",
                    ColorScheme = MetroDialogOptions.ColorScheme
                };

                MessageDialogResult result = await this.ShowMessageAsync("Warrning!", "Do you want do delete this record ?",
                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

                if (result == MessageDialogResult.Affirmative)
                {
                    string query = "";
                    string con = "";


                    if (editBtn.IsEnabled == false || this.mContact.Text != "")
                    {
                        con = this.mContact.Text;
                        /*
                        query = "delete from Members where mContact='" + con + "'; " +
                            "delete from Payments where mContact='" + this.mContact.Text + "'; " +
                            "delete from Measure where mContact='" + this.mContact.Text + "'; ";
                         */
                        query = "delete from Members where mContact= @con; " +
                            "delete from Payments where mContact= @mContact; " +
                            "delete from Measure where mContact= @mContact; ";

                    }
                    else if (editBtn.IsEnabled == true)
                    {

                        Member eMember = memberDataGrid.SelectedItem as Member;
                        con = eMember.mContact;
                        string mName = eMember.mName;

                        /*
                        query = "delete from Members where mContact='" + con + "'; " +
                            "delete from Payments where mContact='" + con + "'; " +
                            "delete from Measure where mContact='" + con + "'; ";
                        */

                        query = "delete from Members where mContact= @con; " +
                            "delete from Payments where mContact= @con; " +
                            "delete from Measure where mContact= @con; ";

                    }

                    SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);

                    cmd.Parameters.AddWithValue("@con", con);
                    cmd.Parameters.AddWithValue("@mContact", this.mContact.Text);

                    sqlConnection.connection.Open();

                    cmd.ExecuteNonQuery();

                    members.Remove(memberDataGrid.SelectedItem as Member);

                    payments.Remove(payments.Where(x => x.mContact == con).FirstOrDefault());

                    showInfoDialog("Record has been deleted");

                    cancelBtn_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.connection.Close();
            }
        }

        private async void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    FirstAuxiliaryButtonText = "Cancel",
                    ColorScheme = MetroDialogOptions.ColorScheme
                };

                MessageDialogResult result = await this.ShowMessageAsync("Warrning!", "Do you want do update this record ?",
                    MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

                if (result == MessageDialogResult.Affirmative)
                {
                    int gender = (this.mGender.IsChecked == true) ? 1 : 0;

                    //string query = "update Members set regDate='" + this.regDate.Text + "', mFirstName='" + this.mFirstName.Text + "', mLastname='" + this.mLastName.Text + "', mAddr='" + this.mAddr.Text + "', mContact='" + this.mContact.Text + "', mEmail='" + this.mEmail.Text + "', mOccup='" + this.mOccup.Text + "', mDob='" + this.mDob.Text + "', mGender=" + gender + ", eName='" + this.eName.Text + "', eContact='" + this.eContact.Text + "', medications='" + this.medications.Text + "', mStatus='" + this.mStatus.Text + "', dName='" + this.dName.Text + "', dContact='" + this.dContact.Text + "' where mContact='" + this.mContact.Text + "';";
                    
                    string query = "update Members set pic= @pic, regDate= @regDate, mFirstName= @mFirstName, mLastname= @mLastName, mAddr= @mAddr, mContact= @mContact, mEmail= @mEmail, mOccup= @mOccup, mDob= @mDob, mGender= @mGender, eName= @eName, eContact= @eContact, medications= @medications, mStatus= @mStatus, dName= @dName, dContact= @dContact, expectation= @expectation where mContact=@mContact";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);
                    cmd.Parameters.AddWithValue("@pic", ConvertToByteFromBitmapImage(this.ImageControl.Source as BitmapImage));
                    cmd.Parameters.AddWithValue("@regDate", this.regDate.Text);
                    cmd.Parameters.AddWithValue("@mFirstName", this.mFirstName.Text);
                    cmd.Parameters.AddWithValue("@mLastName", this.mLastName.Text);
                    cmd.Parameters.AddWithValue("@mAddr", this.mAddr.Text);
                    cmd.Parameters.AddWithValue("@mContact", this.mContact.Text);
                    cmd.Parameters.AddWithValue("@mEmail", this.mEmail.Text);
                    cmd.Parameters.AddWithValue("@mOccup", this.mOccup.Text);
                    cmd.Parameters.AddWithValue("@mDob", this.mDob.Text);
                    cmd.Parameters.AddWithValue("@mGender", gender);
                    cmd.Parameters.AddWithValue("@eName", this.eName.Text);
                    cmd.Parameters.AddWithValue("@eContact", this.eContact.Text);
                    cmd.Parameters.AddWithValue("@medications", this.medications.Text);
                    cmd.Parameters.AddWithValue("@mStatus", this.mStatus.Text);
                    cmd.Parameters.AddWithValue("@dName", this.dName.Text);
                    cmd.Parameters.AddWithValue("@dContact", this.dContact.Text);
                    cmd.Parameters.AddWithValue("@expectation", this.expectation.Text);

                    sqlConnection.connection.Open();
                    cmd.ExecuteNonQuery();

                    updateBtn.IsEnabled = false;
                    disableItems();

                    int year = Convert.ToInt32(DateTime.Today.Year);

                    members.Remove(memberDataGrid.SelectedItem as Member);
                    payments.Remove(payments.Where(x => x.mContact == this.mContact.Text).FirstOrDefault());
                    showInfoDialog("Record has been Updated !");
                    members.Add(new Member { memID = this.memID.Text, mContact = this.mContact.Text, mName = this.mFirstName.Text + " " + this.mLastName.Text });
                    payments.Add(new Payment { memID = this.memID.Text, year=year, mContact = this.mContact.Text, mName = this.mFirstName.Text + " " + this.mLastName.Text });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.connection.Close();
            }
        }

        private void memberDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (memberDataGrid.SelectedIndex != -1)
            {
                editBtn.IsEnabled = true;
                deleteBtn.IsEnabled = true;
                cancelBtn.IsEnabled = true;
                clearItems();

                try
                {
                    Member eMember = memberDataGrid.SelectedItem as Member;
                    string con = eMember.mContact;
                    selectionDataGrid.SelectedItem = members.Where(x => x.mContact == eMember.mContact).FirstOrDefault();

                    string query = "select * from Members where mContact= @con ";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);

                    cmd.Parameters.AddWithValue("@con", con);

                    sqlConnection.connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    readDB(reader);
                    sqlConnection.connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void memberFilterByTextInput(object sender, TextChangedEventArgs e)
        {
            try
            {
                cancelBtn.IsEnabled = true;
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(memberDataGrid.ItemsSource);
                if (filter == "")
                {
                    cv.Filter = null;
                }
                else
                {
                    cv.Filter = o =>
                    {
                        Member p = o as Member;
                        if (t.Name == "memBox")
                        {
                            return (Convert.ToInt32(p.memID) == Convert.ToInt32(filter));
                        }
                        if (t.Name == "contactBox")
                        {
                            return (p.mContact.ToUpper().StartsWith(filter.ToUpper()));
                        }
                        return (p.mName.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch (FormatException)
            {
            }

        }

        private void paymentFilterByTextInput(object sender, TextChangedEventArgs e)
        {
            try
            {
                cancelBtn.IsEnabled = true;
                TextBox t = (TextBox)sender;
                string filter = t.Text;
                ICollectionView cv = CollectionViewSource.GetDefaultView(paymentDataGrid.ItemsSource);
                if (filter == "")
                {
                    cv.Filter = null;
                }
                else
                {
                    cv.Filter = o =>
                    {
                        Payment p = o as Payment;
                        if (t.Name == "memBox")
                        {
                            return (Convert.ToInt32(p.memID) == Convert.ToInt32(filter));
                            //return (p.memID.ToString().StartsWith(filter.ToUpper()));
                        }
                        if (t.Name == "contactBox")
                        {
                            return (p.mContact.ToUpper().StartsWith(filter.ToUpper()));
                        }
                        if (t.Name == "yearBox")
                        {
                            return (p.year.ToString().StartsWith(filter));
                        }
                        return (p.mName.ToUpper().StartsWith(filter.ToUpper()));
                    };
                }
            }
            catch (FormatException)
            {
            }

        }

        private void paymentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            monthCombo.IsEnabled = true;
            validateBtn.IsEnabled = true;
            unValidateBtn.IsEnabled = true;
        }

        private void validateBtn_Click(object sender, RoutedEventArgs e)
        {
            Payment ePayment = paymentDataGrid.SelectedItem as Payment;
            int year = Convert.ToInt32(DateTime.Now.Year.ToString());
            string month = monthCombo.SelectedValue.ToString().ToLower();

               
            string paymentDate = DateTime.Today.ToString("dd MMM  yyyy");
                //string query = "update Payments set " + month + "= '" + paymentDate + "' where mContact='" + ePayment.mContact + "' and year=" + year + ";";

            string query = "update Payments set " + month + "= @paymentDate where mContact= @mContact and year= @year";
            SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);

            cmd.Parameters.AddWithValue("@paymentDate", paymentDate);
            cmd.Parameters.AddWithValue("@mContact", ePayment.mContact);
            cmd.Parameters.AddWithValue("@year", year);


            sqlConnection.connection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.connection.Close();

            payments.Remove(paymentDataGrid.SelectedItem as Payment);
            payments.Add(upt(ePayment, month, paymentDate));
            paymentDataGrid.SelectedItem = payments.Where(x => x.mContact == ePayment.mContact).FirstOrDefault();
            validateBtn.IsEnabled = false;
        }

        private void unValidateBtn_Click(object sender, RoutedEventArgs e)
        {
            Payment ePayment = paymentDataGrid.SelectedItem as Payment;
            int year = Convert.ToInt32(DateTime.Now.Year.ToString());
            string month = monthCombo.SelectedValue.ToString().ToLower();

            //string query = "update Payments set " + month + "= '' where mContact='" + ePayment.mContact + "' and year=" + year + ";";

            string query = "update Payments set " + month + "= @null where mContact= @mContact and year= @year";

            SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);
            cmd.Parameters.AddWithValue("@null", "");
            cmd.Parameters.AddWithValue("@mContact", ePayment.mContact);
            cmd.Parameters.AddWithValue("@year", year);
            
            sqlConnection.connection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.connection.Close();

            payments.Remove(paymentDataGrid.SelectedItem as Payment);
            payments.Add(npt(ePayment, month));
            paymentDataGrid.SelectedItem = payments.Where(x => x.mContact == ePayment.mContact).FirstOrDefault();
            unValidateBtn.IsEnabled = false;
        }

        private Payment upt(Payment ePayment, string month, string date)
        {
            switch (month)
            {
                case "jan":
                    ePayment.jan = date;
                    break;
                case "feb":
                    ePayment.feb = date;
                    break;
                case "mar":
                    ePayment.mar = date;
                    break;
                case "apr":
                    ePayment.apr = date;
                    break;
                case "may":
                    ePayment.may = date;
                    break;
                case "jun":
                    ePayment.jun = date;
                    break;
                case "jul":
                    ePayment.jul = date;
                    break;
                case "aug":
                    ePayment.aug = date;
                    break;
                case "sep":
                    ePayment.sep = date;
                    break;
                case "oct":
                    ePayment.oct = date;
                    break;
                case "nov":
                    ePayment.nov = date;
                    break;
                case "dec":
                    ePayment.dec = date;
                    break;
            }
            return ePayment;
        }

        private Payment npt(Payment ePayment, string month)
        {
            switch (month)
            {
                case "jan":
                    ePayment.jan = "";
                    break;
                case "feb":
                    ePayment.feb = "";
                    break;
                case "mar":
                    ePayment.mar = "";
                    break;
                case "apr":
                    ePayment.apr = "";
                    break;
                case "may":
                    ePayment.may = "";
                    break;
                case "jun":
                    ePayment.jun = "";
                    break;
                case "jul":
                    ePayment.jul = "";
                    break;
                case "aug":
                    ePayment.aug = "";
                    break;
                case "sep":
                    ePayment.sep = "";
                    break;
                case "oct":
                    ePayment.oct = "";
                    break;
                case "nov":
                    ePayment.nov = "";
                    break;
                case "dec":
                    ePayment.dec = "";
                    break;
            }
            return ePayment;
        }

        private void selectionDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mesurements.Clear();

            if (selectionDataGrid.SelectedIndex != -1)
            {
                Member eMember = selectionDataGrid.SelectedItem as Member;
                string con = eMember.mContact;
                int year = Convert.ToInt32(this.regDate.SelectedDate.Value.Year.ToString());

                try
                {
                    //string query = "select * from Measure where mContact='" + con + "' and year=" + year + ";";
                    
                    string query = "select * from Measure where mContact= @con and year= @year";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);

                    cmd.Parameters.AddWithValue("@con", con);
                    cmd.Parameters.AddWithValue("@year", year);

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    DataSet userDataSet = new DataSet();
                    DataSet measurementDataSet = new DataSet();

                    adapter.Fill(measurementDataSet);

                    foreach (DataRow row in measurementDataSet.Tables[0].Rows)
                    {
                        string s1 = row[3].ToString();
                        string s2 = row[4].ToString();
                        string s3 = row[5].ToString();
                        string s4 = row[6].ToString();
                        string s5 = row[7].ToString();
                        string s6 = row[8].ToString();
                        string s7 = row[9].ToString();
                        string s8 = row[10].ToString();
                        string s9 = row[11].ToString();
                        string s10 = row[12].ToString();
                        string s11 = row[13].ToString();
                        string s12 = row[14].ToString();

                        Measurement eMesurement = new Measurement
                        {
                            mContact = row[0].ToString(),
                            year = Convert.ToInt32(row[1]),
                            measurements = row[2].ToString(),

                            set1 = (s1 != null) ? ((s1 == "0") ? "" : s1) : "",
                            set2 = (s2 != null) ? ((s2 == "0") ? "" : s2) : "",
                            set3 = (s3 != null) ? ((s3 == "0") ? "" : s3) : "",
                            set4 = (s4 != null) ? ((s4 == "0") ? "" : s4) : "",
                            set5 = (s5 != null) ? ((s5 == "0") ? "" : s5) : "",
                            set6 = (s6 != null) ? ((s6 == "0") ? "" : s6) : "",
                            set7 = (s7 != null) ? ((s7 == "0") ? "" : s7) : "",
                            set8 = (s8 != null) ? ((s8 == "0") ? "" : s8) : "",
                            set9 = (s9 != null) ? ((s9 == "0") ? "" : s9) : "",
                            set10 = (s10 != null) ? ((s10 == "0") ? "" : s10) : "",
                            set11 = (s11 != null) ? ((s11 == "0") ? "" : s11) : "",
                            set12 = (s12 != null) ? ((s12 == "0") ? "" : s12) : "",
                            /*
                            set1 = (s1 != null) ? s1 : "",
                            */
                        };
                        mesurements.Add(eMesurement);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void measurementUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Measurement eMesurement in mesurements)
                {
                    double s1 = (eMesurement.set1 == "") ? 0 : Convert.ToDouble(eMesurement.set1);
                    double s2 = (eMesurement.set2 == "") ? 0 : Convert.ToDouble(eMesurement.set2);
                    double s3 = (eMesurement.set3 == "") ? 0 : Convert.ToDouble(eMesurement.set3);
                    double s4 = (eMesurement.set4 == "") ? 0 : Convert.ToDouble(eMesurement.set4);
                    double s5 = (eMesurement.set5 == "") ? 0 : Convert.ToDouble(eMesurement.set5);
                    double s6 = (eMesurement.set6 == "") ? 0 : Convert.ToDouble(eMesurement.set6);
                    double s7 = (eMesurement.set7 == "") ? 0 : Convert.ToDouble(eMesurement.set7);
                    double s8 = (eMesurement.set8 == "") ? 0 : Convert.ToDouble(eMesurement.set8);
                    double s9 = (eMesurement.set9 == "") ? 0 : Convert.ToDouble(eMesurement.set9);
                    double s10 = (eMesurement.set10 == "") ? 0 : Convert.ToDouble(eMesurement.set10);
                    double s11 = (eMesurement.set11 == "") ? 0 : Convert.ToDouble(eMesurement.set11);
                    double s12 = (eMesurement.set12 == "") ? 0 : Convert.ToDouble(eMesurement.set12);

                    //string query = "update Measure set set1=" + s1 + ", set2=" + s2 + ", set3=" + s3 + ", set4=" + s4 + ", set5=" + s5 + ", set6=" + s6 + ", set7=" + s7 + ", set8=" + s8 + ", set9=" + s9 + ", set10=" + s10 + ", set11=" + s11 + ", set12=" + s12 + " where mContact ='" + eMesurement.mContact + "' and year=" + eMesurement.year + " and measurements='" + eMesurement.measurements + "';";
                    string query = "update Measure set set1= @s1, set2= @s2, set3= @s3, set4= @s4 , set5= @s5 , set6= @s6 , set7= @s7, set8= @s8, set9= @s9, set10= @s10, set11= @s11, set12= @s12 where mContact = @mContact and year= @year and measurements=@measurements";
                    SqlCommand cmd = new SqlCommand(query, sqlConnection.connection);
                    cmd.Parameters.AddWithValue("@s1", s1);
                    cmd.Parameters.AddWithValue("@s2", s2);
                    cmd.Parameters.AddWithValue("@s3", s3);
                    cmd.Parameters.AddWithValue("@s4", s4);
                    cmd.Parameters.AddWithValue("@s5", s5);
                    cmd.Parameters.AddWithValue("@s6", s6);
                    cmd.Parameters.AddWithValue("@s7", s7);
                    cmd.Parameters.AddWithValue("@s8", s8);
                    cmd.Parameters.AddWithValue("@s9", s9);
                    cmd.Parameters.AddWithValue("@s10", s10);
                    cmd.Parameters.AddWithValue("@s11", s11);
                    cmd.Parameters.AddWithValue("@s12", s12);
                    cmd.Parameters.AddWithValue("@mContact", eMesurement.mContact);
                    cmd.Parameters.AddWithValue("@year", eMesurement.year);
                    cmd.Parameters.AddWithValue("@measurements", eMesurement.measurements);

                    sqlConnection.connection.Open();
                    cmd.ExecuteNonQuery();
                    sqlConnection.connection.Close();
                    Member eMember = selectionDataGrid.SelectedItem as Member;
                    measurementUpdateBtn.IsEnabled = false;
                    showInfoDialog("Measurement Records for the " + eMember.mName + " has been updated");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /*
        public void mailItem()
        {
            Debug.WriteLine("Sathish");
            if (this.mEmail.Text != "")
            {
                Outlook.Application outlookApp = new Outlook.Application();
                Outlook.MailItem mailItem = (Outlook.MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);
                mailItem.To = this.mEmail.Text;
            }
        }
         * */

        private void measurementDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (measurementUpdateBtn.IsEnabled == false)
            {
                measurementUpdateBtn.IsEnabled = true;
            }
        }

        private void monthCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (paymentDataGrid.SelectedIndex != -1)
            {
                validateBtn.IsEnabled = true;
                unValidateBtn.IsEnabled = true;
            }
        }

        public static Byte[] ConvertToByteFromBitmapImage(BitmapImage bitmapImage)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        private static BitmapImage ConvertToBitmapImageFromByteArray(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
