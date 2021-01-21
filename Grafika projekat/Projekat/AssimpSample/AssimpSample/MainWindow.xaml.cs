using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Darts"), "dartboard.obj", "dart.obj", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Sirina = (int)prozor.ActualWidth;
            m_world.Visina = (int)prozor.ActualHeight;
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_world.IsWorking)
            {
                return;
            }
            else
            {
               

                this.sbBtn.IsEnabled = true;
                this.scaleBoard.IsEnabled = true;
                this.tbBtn.IsEnabled = true;
                this.translateBoard.IsEnabled = true;
                this.red.IsEnabled = true;
                this.blue.IsEnabled = true;
                this.green.IsEnabled = true;
                this.clBtn.IsEnabled = true;

                

                switch (e.Key)
                {
                    case Key.F5: this.Close(); break;
                    case Key.T: m_world.RotationX -= 5.0f; break;
                    case Key.G: m_world.RotationX += 5.0f; break;
                    case Key.F: m_world.RotationY -= 5.0f; break;
                    case Key.H: m_world.RotationY += 5.0f; break;
                    case Key.Add: m_world.SceneDistance -= 100.0f; break;
                    case Key.Subtract: m_world.SceneDistance += 100.0f; break;
                    case Key.Left: m_world.TranslateX += 30.0f; break;
                    case Key.Right: m_world.TranslateX -= 30.0f; break;
                    case Key.Up: m_world.TranslateY -= 30.0f; break;
                    case Key.Down: m_world.TranslateY += 30.0f; break;
                    case Key.C:
                        m_world.Animation1();
                        this.sbBtn.IsEnabled = false;
                        this.scaleBoard.IsEnabled = false;
                        this.tbBtn.IsEnabled = false;
                        this.translateBoard.IsEnabled = false;
                        this.red.IsEnabled = false;
                        this.blue.IsEnabled = false;
                        this.green.IsEnabled = false;
                        this.clBtn.IsEnabled = false;
                        break;
                    case Key.V:
                        m_world.Reset();
                        break;

                        /*
                        case Key.F2:
                            OpenFileDialog opfModel = new OpenFileDialog();
                            bool result = (bool) opfModel.ShowDialog();
                            if (result)
                            {

                                try
                                {
                                    World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                                    m_world.Dispose();
                                    m_world = newWorld;
                                    m_world.Initialize(openGLControl.OpenGL);
                                }
                                catch (Exception exp)
                                {
                                    MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK );
                                }
                            }
                            break;
                        */
                }

            }

           
        }

        private void TbBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_world.TranslateBoard = float.Parse(this.translateBoard.Text);
            }
            catch
            {
                MessageBox.Show("Neuspešno transliranje pikado table!");
            }
        }

        private void SbBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                m_world.ScaleBoard = float.Parse(this.scaleBoard.Text);
            }
            catch
            {
                MessageBox.Show("Neuspešno skaliranje pikado table!");
            }
        }

        private void ClBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                float red = float.Parse(this.red.Text);
                float blue = float.Parse(this.blue.Text);
                float green = float.Parse(this.green.Text);

                if (red >= 0 && red <= 1)
                {
                    m_world.RedRefAmb = red;
                }
                else
                {
                    MessageBox.Show("Za boju koristite vrednosti između 0 i 1.");
                }

                if (blue >= 0 && blue <= 1)
                {
                    m_world.BlueRefAmb = blue;
                }
                else
                {
                    MessageBox.Show("Za boju koristite vrednosti između 0 i 1.");
                }

                if (green >= 0 && green <= 1)
                {
                    m_world.GreenRefAmb = green;
                }
                else
                {
                    MessageBox.Show("Za boju koristite vrednosti između 0 i 1.");
                }
            }
            catch
            {
                MessageBox.Show("Greska kod odabira boja");
            }
            


        }

        

    }
}
