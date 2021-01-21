// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using SharpGL.SceneGraph.Primitives;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        private uint[] m_textures;
        private string[] m_textureFiles = { "..//..//Textures//bricks.jpg", "..//..//Textures//wood.jpg", "..//..//Textures//floor.jpg" };
        private enum TextureObjects { Brick = 0, Wood, Floor}
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        private AssimpScene m_scene2;

        private int sirina_prozora;
        private int visina_prozora;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 200.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private bool isWorking = false;
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;
        private DispatcherTimer timer3;
        private DispatcherTimer timer4;

        private float m_Dart1_z = 0.0f;
        private float m_Dart2_z = 0.0f;
        private float m_Dart3_z = 0.0f;
        private float m_BoardFall_y = 0.0f;

        private float translateBoard = 0.0f;
        private float scaleBoard = 2.0f;

        private float redRefAmb = 1.0f;
        private float greenRefAmb = 0.0f;
        private float blueRefAmb = 0.0f;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        public AssimpScene Scene2
        {
            get { return m_scene2; }
            set { m_scene2 = value; }
        }


        public int Sirina
        {
            get { return sirina_prozora; }
            set { sirina_prozora = value; }
        }

        public int Visina
        {
            get { return visina_prozora; }
            set { visina_prozora = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set
            {
                m_xRotation = value;
                if (m_xRotation >= 90)
                {
                    m_xRotation = 90;
                }
                if (m_xRotation <= 0)
                {
                    m_xRotation = 0;
                }
            }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        public float TranslateX { get; set; }
        public float TranslateY { get; set; }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public bool IsWorking
        {
            get
            {
                return this.isWorking;
            }
            set
            {
                this.isWorking = value;
            }
        }

        public float TranslateBoard
        {
            get { return this.translateBoard; }
            set { this.translateBoard = value; }
        }

        public float ScaleBoard
        {
            get { return this.scaleBoard; }
            set { this.scaleBoard = value; }
        }

        public float RedRefAmb
        {
            get { return this.redRefAmb; }
            set { this.redRefAmb = value; }
        }

        public float BlueRefAmb
        {
            get { return this.blueRefAmb; }
            set { this.blueRefAmb = value; }
        }

        public float GreenRefAmb
        {
            get { return this.greenRefAmb; }
            set { this.greenRefAmb = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName1, String sceneFileName2, int width, int height, OpenGL gl)
        {
            m_textures = new uint[m_textureCount];
            this.m_scene = new AssimpScene(scenePath, sceneFileName1, gl);
            this.m_scene2 = new AssimpScene(scenePath, sceneFileName2, gl);
            this.m_width = width;
            this.m_height = height;
            TranslateX = 0;
            TranslateY = 0;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori


        #region Metode
        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);//Uključiti color tracking mehanizam 
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE); //Ambijentalna i difuzna komponenta materijala
            // InitLights(gl); //Tackasto, bele boje, gore-levo
            SetTackasto(gl);
            SetReflektor(gl);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 1f, 1f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);

            gl.Enable(OpenGL.GL_DEPTH_TEST); //Ukljuceno testiranje dubina
            //gl.Enable(OpenGL.GL_CULL_FACE_MODE); //Ukljuceno sakrivanje nevidljivih povrsina 
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);
            //stapanje teksture sa materijalom postaviti da bude GL_DECAL
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);

            //Za teksture podesiti wrapping da bude GL_REPEAT po obema osama
            //Podesiti filtere za teksture da budu linearno mipmap linearno filtriranje
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);  // Linear mipmap Filtering
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);  // Linear mipmap Filtering
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);

                image.UnlockBits(imageData);
                image.Dispose();
            }

            gl.Disable(OpenGL.GL_TEXTURE_2D);

            m_scene.LoadScene();  //Dartboard
            m_scene.Initialize();
            m_scene2.LoadScene(); //Dart
            m_scene2.Initialize();
        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.FrontFace(OpenGL.GL_CCW);  

            gl.PushMatrix();
            gl.LookAt(0, 0, m_sceneDistance + 150, 0, 0, 0, 0, 1, 0);
            gl.Translate(TranslateX, TranslateY, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);


            //gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);


            /*
            gl.PushMatrix();
            gl.PointSize(50.0f);
            gl.Translate(0, 0, 0);
            gl.Begin(OpenGL.GL_POINTS);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.End();
            gl.PopMatrix();
            */

            
            //tackasto
            gl.PushMatrix();
            Cube cube = new Cube();
            float[] poz = new float[] { -300.0f, 180.0f, 0.0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, poz);
            gl.Translate(poz[0], poz[1], poz[2]);
            gl.Scale(10.0f, 10.0f, 10.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            /*
            //reflektor
            gl.PushMatrix();
            Cube reflekt = new Cube();
            float[] light1pos = new float[] {0f, 180f, -215f, 1.0f };
            float[] light1diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light1ambient = new float[] { 0f, 1f, 0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Translate(0.0f, 120.0f, -225f);
            gl.Scale(10.0f, 10.0f, 10.0f);
            gl.Color(1.0f, 0.0f, 0.0f);
            reflekt.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            */

            
            gl.PushMatrix();
            Cube reflekt = new Cube();
            SetReflektor(gl);
            gl.Translate(0.0f, 180.0f, -225f);
            gl.Scale(10.0f, 10.0f, 10.0f);
            gl.Color(1.0f, 0.0f, 0.0f);
            reflekt.Render(gl, RenderMode.Render);
            gl.PopMatrix(); 

            #region Floor + Walls
            gl.PushMatrix(); //Pod
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Floor]);
            gl.LoadIdentity();
            gl.Scale(3, 7, 7);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);


            gl.Translate(0, -50, 0);
            gl.Scale(300, 180, 250);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0f, 1f, 0f);  //Pod
            gl.Color(0.30f, 0.15f, 0.0f);
            gl.TexCoord(1f, 1f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.TexCoord(0f, 1f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.TexCoord(0f, 0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.TexCoord(1f, 0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.End();

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.Scale(1, 1, 1);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            //Zidovi
            gl.PushMatrix();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);          
            gl.Translate(0, -50, 0);
            gl.Scale(300, 180, 250);

            gl.Begin(OpenGL.GL_QUADS);     
            gl.Color(0.556863f, 0.419608f, 0.137255f); //Nazad zid
            gl.Normal(0f, 0f, 1f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-1, -1, -1); //leva dole daleko
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(1, -1, -1);//desna dole daleko
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(1, 1, -1); //desna gore daleko
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-1, 1, -1); //leva gore daleko
            gl.End();

            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(0.556863f, 0.419608f, 0.137255f); //Desni zid
            gl.Normal(-1f, 0f, 0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);      
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);     
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);   
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.End();

            gl.Begin(OpenGL.GL_QUADS);
            gl.Color(0.556863f, 0.419608f, 0.137255f); //Levi zid
            gl.Normal(1f, 0f, 0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-1, 1, 1); //levo gore blizu
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-1, -1, 1); //levo dole blizu
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(-1, -1, -1); //levo dole lako
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-1, 1, -1); //levo gore daleko
            gl.End();

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            #endregion

            #region postolje

            gl.PushMatrix();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);
            Cube postolje = new Cube();
            gl.Normal(0f, 0f, 1f);
            gl.Translate(0.0f, 0.0f, -235.0f);
            gl.Scale(100.0f, 80.0f, 10f);
             gl.Color(0.4f, 0.4f, 0.4f);
            //gl.Color(1f, 1f, 1f);
            gl.Rotate(0f, 0f, -90f);
            postolje.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            #endregion

            #region pikado
            gl.PushMatrix();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            //gl.Translate(0.0f, 0.0f, -225.0f);
            gl.Translate(translateBoard, 0.0f + this.m_BoardFall_y, -225.0f);
            //gl.Scale(2.0f, 2.0f, 2.0f);
            gl.Scale(scaleBoard, scaleBoard, scaleBoard);
            
            m_scene.Draw();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();
            #endregion 

            #region Strelice
            gl.PushMatrix(); //1
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            //gl.Translate(0.0f, 0.0f, 0.0f);
            gl.Translate(0.0f, 0.0f, 0.0f + this.m_Dart3_z);
            gl.Scale(10.0f, 10f, 5.0f);
            
            m_scene2.Draw();
            gl.PopMatrix();

            gl.PushMatrix(); //2
            //gl.Translate(-50.0f, 0.0f, 0.0f);
            gl.Translate(-50.0f, 0.0f, 0.0f + this.m_Dart1_z);
            gl.Scale(10.0f, 10f, 5.0f);

            m_scene2.Draw();
            gl.PopMatrix();

            gl.PushMatrix(); //3
                             //  gl.Translate(50.0f, 0.0f, 0.0f);
            gl.Translate(50.0f, 0.0f , 0.0f + this.m_Dart2_z);
            gl.Scale(10.0f, 10f, 5.0f);

            m_scene2.Draw();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            #endregion


            #region tekst
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PushMatrix();
            gl.Viewport(m_width / 2, 0, m_width / 2, m_height /2 );
            gl.MatrixMode(OpenGL.GL_PROJECTION);          
            gl.LoadIdentity();
            gl.Ortho2D(-10, 10, -10, 10);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            
            gl.LoadIdentity();
            gl.Translate(-3.0f, -4.0f, 0.0f);
            gl.Color(10.0f, 10.0f, 0.0f);

            gl.PushMatrix();
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "Predmet: Racunarska grafika");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "_________________________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0.0f, -1.0f, 0.0f);
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "Sk.god: 2020/21");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0.0f, -1.0f, 0.0f);
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "_____________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0.0f, -2.0f, 0.0f);
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "Ime: Stefan");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0.0f, -2.0f, 0.0f);
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "__________");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0.0f, -3.0f, 0.0f);
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "Prezime: Aradjanin");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0.0f, -3.0f, 0.0f);
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "________________");
            gl.PopMatrix();
            gl.PushMatrix(); gl.Translate(0.0f, -4.0f, 0.0f);
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "Sifra: 9.1");
            gl.PopMatrix();
            gl.PushMatrix();
            gl.Translate(0.0f, -4.0f, 0.0f);
            gl.DrawText3D("Arial Bold", 14.0f, 0f, 0f, "_______");

            gl.PopMatrix();

            Resize(gl, m_width, m_height);
            gl.PopMatrix();

            #endregion

            // Oznaci kraj iscrtavanja

            gl.PopMatrix();
            gl.Flush();
        }

        public void InitLights(OpenGL gl)
        {

            float[] global_ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            //Definisati tačkasti svetlosni izvor bele boje
            //i pozicionirati ga gore-levo u odnosu na centar scene 
            float[] light0pos = new float[] { -300.0f, 180.0f, 0f, 1.0f };
            float[] light0diffuse = new float[] { 1f, 1f, 1f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };


            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);//Tackasto
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);

            
            float[] light1pos = new float[] { 0f, 180f, -215f, 1.0f };
            float[] light1ambient = new float[] { 0f, 0f, 0f, 1.0f };
            float[] light1diffuse = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
            float[] smer = new float[] { 0.0f, -1.0f, 0.0f };//dole


            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);     
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
          


            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
           // gl.Enable(OpenGL.GL_LIGHT1);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);
        }

        public void SetTackasto(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.6f, 0.6f, 0.6f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            float[] light0pos = new float[] { -300.0f, 180.0f, 0f, 1.0f };
            float[] light0diffuse = new float[] { 0.99f, 0.99f, 0.99f, 1.0f };
            float[] light0ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };


            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);//Tackasto
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);

        }

        public void SetReflektor(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.6f, 0.6f, 0.6f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            float[] light1pos = new float[] { 0f, 180f, -215f, 1.0f };
            float[] light1ambient = new float[] { redRefAmb, greenRefAmb, blueRefAmb, 1.0f };
            float[] light1diffuse = new float[] { 1f, 0f, 0f, 1.0f };
            float[] smer = new float[] { 0.0f, -1.0f, 0.0f };//dole


            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);
        }


        public void Animation1()
        {
            this.isWorking = true;
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(100);
            timer1.Tick += new EventHandler(ThrowingDart1);
            timer1.Start();
        }

        public void ThrowingDart1(object sender, EventArgs e)
        {
            if (m_Dart2_z > -215)         
            {
                m_Dart2_z -= 40f;             
            }
            else
            {
                Animation2();
                timer1.Stop();

            }
        }

        public void Animation2()
        {
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(100);
            timer2.Tick += new EventHandler(ThrowingDart2);
            timer2.Start();
        }

        public void ThrowingDart2(object sender, EventArgs e)
        {
            if (m_Dart1_z > -215)  
            {
                m_Dart1_z -= 40f;
            }
            else
            {
                Animation3();
                timer2.Stop();

            }
        }



        public void Animation3()
        {
            timer3 = new DispatcherTimer();
            timer3.Interval = TimeSpan.FromMilliseconds(100);
            timer3.Tick += new EventHandler(ThrowingDart3);
            timer3.Start();
        }

        public void ThrowingDart3(object sender, EventArgs e)
        {
            if (m_Dart3_z > -215) {     
                m_Dart3_z -= 40f;
               
            }
            else
            {
                Animation4();
                timer3.Stop();

            }
        }

        public void Animation4()
        {
            timer4 = new DispatcherTimer();
            timer4.Interval = TimeSpan.FromMilliseconds(200);
            timer4.Tick += new EventHandler(FallingDart);
            timer4.Start();
        }

        public void FallingDart(object sender, EventArgs e)
        {
            if(m_BoardFall_y >= -200)
            {
                m_BoardFall_y -= 25;
            }
            else
            {
                IsWorking = false;
                Reset();
                timer4.Stop();
                IsWorking = false;
            }


        }


        public void Reset()
        {
            IsWorking = false;
            m_Dart1_z = 0.0f;          
            m_Dart2_z = 0.0f;         
            m_Dart3_z = 0.0f;
            m_BoardFall_y = 0.0f;
        }


            /// <summary>
            /// Podesava viewport i projekciju za OpenGL kontrolu.
            /// </summary>
            public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
           
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 1f, 20000f); //fav = 45, near = 1, far po potrebi
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
