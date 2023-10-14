using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VacuumCleaner
{
    public partial class Form1 : Form
    {
        string[] rooms = new string[] { "A", "B", "C", "D" };
        string[] rooms_status = new string[] { "Dirty", "Dirty", "Dirty", "Dirty" };
        string[] agent_rooms_status = new string[] { "NA", "NA", "NA", "NA" };
        int Current_Room = 0;
        Dictionary<string, Label> status_label = new Dictionary<string, Label>();
        Dictionary<string, PictureBox> imagebox = new Dictionary<string, PictureBox>();
        Dictionary<string, Image> imagebox_normal = new Dictionary<string, Image>();
        Dictionary<string, Image> imagebox_suck = new Dictionary<string, Image>();



        public Form1()
        {
            InitializeComponent();
            Random random = new Random();
            
            Room_B_pic.Image = null;
            Room_C_pic.Image = null;
            Room_D_pic.Image = null;

            textBox1.Text = "Start state: Room A and B are dirty, Agent starts at Room A.\r\n" +
                            "Actions: Cleaning, Move right, Move left, NoOp.\r\n" +
                            "Goal: Ensure that all rooms are clean.\r\n";
            textBox1.ForeColor = Color.Black;

            textBox1.Select(0, 0);
            status_label.Add("A", state_A);
            status_label.Add("B", state_B);
            status_label.Add("C", state_C);
            status_label.Add("D", state_D);

            imagebox.Add("A", Room_A_pic);
            imagebox.Add("B", Room_B_pic);
            imagebox.Add("C", Room_C_pic);
            imagebox.Add("D", Room_D_pic);

            imagebox_normal.Add("A", Properties.Resources.vacuum_rnr);
            imagebox_normal.Add("B", Properties.Resources.vacuum_rn);
            imagebox_normal.Add("C", Properties.Resources.vacuum_rnr);
            imagebox_normal.Add("D", Properties.Resources.vacuum_rn);

            imagebox_suck.Add("A", Properties.Resources.vacuum_s);
            imagebox_suck.Add("B", Properties.Resources.vacuum_sr);
            imagebox_suck.Add("C", Properties.Resources.vacuum_s);
            imagebox_suck.Add("D", Properties.Resources.vacuum_sr);

        }
        bool simple_clicked = false;

        private void button1_Click(object sender, EventArgs e)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            BackgroundWorker bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(bw_DoWork2);
            if (!simple_clicked)
            {//start
                button2.Enabled = false;
                button1.Text = "STOP";

                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync();
                }
                simple_clicked = !simple_clicked;
            }
            else
            {//stop
                button1.Cursor = Cursors.WaitCursor;
                simple_clicked = !simple_clicked;
                if (bw2.IsBusy != true)
                {
                    bw2.RunWorkerAsync();
                }
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {

            this.Invoke(new MethodInvoker(delegate
            {
                textBox1.AppendText("Vacuum on\r\n" +
                  "Current Room " + rooms[Current_Room] + "\r\n");
            }));
            System.Threading.Thread.Sleep(1000);
            while (simple_clicked)
            {
                end_loop = false;
                this.Invoke(new MethodInvoker(delegate
                {
                    textBox1.AppendText("Scanning...\r\n");
                }));
                System.Threading.Thread.Sleep(1000);
                if (rooms_status[Current_Room] == "Dirty")
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        textBox1.AppendText("Dirt has been found, now cleaning...\r\n");
                    }));
                    System.Threading.Thread.Sleep(1000);
                    this.Invoke(new MethodInvoker(delegate
                    {
                        Clean();
                    }));
                }
                else
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        textBox1.AppendText("Room is clean. No operation\r\n");
                    }));
                }
                System.Threading.Thread.Sleep(1000);
                this.Invoke(new MethodInvoker(delegate
                {
                    Move();
                }));
                System.Threading.Thread.Sleep(1000);
                end_loop = true;
            }

        }


        private void bw_DoWork2(object sender, DoWorkEventArgs e)
        {
            Random random = new Random();
            while (!end_loop) ;
            if (end_loop)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    button1.Text = "SIMPLE REFLEX AGENT";

                    // Reset all rooms to dirty
                    for (int i = 0; i < rooms.Length; i++)
                    {
                        rooms_status[i] = "Dirty";
                        status_label[rooms[i]].Text = "Dirty";
                        status_label[rooms[i]].ForeColor = Color.Red;
                        imagebox[rooms[i]].Image = imagebox_normal[rooms[i]];
                    }

                    // Set one random room to clean
                    int cleanRoom = random.Next(rooms.Length);
                    rooms_status[cleanRoom] = "Clean";
                    status_label[rooms[cleanRoom]].Text = "Clean";
                    status_label[rooms[cleanRoom]].ForeColor = Color.Green;

                    // Set the vacuum's position to a random room
                    Current_Room = random.Next(rooms.Length);
                    status_label[rooms[Current_Room]].Text = "Dirty";
                    status_label[rooms[Current_Room]].ForeColor = Color.Red;

                    // Set the vacuum's image to null for other rooms
                    foreach (var room in rooms.Where(r => r != rooms[Current_Room]))
                    {
                        imagebox[room].Image = null;
                    }

                    imagebox[rooms[Current_Room]].Image = imagebox_normal[rooms[Current_Room]];

                    textBox1.AppendText("Vacuum has been turned off\r\nReset Environment.\r\n\r\n");
                    button1.Cursor = Cursors.Default;
                    button2.Enabled = true;
                }));
            }
        }



        bool end_loop = false;

       
        void Clean()
        {
            imagebox[rooms[Current_Room]].Image = imagebox_suck[rooms[Current_Room]];
            textBox1.AppendText("Clean\r\n");
            rooms_status[Current_Room] = "Clean";
            status_label[rooms[Current_Room]].Text = rooms_status[Current_Room];
            status_label[rooms[Current_Room]].ForeColor = Color.Green;
            if (simple_state_clicked)
            {
                agent_rooms_status[Current_Room] = "Clean";
            }
        }

        void Move()
        {
            imagebox[rooms[Current_Room]].Image = null;

            // Move to the next room
            Current_Room++;

            // If Current_Room exceeds the last room index (3), wrap around to the first room (0)
            if (Current_Room >= rooms.Length)
            {
                Current_Room = 0;
            }

            textBox1.AppendText("Move to room " + rooms[Current_Room] + "\r\n");
            imagebox[rooms[Current_Room]].Image = imagebox_normal[rooms[Current_Room]];
        }

        private void Stop()
        {
            imagebox[rooms[Current_Room]].Image = imagebox_normal[rooms[Current_Room]];
            textBox1.AppendText("All Cleaned, turning off vacuum " + "\r\n");
        }
        bool simple_state_clicked = false;


        private void button2_Click_1(object sender, EventArgs e)
        {

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork3);
            BackgroundWorker bw2 = new BackgroundWorker();
            bw2.DoWork += new DoWorkEventHandler(bw_DoWork4);
            if (!simple_state_clicked)
            {//start
                button1.Enabled = false;
                button2.Text = "STOP";
                simple_state_clicked = !simple_state_clicked;
                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync();
                }

            }
            else
            {//stop
                button2.Cursor = Cursors.WaitCursor;
                simple_state_clicked = !simple_state_clicked;
                if (bw2.IsBusy != true)
                {
                    bw2.RunWorkerAsync();
                }
            }
        }


        private void bw_DoWork3(object sender, DoWorkEventArgs e)
        {

            this.Invoke(new MethodInvoker(delegate
            {
                textBox1.AppendText("Agent has started.\r\n" +
                  "Current Room " + rooms[Current_Room] + "\r\n");
            }));
            System.Threading.Thread.Sleep(1000);
            while (simple_state_clicked)
            {
                if (!agent_rooms_status.Contains("NA"))
                {
                    break;
                }
                end_loop = false;
                this.Invoke(new MethodInvoker(delegate
                {
                    textBox1.AppendText("Scanning...\r\n");
                }));
                System.Threading.Thread.Sleep(1000);
                if (rooms_status[Current_Room] == "Dirty")
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        textBox1.AppendText("Dirt has been found, now cleaning...\r\n");
                    }));
                    System.Threading.Thread.Sleep(1000);
                    this.Invoke(new MethodInvoker(delegate
                    {
                        Clean();
                    }));
                }
                else
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        textBox1.AppendText("Room is clean.\r\n");
                    }));
                }
                System.Threading.Thread.Sleep(1000);
                this.Invoke(new MethodInvoker(delegate
                {
                    if (agent_rooms_status.Contains("NA") || agent_rooms_status.Contains("Dirty"))
                    {
                        Move();
                    }
                    else
                    {
                        Stop();
                    }
                }));
                System.Threading.Thread.Sleep(1000);
                end_loop = true;
            }

        }

        private void bw_DoWork4(object sender, DoWorkEventArgs e)
        {
            Random random = new Random();
            while (!end_loop) ;
            if (end_loop)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    button2.Text = "WITH STATE";

                    // Reset all rooms to dirty
                    for (int i = 0; i < rooms.Length; i++)
                    {
                        rooms_status[i] = "Dirty";
                        status_label[rooms[i]].Text = "Dirty";
                        status_label[rooms[i]].ForeColor = Color.Red;
                        imagebox[rooms[i]].Image = imagebox_normal[rooms[i]];
                    }

                    // Randomly select one room to mark as clean
                    int randomCleanRoom = random.Next(rooms.Length);
                    rooms_status[randomCleanRoom] = "Clean";
                    status_label[rooms[randomCleanRoom]].Text = "Clean";
                    status_label[rooms[randomCleanRoom]].ForeColor = Color.Green;

                    // Set the vacuum's position to a random room
                    Current_Room = random.Next(rooms.Length);
                    // Make sure the current room is marked as dirty
                    rooms_status[Current_Room] = "Dirty";
                    status_label[rooms[Current_Room]].Text = "Dirty";
                    status_label[rooms[Current_Room]].ForeColor = Color.Red;

                    // Set the vacuum's image to null for other rooms
                    foreach (var room in rooms.Where(r => r != rooms[Current_Room]))
                    {
                        imagebox[room].Image = null;
                    }
                    imagebox[rooms[Current_Room]].Image = imagebox_normal[rooms[Current_Room]];

                    // Reset agent status
                    for (int i = 0; i < agent_rooms_status.Length; i++)
                    {
                        agent_rooms_status[i] = "NA";
                    }

                    // Check if all rooms are clean
                    if (rooms_status.All(status => status == "Clean"))
                    {
                        // Stop the program when all rooms are clean
                        textBox1.AppendText("All rooms are clean. Program stopped.\r\n");
                        button2.Cursor = Cursors.Default;
                        button1.Enabled = true;
                        return;
                    }

                    textBox1.AppendText("Vacuum has been turned off\r\nReset Environment.\r\n\r\n");
                    button2.Cursor = Cursors.Default;
                    button1.Enabled = true;
                }));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Room_B_pic_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void state_B_Click(object sender, EventArgs e)
        {

        }

        private void Room_D_pic_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void state_D_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
