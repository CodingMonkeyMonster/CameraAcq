using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sick.Icon;

namespace MultRangerWIconExpress
{
    public partial class MainForm : Form
    {
        List<CameraHelper> cameras;

        public MainForm()
        {
            InitializeComponent();
        }

        private void initialize_button_Click(object sender, EventArgs e)
        {
            // Register error/log message handler.
            Sick.Icon.IconApi.Utility.OpenLogFile("../../../example.log");
            Sick.Icon.IconApi.Utility.SetErrorEventHandler(MyErrorHandler);

            // Define name .icx files and IP addresses for all camera systems.
            // The configuration file can be replaced by configurations of your own. Use Ranger Studio to create and configure such files.
            // Optionally you can manually set the IP to the camera. Note that the configuration file contains the IP and this 
            // manual setting is not needed if the IP in the configuration file already is matching the camera.
            cameras = new List<CameraHelper>();
            cameras.Add(new CameraHelper("Camera 0", 
                "C:\\Users\\Omicron\\Documents\\402-13-12-16-2014-onsite\\RangerSetupFiles\\Parameter File Side Inspection (three fast multiscan 3D inspection) v14.icx", 
                "10.10.15.105"));
            cameras.Add(new CameraHelper("Camera 1",
                "C:\\Users\\Omicron\\Documents\\402-13-12-16-2014-onsite\\RangerSetupFiles\\Parameter File Side Inspection (three fast multiscan 3D inspection) v14.icx",
                "10.10.11.101"));
            
            // Do the initialization and connection of each camera system in a separate thread so that
            // the cameras don't have to wait for each other.
            foreach (CameraHelper camera in cameras)
            {
                camera.ConnectDelegate = new CameraHelper.ConnectDelegateType(camera.ConnectAsync);
                camera.AsyncResult = camera.ConnectDelegate.BeginInvoke(null, null);                               
            }

            // Wait until all initialization threads has finished.
            foreach (CameraHelper camera in cameras)
            {
                Sick.Icon.Result result = camera.ConnectDelegate.EndInvoke(camera.AsyncResult);
                if (result != Result.E_ALL_OK)
                {
                    // Rethrow error code (from async call) as exception in main thread.
                    throw new Sick.Icon.IconException(result);
                }
            }
        }

        private void capture_button_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (CameraHelper camera in cameras)                
                {
                    camera.CameraSystem.Start();                                  
                }                    
            }
            catch (Sick.Icon.IconException ex)
            {
                Sick.Icon.IconApi.Utility.CloseLogFile();
                Sick.Icon.IconApi.CloseApi();
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            // PUT IN A STOP FUNCTION SOMEWHERE SO YOU DON'T KEEP TRYING TO ACQUIRE IMAGE
        }

        private void disconnect_button_Click(object sender, EventArgs e)
        {
            // Disconnect cameras and delete unused objects.
            foreach (CameraHelper camera in cameras)
            {
                // Disconnect camera
                camera.CameraSystem.Disconnect();
            }

            Sick.Icon.IconApi.Utility.CloseLogFile();
            Sick.Icon.IconApi.CloseApi();
        }

        // Error handler function (callback).
        static void MyErrorHandler(int errLevel, string errString)
        {
            System.Windows.Forms.MessageBox.Show("{0}: {1} " + errLevel.ToString() + errString);            
        }

        private static bool m_printStateChange = true;
        /// <summary>
        /// This class handles what to do when the instance of ICameraSystem in the CameraHelper has 
        /// a state change or data becomes available.
        /// </summary>
        class MyEventHandlers
        {
           // public ICameraSystem CameraSystem;
            //int imgCount = 0;

            public MyEventHandlers(string camName)
            {
                m_camName = camName;
                m_bufferCount = 0;
               // CameraSystem = camSystem;
            }

            public void StateChangedHandler(IState state)
            {
                bool _state = state.ToString() == "NotConnected";
                _state = state.ToString() == "Connected";

                /*
                if (state.ToString() == "Acquiring")
                    imgCount++;
                else if (state.ToString() == "Started" && imgCount > 0)
                {
                    imgCount = 0;
                    CameraSystem.Stop();
                }
                */

                if (m_printStateChange)
                {                    
                    //System.Windows.Forms.MessageBox.Show("State of {0} changed to {1}.  " + m_camName + state.ToString());
                }
            }
            /// <summary>
            /// This method is intended to suscribe to a ICameraSystem.DataAvailable event.  
            /// It takes the data from the camera which is stored in the IIconBuffer.
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="status"></param>
            public void DataAvailableHandler(IIconBuffer buffer, IGrabStatus status)
            {
                // Check and print status of the received buffer.
                String outString = String.Format("\n{0} received buffer {1}.\n", m_camName, ++m_bufferCount);
                if (status.AllOK() != true)
                {
                    if (status.GetOverflowStatus()) outString += "Buffer overflow occurred.\n";
                    if (status.GetOvertrigs() != 0) outString += String.Format("Camera reported {0} overtrigs in the buffer.\n", status.GetOvertrigs());
                    if (status.GetScansLost() != 0) outString += String.Format("Lost {0} scans in the buffer.\n", status.GetScansLost());
                }                

                // See DataAccess.cpp for example code for reading out data from the buffer.
                String resultString;
                DataProcessor.accessData(buffer, out resultString);

                System.Windows.Forms.MessageBox.Show(outString + resultString);
               // CameraSystem.Stop(); // stops camera after image data is obtained from first capture
            }
                       
            private string m_camName;
            private int m_bufferCount;            
        };

        // Helper class which can connect to a CameraSystem asynchronously.
        class CameraHelper
        {
            private ICameraSystem m_cameraSystem;
            public ICameraSystem CameraSystem { get { return m_cameraSystem; } set { m_cameraSystem = value; } }
            private bool m_cameraStarted;
            public bool CameraStarted { get { return m_cameraStarted; } set { m_cameraStarted = value; } }
            private MyEventHandlers m_eventHandlers;
            // The following two fields are used for BeginInvoke/EndInvoke calls.
            public IAsyncResult AsyncResult;
            public ConnectDelegateType ConnectDelegate;

            /// <summary>
            /// Helper class which can connect to a CameraSystem asynchronously.
            /// </summary>
            /// <param name="camName">
            /// A name created to uniquley identify the camera.
            /// </param>
            /// <param name="icxFile">
            /// This file is created within RangerStudio
            /// </param>
            /// <param name="ipAddress">
            /// ip address of the camera.
            /// </param>
            public CameraHelper(string camName, string icxFile, string ipAddress)
            {
                CameraSystem = Sick.Icon.IconApi.Factory.CreateInstance<Sick.Icon.ICameraSystem>();
                m_eventHandlers = new MyEventHandlers(camName);

                // Initialize camera system.
                CameraSystem.Init(CameraSystemType.ETHERNET_CAMERA, camName);
                // Attach your event handlers to the StateChanged and DataAvailable events.
                CameraSystem.StateChanged += m_eventHandlers.StateChangedHandler;
                CameraSystem.DataAvailable += m_eventHandlers.DataAvailableHandler; // Image acqusition is asynchronous, so an event is needed to indicate when data is available

                // Load the system configuration.
                CameraSystem.LoadConfiguration(icxFile);

                // Set the IP-address to the camera if other than the one stated in the configuration file.
                if (ipAddress != "")                
                    CameraSystem.SetParameter("", "camera IP address", ipAddress);                

                // Check that system is set to callback mode.
                string callbackEnabled;
                CameraSystem.GetParameter("", "buffer callbacks enabled", out callbackEnabled);
                if (callbackEnabled == "false")
                {
                    System.Windows.Forms.MessageBox.Show("This example program runs in callback mode, but 'buffer callbacks enabled' is set to 'false' in the .icx-file . \n Adjusting 'buffer callbacks enabled' parameter to 'true'.");                    
                    CameraSystem.SetParameter("", "buffer callbacks enabled", "true");
                }
            }

            /// <summary>
            /// Automatically toggles between ICameraSystem.Start() and ICameraSystem.Stop()
            /// </summary>
            public void ToggleStartStop()
            {
                if (CameraStarted)
                {
                    CameraSystem.Stop();
                    CameraStarted = false;
                }
                else
                {
                    CameraSystem.Start();
                    CameraStarted = true;
                }
            }

            /// <summary>
            /// Delegate defined to enabled asyncronous grabbing.  Return type is for returning errorcode 
            /// </summary>
            /// <returns></returns>
            public delegate Sick.Icon.Result ConnectDelegateType();

            /// <summary>
            /// A function to be used by Begin/EndInvoke needs to return an error code rather than throwing exception.
            /// </summary>
            /// <returns></returns>
            public Sick.Icon.Result ConnectAsync()
            {                
                try
                {
                    CameraSystem.Connect();                    
                    return Sick.Icon.Result.E_ALL_OK;
                }
                catch (Sick.Icon.IconException e)
                {
                    Sick.Icon.IconApi.Utility.CloseLogFile();
                    Sick.Icon.IconApi.CloseApi();
                    return e.ResultCode;
                }
            }
        }


    }
}
