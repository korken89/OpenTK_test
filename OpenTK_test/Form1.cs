using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO.Ports; 
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using KFlySerialStateMachine;

namespace OpenTK_test
{
    public partial class OpenTK_test : Form
    {
        Stopwatch sw = new Stopwatch();
        static float CAMERA_FOVY = 30.0f;
        static float CAMERA_ZFAR = 1000.0f;
        static float CAMERA_ZNEAR = 0.1f;

        //double q0 = 0.924437881, q1 = 0.0802535266, q2 = -0.308261216, q3 = 0;

        
        //Matrix4 testRot = new Matrix4(

        float g_heading;
        float g_pitch;

        public static bool Available_VBO_IBO = false;
        public static bool Available_GLSL = false;

        float[] g_cameraPos = new float[3];
        float[] g_targetPos = new float[3];

        private SerialPort _serialPort = new SerialPort();
        private int _baudRate = 115200;
        private int _dataBits = 8;
        private Handshake _handshake = Handshake.None;
        private Parity _parity = Parity.None;
        private string _portName = "COM8";
        private StopBits _stopBits = StopBits.One;

        StateMachine statem = new StateMachine();

        public OpenTK_test()
        {
            InitializeComponent();
        }

        public void Open()
        {

                _serialPort.BaudRate = _baudRate;
                _serialPort.DataBits = _dataBits;
                _serialPort.Handshake = _handshake;
                _serialPort.Parity = _parity;
                _serialPort.PortName = _portName;
                _serialPort.StopBits = _stopBits;
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                try
                {
                    _serialPort.Open();
                }
                catch { }
        }

        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Initialize a buffer to hold the received data 
            byte[] buffer = new byte[_serialPort.ReadBufferSize];

            //There is no accurate method for checking how many bytes are read 
            //unless you check the return from the Read method 
            int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);

            foreach (byte bte in buffer)
                statem.SerialManager(bte);

            glControl1.Invalidate();
        }

        internal void gluPerspective(double fovy, double aspect, double zNear, double zFar)
        {
            double xmin, xmax, ymin, ymax;

            ymax = zNear * Math.Tan(fovy * Math.PI / 360.0);
            ymin = -ymax;

            xmin = ymin * aspect;
            xmax = ymax * aspect;

            GL.Frustum(xmin, xmax, ymin, ymax, zNear, zFar);
        }

        Vector3 forward = new Vector3();
        Vector3 up = new Vector3();
        Vector3 right = new Vector3();

        void gluLookAt(float eyex, float eyey, float eyez,
               float centerx, float centery, float centerz,
               float upx, float upy, float upz)
        {
            float[] m = new float[16];

            forward.X = centerx - eyex;
            forward.Y = centery - eyey;
            forward.Z = centerz - eyez;

            up.X = upx;
            up.Y = upy;
            up.Z = upz;

            forward.Normalize();

            /* Side = tForward x tUp */
            Vector3.Cross(ref forward, ref up, out right);
            right.Normalize();

            /* Recompute tUp as: tUp = tRight x tForward */
            Vector3.Cross(ref right, ref forward, out up);

            // set right vector
            m[0] = right.X; m[1] = up.X; m[2] = -forward.X; m[3] = 0;
            // set up vector
            m[4] = right.Y; m[5] = up.Y; m[6] = -forward.Y; m[7] = 0;
            // set forward vector
            m[8] = right.Z; m[9] = up.Z; m[10] = -forward.Z; m[11] = 0;
            // set translation vector
            m[12] = 0; m[13] = 0; m[14] = 0; m[15] = 1;

            GL.MultMatrix(m);
            GL.Translate(-eyex, -eyey, -eyez);
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.Black);
            ResetCamera();

            GL.Enable(EnableCap.DepthTest); // Está desactivado por defecto en OpenGL
            GL.Enable(EnableCap.CullFace); // Está desactivado por defecto en OpenGL
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            sw.Start();
            Application.Idle +=new EventHandler(Application_Idle);
            Open();
        }

        float rotation = 0;
        float old_y = 0, y = 0;

        void Application_Idle(object sender, EventArgs e)
        {
                
                //glControl1.Invalidate();

        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            double aspectRatio = glControl1.Width / (double)glControl1.Height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            gluPerspective(CAMERA_FOVY, aspectRatio, CAMERA_ZNEAR, CAMERA_ZFAR);

            glControl1.Invalidate();
        }

        private void DrawCube()
        {
            double q0 = statem.q0;
            double q1 = statem.q1;
            double q2 = statem.q2;
            double q3 = statem.q3;

            double gs = Math.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
            q0 /= gs;
            q1 /= gs;
            q2 /= gs;

            label2.Text = "q0: " + (Math.Round(q0 * 1000) / 1000).ToString();
            label3.Text = "q1: " + (Math.Round(q1 * 1000) / 1000).ToString();
            label4.Text = "q2: " + (Math.Round(q2 * 1000) / 1000).ToString();
            label5.Text = "q3: " + (Math.Round(q3 * 1000) / 1000).ToString();

            label6.Text = "ax: " + statem.ax.ToString();
            label7.Text = "ay: " + statem.ay.ToString();
            label8.Text = "az: " + statem.az.ToString();

            label9.Text = "wx: " + (Math.Round((float)statem.wx * 0.0174532925f / 16.4f * 1000) / 1000).ToString();
            label10.Text = "wy: " + (Math.Round((float)statem.wy * 0.0174532925f / 16.4f * 1000) / 1000).ToString();
            label11.Text = "wz: " + (Math.Round((float)statem.wz * 0.0174532925f / 16.4f * 1000) / 1000).ToString();

            label12.Text = "mx: " + statem.mx.ToString();
            label13.Text = "my: " + statem.my.ToString();
            label14.Text = "mz: " + statem.mz.ToString();

            Quaternion testQuat = new Quaternion((float)q1, (float)q2, (float)q3, (float)q0);

            float xrot, yrot, zrot;
            float angle = (float)(2 * Math.Acos(testQuat.W));
            double s = Math.Sqrt(1 - testQuat.W * testQuat.W);

            // test to avoid divide by zero, s is always positive due to sqrt
            // if s close to zero then direction of axis not important
            if (s < 0.001)
            {
                // if it is important that axis is normalised then replace with x=1; y=z=0;
                xrot = testQuat.X;
                yrot = testQuat.Y;
                zrot = testQuat.Z;
                // z = q.getZ();
            }
            else
            {
                xrot = (float)(testQuat.X / s); // normalise axis
                yrot = (float)(testQuat.Y / s);
                zrot = (float)(testQuat.Z / s);
            }

            GL.Rotate(angle * 180.0f / (float)Math.PI, new Vector3(xrot, zrot, yrot));
            //GL.Rotate(rotation, Vector3.UnitZ);

            GL.Begin(BeginMode.Quads);

            GL.Color3(Color.Gray);
            GL.Vertex3(-1.0f, -0.05f, -1.0f);
            GL.Vertex3(-1.0f, 0.05f, -1.0f);
            GL.Vertex3(1.0f, 0.05f, -1.0f);
            GL.Vertex3(1.0f, -0.05f, -1.0f);

            GL.Color3(Color.DarkGreen);
            GL.Vertex3(-1.0f, -0.05f, -1.0f);
            GL.Vertex3(1.0f, -0.05f, -1.0f);
            GL.Vertex3(1.0f, -0.05f, 1.0f);
            GL.Vertex3(-1.0f, -0.05f, 1.0f);

            GL.Color3(Color.Gray);
            GL.Vertex3(-1.0f, -0.05f, -1.0f);
            GL.Vertex3(-1.0f, -0.05f, 1.0f);
            GL.Vertex3(-1.0f, 0.05f, 1.0f);
            GL.Vertex3(-1.0f, 0.05f, -1.0f);

            GL.Color3(Color.Gray);
            GL.Vertex3(-1.0f, -0.05f, 1.0f);
            GL.Vertex3(1.0f, -0.05f, 1.0f);
            GL.Vertex3(1.0f, 0.05f, 1.0f);
            GL.Vertex3(-1.0f, 0.05f, 1.0f);

            GL.Color3(Color.Green);
            GL.Vertex3(-1.0f, 0.05f, -1.0f);
            GL.Vertex3(-1.0f, 0.05f, 1.0f);
            GL.Vertex3(1.0f, 0.05f, 1.0f);
            GL.Vertex3(1.0f, 0.05f, -1.0f);

            GL.Color3(Color.LightGray);
            GL.Vertex3(1.0f, -0.05f, -1.0f);
            GL.Vertex3(1.0f, 0.05f, -1.0f);
            GL.Vertex3(1.0f, 0.05f, 1.0f);
            GL.Vertex3(1.0f, -0.05f, 1.0f);

            GL.End();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            sw.Stop(); // we've measured everything since last Idle run
            double milliseconds = sw.Elapsed.TotalMilliseconds;
            sw.Reset(); // reset stopwatch
            sw.Start(); // restart stopwatch
            // total time since last Idle run
            float deltaRotation = (float)milliseconds / 20.0f;
            rotation += deltaRotation;

            y = 0.05f / ((float)milliseconds) * 1000.0f + 0.95f * old_y;
            old_y = y;

            label1.Text = "FPS: " + (Math.Round(y)).ToString();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.Black);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            gluLookAt(g_cameraPos[0], g_cameraPos[1], g_cameraPos[2],
                      g_targetPos[0], g_targetPos[1], g_targetPos[2],
                      0.0f, 1.0f, 0.0f);

            GL.Rotate(g_pitch, 1.0f, 0.0f, 0.0f);
            GL.Rotate(g_heading, 0.0f, 1.0f, 0.0f);

            GL.Disable(EnableCap.Lighting);
            DrawCube();

            glControl1.SwapBuffers();
            //System.Threading.Thread.Sleep(15); // Manual VSync-ish
        }

        void ResetCamera()
        {
            g_targetPos[0] = 0; g_targetPos[1] = 0; g_targetPos[2] = 0;

            g_cameraPos[0] = g_targetPos[0];
            g_cameraPos[1] = g_targetPos[1];
            g_cameraPos[2] = g_targetPos[2] + 10 + CAMERA_ZNEAR + 0.4f;

            g_pitch = 20.0f;
            g_heading = 0.0f;
        }
    }

    enum ECameraMode
    {
        CAMERA_NONE, CAMERA_TRACK, CAMERA_DOLLY, CAMERA_ORBIT
    }
}
