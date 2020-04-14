using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Brownian_Motion_Visualisation.Forms;

namespace Brownian_Motion_Visualisation
{
    public partial class MainWindow : Form
    {
        private Random _rand = new Random();
        private Task[] _tasks;
        private int _molNum;

        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _token;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void TempBar_Scroll(object sender, EventArgs e)
        {
            TempLbl.Text = TempBar.Value.ToString();
            Particle.AdjustTemp(TempBar.Value);
        }

        public void StopBtn_Click(object sender, EventArgs e)
        {
            NumOfMolecules.Enabled = true;
            StartBtn.Enabled = true;
            StopBtn.Enabled = false;
            _cancelTokenSource.Cancel();

            Task.WaitAll();
            Particle.Clear();
        }


        public void StartBtn_Click(object sender, EventArgs e)
        {
            NumOfMolecules.Enabled = false;
            StartBtn.Enabled = false;
            StopBtn.Enabled = true;
            _cancelTokenSource = new CancellationTokenSource();
            _token = _cancelTokenSource.Token;
            _molNum = (int)NumOfMolecules.Value;

            Particle.SetUp(_molNum);
            _tasks = new Task[_molNum];

            for (int i = 0; i < _molNum; ++i)
            {
                _tasks[i] = Task.Run(() => ControlMolecule());
            }
        }

        public void ControlMolecule()
        {
            Particle mol = new Particle(
                      _rand.Next(0, this.ClientSize.Width),
                      _rand.Next(0, this.ClientSize.Height));

            this.Invoke(new MethodInvoker(() => { this.Controls.Add(mol); }));

            while (!_token.IsCancellationRequested)
            {
                mol.Invoke(new MethodInvoker(() => { mol.MoveParticle(); }));
                
                Thread.Sleep(60/_molNum);
            }
            this.Invoke(new MethodInvoker(() => { this.Controls.Remove(mol); }));

        }
    }
}
