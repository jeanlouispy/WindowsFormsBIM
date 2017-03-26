using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;
using Xbim.Presentation;

namespace WindowsFormsBIM
{
    public partial class Form1 : Form
    {
        DrawingControl3D control3D;
        private ElementHost ctrlHost;
       
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {             
                OpenIfc(openFileDialog1.FileName);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ctrlHost = new ElementHost();
            ctrlHost.Dock = DockStyle.Fill;
            panel1.Controls.Add(ctrlHost);
            control3D = new DrawingControl3D();
            
            ctrlHost.Child = control3D;
            control3D.MouseDoubleClick += Control3D_MouseDoubleClick;
        }

        private void Control3D_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Console.WriteLine(((Xbim.Ifc2x3.Kernel.IfcRoot)(((DrawingControl3D)sender).SelectedEntity)).FriendlyName);


        }

        private void OpenIfc(string inIfcFilePath)
        {
            var tmpFile = Path.GetTempFileName();
            var model = IfcStore.Open(inIfcFilePath);
            if(model.GeometryStore.IsEmpty)
            {
                var context = new Xbim3DModelContext(model);
                context.CreateContext();
            }
            foreach(var modelReference in model.ReferencedModels)
            {
                if (modelReference == null)
                    continue;
                if (!modelReference.Model.GeometryStore.IsEmpty)
                    continue;
                var context = new Xbim3DModelContext(modelReference.Model);
                context.CreateContext();
            }

            control3D.Model = model;
        }
    }
}
