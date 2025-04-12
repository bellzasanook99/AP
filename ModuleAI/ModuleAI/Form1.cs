using ModuleAI.Commons;
using ModuleAI.Services;
using System.IO;

namespace ModuleAI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            HttpListenerService.Port = 88;
            HttpListenerService.StartWebServer();

           Module01.initial();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Module01.train();
            Bitmap bmp = new Bitmap(@"D:\\NEW2024\\devai\\PortAI\\bin\\Debug\\Database\\15122024\\datatrin\\circle\\63869800936273.jpg");
          string type =   Module01.Matching(bmp);
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        }
    }
}
