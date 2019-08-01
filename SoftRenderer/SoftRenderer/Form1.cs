﻿// jave.lin 2019.07.15
// 这个做模拟光线可以测试用，但会非常卡，因为可编程管线我们使用的是反射的机制来处理的
#define PROGRAMMABLE_PIPELINE

using RendererCommon.SoftRenderer.Common.Shader;
using SoftRenderer.Common.Mathes;
using SoftRenderer.Games;
using SoftRenderer.SoftRenderer;
using SoftRenderer.SoftRenderer.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SoftRenderer
{
    public class GlobalMessageHandler : IMessageFilter
    {
        public delegate void OnWheel(bool fromDownToUp);
        /* =======message start======= */
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        //https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.forms.message.msg?view=netframework-4.8
        public const int WM_ACTIVATEAPP = 0x001C;

        // https://docs.microsoft.com/zh-cn/previous-versions/windows/desktop/inputmsg/wm-parentnotify
        // CREATE WIN, DESTROY WIN, MS_LBTN, MS_RBTN, MS_MBTN, ETC.
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MOUSEWHEEL = 0x020A;
        // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-ncmousemove
        //https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-mousemove
        // MS_MOVE
        //private const int WM_NCMOUSEMOVE = 0x00A0;
        public const int WM_MOUSEMOVE = 0x0200;

        public bool appActive { get; set; }
        public Point last_ms_pos { get; private set; } = new Point(-1, -1);
        public Point delta_ms_pos { get; set; }
        public bool MS_LBTN { get; private set; }
        public bool MS_RBTN { get; private set; }
        public bool MS_MBTN { get; private set; }

        private Dictionary<Keys, bool> keysDownStatus = new Dictionary<Keys, bool>();

        public event OnWheel OnWheelEvent;

        public void SetKeyDown(Keys k, bool isDown)
        {
            if ((k & Keys.Control) != 0)
                k = k ^ Keys.Control;
            keysDownStatus[k] = isDown;
        }
        public bool GetKeyDown(Keys k)
        {
            keysDownStatus.TryGetValue(k, out bool down);
            return down;
        }

        /* =======message end======= */

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_LBUTTONDOWN:
                    MS_LBTN = true;
                    break;
                case WM_LBUTTONUP:
                    MS_LBTN = false;
                    break;
                case WM_MBUTTONDOWN:
                    MS_MBTN = true;
                    break;
                case WM_MBUTTONUP:
                    MS_MBTN = false;
                    break;
                case WM_RBUTTONDOWN:
                    MS_RBTN = true;
                    break;
                case WM_RBUTTONUP:
                    MS_RBTN = false;
                    break;
                case WM_MOUSEWHEEL:
                    OnWheelEvent.Invoke((int)m.WParam > 0);
                    break;
                case WM_MOUSEMOVE:
                    var nowPos = Cursor.Position;
                    if (last_ms_pos.X == -1 || last_ms_pos.Y == -1)
                        last_ms_pos = nowPos;
                    delta_ms_pos = new Point(nowPos.X - last_ms_pos.X, nowPos.Y - last_ms_pos.Y);
                    last_ms_pos = nowPos;
                    //Console.WriteLine($"mx,y:{nowPos}, deltax,y:{delta_ms_pos}");
                    break;
                case WM_ACTIVATEAPP:
                    appActive = m.WParam != IntPtr.Zero;
                    //Console.WriteLine($"win actived:{appActive}");
                    break;
                case WM_KEYUP:
                    SetKeyDown((Keys)m.WParam, false);
                    //Console.WriteLine($"Key{(Keys)m.WParam} up:true");
                    break;
            }
            // Always allow message to continue to the next filter control
            return false;
        }

        #endregion
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public partial class MainForm : Form
    {
        //API声明：获取当前焦点控件句柄      
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();
        ///获取 当前拥有焦点的控件
        private Control GetFocusedControl()
        {
            Control focusedControl = null;
            // To get hold of the focused control:
            IntPtr focusedHandle = GetFocus();
            if (focusedHandle != IntPtr.Zero)
                //focusedControl = Control.FromHandle(focusedHandle);
                focusedControl = Control.FromChildHandle(focusedHandle);
            return focusedControl;
        }

        private static readonly int outlineOffsetHash = "outlineOffset".GetHashCode();
        private const int buff_size = 200;
        private Renderer renderer;

        private long lastTick = -1; // 1 tick = 1/10000 ms
        private float elapsedTime;
        private int fpsCounter;
        private enum FpsShowType
        {
            Immediately,
            OneSecond,
        }
        private FpsShowType fpsShowType = FpsShowType.OneSecond;
        private Rectangle bmdRect = new Rectangle(0, 0, buff_size, buff_size);
        private Random ran = new Random();
        private bool paused = false;
        private List<GameObject> gameObjs = new List<GameObject>();
        public List<GameObject> GameObjs { get => gameObjs; }
        private Camera camera;
        private List<Vector3> wposList = new List<Vector3>();
        private List<Triangle> triangles = new List<Triangle>();
        private ShaderData shaderData;

        private bool autoRotate = true;

        public float Tx { get; set; }
        public float Ty { get; set; }
#if PROGRAMMABLE_PIPELINE
        // 可编程管线太卡，所以我拉远一些镜头，让片段少一些
        public float Tz { get; set; } = 8;
#else
        public float Tz { get; set; } = 7;
#endif
        public float Rx { get; set; }
        public float Ry { get; set; }
        public float Rz { get; set; }

        // 法线描边的偏移值，这个需要好的模型才能测试出来
        // TODO 后面添加支持：加载*.obj模型
        public float normalOutlineOffset { get; set; } = -1f; // 法线现在有问题，所以取负数，以后有精力再看，因为现在看了不少于50篇相关内容，没一个可以解决问题的

        private GlobalMessageHandler globalMsg = new GlobalMessageHandler();

        public MainForm()
        {
            InitializeComponent();

            PictureBox.Click += PictureBox_Click;
            globalMsg.OnWheelEvent += GlobalMsg_OnWheelEvent;
            Application.AddMessageFilter(globalMsg);
            this.FormClosed += (s, e) => Application.RemoveMessageFilter(globalMsg);

            renderer = new Renderer(buff_size, buff_size);

            renderer.State.ClearColor = Color.Gray;
            renderer.State.ShadingMode = ShadingMode.Shaded;
            renderer.State.BlendSrcColorFactor = BlendFactor.SrcAlpha;
            renderer.State.BlendDstColorFactor = BlendFactor.OneMinusSrcAlpha;
            renderer.State.BlendSrcAlphaFactor = BlendFactor.One;
            renderer.State.BlendDstAlphaFactor = BlendFactor.One;
            // test
            renderer.State.Cull = FaceCull.Off;

            this.PictureBox.Width = buff_size;
            this.PictureBox.Height = buff_size;

            PropertyGrid.SelectedObject = renderer;

            gameObjs = new List<GameObject>();

            var go = new GameObject("Cube");
            /**
               

            bug
               
            (1,1)          (-1,1)
             ----------------
             |              |
             |              |
             |----(0,0)-----|
             |              |
             |              |
             ----------------
            (1,-1)         (-1,-1)




      [4/15/16]             [11/12/19]
            ------------------
            |\              | \
            | \(1,1)[0/7/18]|  \ (-1,1)[3/8/17]
            |  \ ----------------
            |    |          |   |
            |- - |- - - - - -   |
   [6/13/22] \   | [9/14/21] \  |
              \  |            \ |
               \ |             \|
                 ----------------
            (1,-1)[2/5/20]     (-1,-1)[1/10/23]


            correct

            (-1,1)          (1,1)
             ----------------
             |              |
             |              |
             |----(0,0)-----|
             |              |
             |              |
             ----------------
            (-1,-1)         (1,-1)




      [4/15/16]             [11/12/19]
            ------------------
            |\              | \
            | \(-1,1)[0/7/18]  \ (1,1)[3/8/17]
            |  \ ----------------
            |    |          |   |
            |- - |- - - - - -   |
   [6/13/22] \   | [9/14/21] \  |
              \  |            \ |
               \ |             \|
                 ----------------
            (-1,-1)[2/5/20]    (1,-1)[1/10/23]



             * */
            var size = 1f;
            var vertices = new Vector3[]
            {
                //#region test normal
                //new Vector3(-size, size,-size),         // 0
                //new Vector3(size,-size,-size),          // 1
                //new Vector3(-size,-size,-size),         // 2
                //new Vector3(size, size,-size),          // 3
                //#endregion

                // correct position
                //#region front
                //new Vector3(-size, size,-size),         // 0
                //new Vector3(size,-size,-size),          // 1
                //new Vector3(-size,-size,-size),         // 2
                //new Vector3(size, size,-size),          // 3
                //#endregion
                //#region left
                //new Vector3(-size, size,size),           // 4
                //new Vector3(-size,-size,-size),          // 5
                //new Vector3(-size,-size,size),           // 6
                //new Vector3(-size, size,-size),          // 7
                //#endregion
                //#region right
                //new Vector3(size, size,-size),         // 8
                //new Vector3(size,-size,size),          // 9
                //new Vector3(size,-size,-size),         // 10
                //new Vector3(size, size,size),          // 11
                //#endregion
                //#region back
                //new Vector3(size, size,size),          // 12
                //new Vector3(-size,-size,size),           // 13
                //new Vector3(size,-size,size),          // 14
                //new Vector3(-size, size,size),           // 15
                //#endregion
                //#region top
                //new Vector3(-size, size,size),           // 16
                //new Vector3(size, size,-size),         // 17
                //new Vector3(-size, size,-size),          // 18
                //new Vector3(size, size,size),          // 19
                //#endregion
                //#region bottom
                //new Vector3(-size,-size,-size),          // 20
                //new Vector3(size,-size,size),          // 21
                //new Vector3(-size,-size,size),           // 22
                //new Vector3(size,-size,-size),         // 23
                //#endregion


                // incorrect position
                #region front
                                new Vector3(size, size,-size),          // 0
                                new Vector3(-size,-size,-size),         // 1
                                new Vector3(size,-size,-size),          // 2
                                new Vector3(-size, size,-size),         // 3
                #endregion
                #region left
                                new Vector3(size, size,size),           // 4
                                new Vector3(size,-size,-size),          // 5
                                new Vector3(size,-size,size),           // 6
                                new Vector3(size, size,-size),          // 7
                #endregion
                #region right
                                new Vector3(-size, size,-size),         // 8
                                new Vector3(-size,-size,size),          // 9
                                new Vector3(-size,-size,-size),         // 10
                                new Vector3(-size, size,size),          // 11
                #endregion
                #region back
                                new Vector3(-size, size,size),          // 12
                                new Vector3(size,-size,size),           // 13
                                new Vector3(-size,-size,size),          // 14
                                new Vector3(size, size,size),           // 15
                #endregion
                #region top
                                new Vector3(size, size,size),           // 16
                                new Vector3(-size, size,-size),         // 17
                                new Vector3(size, size,-size),          // 18
                                new Vector3(-size, size,size),          // 19
                #endregion
                #region bottom
                                new Vector3(size,-size,-size),          // 20
                                new Vector3(-size,-size,size),          // 21
                                new Vector3(size,-size,size),           // 22
                                new Vector3(-size,-size,-size),         // 23
                #endregion
            };
            var indices = new int[vertices.Length / 4 * 6];
            var idx = 0;
            //for (int i = 0; i < 36; i+=6)
            for (int i = 0; i < indices.Length; i+=6)
            {
                indices[i + 0] = 0 + 4 * idx;
                indices[i + 1] = 1 + 4 * idx;
                indices[i + 2] = 2 + 4 * idx;
                indices[i + 3] = 0 + 4 * idx;
                indices[i + 4] = 3 + 4 * idx;
                indices[i + 5] = 1 + 4 * idx;
                idx++;
            }
            var uvs = new Vector2[vertices.Length];
            //var uvScale = 3;
            //var offsetU = -1.5f;
            //var offsetV = -1.5f;
            var uvScale = 1;
            var offsetU = 0;
            var offsetV = 0;
            for (int i = 0; i < vertices.Length; i+=4)
            {
                uvs[i + 0] = new Vector2(0, 0) * uvScale + new Vector2(offsetU, offsetV);    // 0
                uvs[i + 1] = new Vector2(1, 1) * uvScale + new Vector2(offsetU, offsetV);    // 1
                uvs[i + 2] = new Vector2(0, 1) * uvScale + new Vector2(offsetU, offsetV);    // 2
                uvs[i + 3] = new Vector2(1, 0) * uvScale + new Vector2(offsetU, offsetV);    // 3
            }
            var colors = new ColorNormalized[vertices.Length];
            for (int i = 0; i < colors.Length; i+=4)
            {
                //colors[i + 0] = ColorNormalized.red;
                //colors[i + 1] = ColorNormalized.green;
                //colors[i + 2] = ColorNormalized.blue;
                //colors[i + 3] = ColorNormalized.yellow;
                colors[i + 0] = ColorNormalized.yellow;
                colors[i + 1] = ColorNormalized.yellow;
                colors[i + 2] = ColorNormalized.yellow;
                colors[i + 3] = ColorNormalized.yellow;
            }
            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.uv = uvs;
            mesh.colors = colors;
            mesh.CaculateNormalAndTangent();
            go.Mesh = mesh;
            // 第一个游戏对象
            gameObjs.Add(go);

            colors = new ColorNormalized[vertices.Length];
            for (int i = 0; i < colors.Length; i += 4)
            {
                //colors[i + 0] = ColorNormalized.lightBlue;
                //colors[i + 1] = ColorNormalized.pink;
                //colors[i + 2] = ColorNormalized.purple;
                //colors[i + 3] = ColorNormalized.green;
                colors[i + 0] = ColorNormalized.blue;
                colors[i + 1] = ColorNormalized.blue;
                colors[i + 2] = ColorNormalized.blue;
                colors[i + 3] = ColorNormalized.blue;
            }
            go = new GameObject("Cube1");
            go.LocalPosition = new Vector3(1, 0, -1);
            mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.uv = uvs;
            mesh.colors = colors;
            mesh.CaculateNormalAndTangent();
            go.Mesh = mesh;
            // 第二个游戏对象
            //gameObjs.Add(go);

            camera = new Camera();
            camera.aspect = 1;
            camera.TranslateTo(new Vector3(0, 0, 8));
            // 暂时使用正交来测试
            // 透视有校正问题没处理好
            camera.isOrtho = false;
            if (camera.isOrtho)
            {
                camera.size = 5;
            }
            else
            {
                camera.TranslateTo(new Vector3(0, 0, 10));
            }

#if PROGRAMMABLE_PIPELINE
            renderer.ShaderData = shaderData = new ShaderData(1);

            renderer.ShaderMgr.Load("Shaders/SoftRendererShader.dll");

            var vs_shaderName = "MyTestVSShader";
            var vs_shaderHash = vs_shaderName.GetHashCode();

            var fs_shaderName = "MyTestFSShader";
            var fs_shaderHash = fs_shaderName.GetHashCode();

            var vsShader = renderer.ShaderMgr.CreateShader(vs_shaderHash);
            var fsShader = renderer.ShaderMgr.CreateShader(fs_shaderHash);

            //var tex_bmp = new Bitmap("Images/texture.png");
            var tex_bmp = new Bitmap("Images/GitHubIcon.PNG");
            //var tex_bmp = new Bitmap("Images/heightMap1.jpg");
            //var tex_bmp = new Bitmap("Images/tex.jpg");
            //var tex_bmp = new Bitmap("Images/icon.PNG");
            var tex = new Texture2D(tex_bmp);
            fsShader.ShaderProperties.SetUniform("mainTex", tex);

            foreach (var go1 in gameObjs)
            {
                go1.Material = new Material(vsShader, fsShader);
            }
#endif
        }

        private void GlobalMsg_OnWheelEvent(bool fromDownToUp)
        {
            if (!PictureBox.Focused) return;
            if (fromDownToUp)
            {
                camera.Translate += (camera.Euler + Vector3.forward).normalized;
            }
            else
            {
                camera.Translate -= (camera.Euler + Vector3.forward).normalized;
            }
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if ((m.Msg == GlobalMessageHandler.WM_KEYDOWN) || (m.Msg == GlobalMessageHandler.WM_SYSKEYDOWN) || (m.Msg == GlobalMessageHandler.WM_KEYUP))
            {
                var down = (m.Msg == GlobalMessageHandler.WM_KEYDOWN || m.Msg == GlobalMessageHandler.WM_SYSKEYDOWN);
                if (m.Msg == GlobalMessageHandler.WM_KEYUP) down = false;

                this.globalMsg.SetKeyDown(keyData, down);
                //Console.WriteLine($"SetKey:{keyData}, down:{down}");

                //switch (keyData)
                //{
                //    case Keys.Down:
                //        Debug.WriteLine("Down Arrow Captured");
                //        break;

                //    case Keys.Up:
                //        Debug.WriteLine("Up Arrow Captured");
                //        break;

                //    case Keys.Tab:
                //        Debug.WriteLine("Tab Key Captured");
                //        break;

                //    case Keys.Control | Keys.M:
                //        Debug.WriteLine("<CTRL> + M Captured");
                //        break;

                //    case Keys.Alt | Keys.Z:
                //        Debug.WriteLine("<ALT> + Z Captured");
                //        break;
                //    default:
                //        Debug.WriteLine($"Unhandle {keyData} Captured");
                //        break;
                //}
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == GlobalMessageHandler.WM_ACTIVATEAPP)
            {
                this.globalMsg.appActive = (((int)m.WParam != 0));
                //Console.WriteLine($"Win actived:{this.globalMsg.appActive}");
                return;
            }
            base.WndProc(ref m);
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox.Focus();
        }

        private void RenderBtn_Click(object sender, EventArgs e)
        {
            Draw();
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            renderer.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long nowTick = DateTime.Now.Ticks;

            float dt = nowTick - lastTick;
            dt /= 10000f; // to ms
            if (lastTick == -1) dt = timer1.Interval;

            if (!paused)
            {
                var usingDt = dt * TimeScaleSlider.Value / 500f;
                Update(usingDt);
                UpdateGameObjs(usingDt);
                Draw();
                PictureBox.Image = renderer.SwapBuffer();

                if (fpsShowType == FpsShowType.Immediately)
                {
                    var fps = timer1.Interval / (float)dt * 60;
                    FpsLabel1.Text = $"FPS:{fps.ToString("0.00")}";
                }
                else
                {
                    elapsedTime += dt;
                    fpsCounter++;
                    if (elapsedTime >= 1000)
                    {
                        FpsLabel1.Text = $"FPS:{fpsCounter}";
                        elapsedTime %= 1000;
                        fpsCounter = 0;
                    }
                }
            }
            lastTick = nowTick;

            var curCtrl = GetFocusedControl();
            focusCtrlLabel.Text = curCtrl != null ? $"Focus:{curCtrl.Name}" : "";
            PictureBox.BorderStyle = PictureBox.Focused ? BorderStyle.Fixed3D : BorderStyle.None;

            UpdateCameraTR();
        }

        private Vector2 RanVec()
        {
            return Vector2.Ran(ran.Next(1, 5));
        }

        private void UpdateCameraTR()
        {
            if (PictureBox.Focused)
            {
                if (this.globalMsg.GetKeyDown(Keys.ControlKey) && this.globalMsg.MS_LBTN)
                {
                    Console.WriteLine($"dx,y:{this.globalMsg.delta_ms_pos}");
                    if (this.globalMsg.delta_ms_pos.IsEmpty == false)
                    {
                        Ry += this.globalMsg.delta_ms_pos.X;
                        Rx -= this.globalMsg.delta_ms_pos.Y;
                        this.globalMsg.delta_ms_pos = Point.Empty;
                    }
                }
                if (this.globalMsg.MS_RBTN)
                {
                    if (this.globalMsg.delta_ms_pos.IsEmpty == false)
                    {
                        Ry += this.globalMsg.delta_ms_pos.X;
                        Rx -= this.globalMsg.delta_ms_pos.Y;
                        this.globalMsg.delta_ms_pos = Point.Empty;
                    }
                    //if (this.globalMsg.GetKeyDown(Keys.W)) camera.Translate -= camera.forward;
                    //if (this.globalMsg.GetKeyDown(Keys.S)) camera.Translate += camera.forward;
                    //if (this.globalMsg.GetKeyDown(Keys.A)) camera.Translate -= camera.forward;
                    //if (this.globalMsg.GetKeyDown(Keys.D)) camera.Translate += camera.forward;
                }
            }
        }

        private void Update(float deltaMs)
        {
            // camera update
            // 后面封装承Component
            camera.TranslateTo(new Vector3(Tx, Ty, Tz));
            camera.RotateTo(new Vector3(Rx, Ry, Rz));
            camera.Update(deltaMs);

            if (camera.viewport == Rectangle.Empty)
            {
                camera.viewport = new Rectangle(0, 0, buff_size, buff_size);
            }
            // renderer state update
            renderer.State.CameraFar = camera.far;
            renderer.State.CameraNear = camera.near;
            renderer.State.CameraViewport = camera.viewport;
            renderer.State.IsOrtho = camera.isOrtho;

#if PROGRAMMABLE_PIPELINE
            // shader uniform block update
            shaderData.Ambient = new ColorNormalized(0.3f, 0.2f, 0.1f, 0.7f);
            // directional light
            shaderData.LightPos[0] = new Vector3(1, -0.5f, 1).normalized;
            shaderData.LightPos[0].w = 0;// directional light
            shaderData.LightColor[0] = new ColorNormalized(1, 0.5f, 0.5f, 1f);
            shaderData.LightItensity[0] = Vector4.one;
            shaderData.LightParams1[0] = Vector4.one;
            shaderData.CameraPos = camera.Translate;
            shaderData.CameraParams = new Vector4(camera.near, camera.far, 0, 0);

            shaderData.NowDataWriteToBuff();

            for (int i = 0; i < gameObjs.Count; i++)
            {
                gameObjs[i].Material.VS.ShaderProperties.SetUniform(outlineOffsetHash, normalOutlineOffset);
            }
#else
            renderer.State.CamX = camera.Translate.x;
            renderer.State.CamY = camera.Translate.y;
            renderer.State.CamZ = camera.Translate.z;
#endif
        }

        private void Draw()
        {
            renderer.Clear();
#if PROGRAMMABLE_PIPELINE
            for (int i = 0; i < gameObjs.Count; i++)
            {
                gameObjs[i].Draw();
            }
#else
            //var srcDepthOffset = renderer.State.DepthOffset;
            //var srcDepthOffsetFactor = renderer.State.DepthOffsetFactor;
            //var srcDepthOffsetUnity = renderer.State.DepthOffsetUnit;

            for (int j = 0; j < triangles.Count; j++)
            {
                //if (j >= 12)
                //{
                //    //renderer.State.DepthOffset = DepthOffset.On;
                //    //renderer.State.DepthOffsetFactor = -1;
                //    //renderer.State.DepthOffsetUnit = -1;
                //}
                renderer.Rasterizer.DrawTriangle(triangles[j], j < 12 ? Color.Red : Color.Green, Color.White);

                //if (j >= 12)
                //{
                //    renderer.State.DepthOffset = srcDepthOffset;
                //    renderer.State.DepthOffsetFactor = srcDepthOffsetFactor;
                //    renderer.State.DepthOffsetUnit = srcDepthOffsetUnity;
                //}
            }
#endif
        }

        private void UpdateGameObjs(float deltaMs)
        {
            GameObject target = null;
            var len = gameObjs.Count;
            for (int i = 0; i < len; i++)
            {
                var go = gameObjs[i];
                go.UpdateTransform(camera);
                go.Update(deltaMs);
                target = go;
            }
            if (target != null)
            {
                //camera.target = target.WorldPosition;
            }
            // dummy : vertex shader
            var cx = camera.viewport.X;
            var cy = camera.viewport.Y;
            var cw = camera.viewport.Width;
            var ch = camera.viewport.Height;
            var f = camera.far;
            var n = camera.near;

            triangles.Clear();
            for (int i = 0; i < len; i++)
            {
                var go = gameObjs[i];
                if (autoRotate)
                {
                    go.LocalRotation += new Vector3(0.1f + i * 0.01f, 0.015f + i * 0.02f, 0.016f + i * 0.03f) * deltaMs;
                }
#if !PROGRAMMABLE_PIPELINE
                if (go.Mesh != null)
                {
                    var mesh = go.Mesh;
                    wposList.Clear();
                    for (int j = 0; j < mesh.vertices.Length; j++)
                    {
                        var clipPos = Matrix4x4.MulPos4(go.ModelViewProjMat, mesh.vertices[j]);
                        // ndc:https://blog.csdn.net/linjf520/article/details/95770635
                        Vector4 ndcPos;
                        if (!camera.isOrtho)
                        {
                            // 透视投影需要处理，透视除法
                            // http://www.songho.ca/opengl/gl_projectionmatrix.html#perspective
                            ndcPos = clipPos * (1 / clipPos.w);
                        }
                        else
                        {
                            ndcPos = clipPos;
                        }
                        // window pos:
                        var wposX = cw * 0.5f * ndcPos.x + (cx + cw * 0.5f);
                        var wposY = ch * 0.5f * ndcPos.y + (cy + ch * 0.5f);
                        var wposZ = (f - n) * 0.5f * ndcPos.z + (f + n) * 0.5f;
                        var winPos = new Vector3(wposX, wposY, wposZ);
                        wposList.Add(winPos);
                    }
                    // polygon mode, now only has the triangle polygon
                    // triangles
                    
                    for (int j = 0; j < mesh.triangles.Length; j += 3)
                    {
                        var idx0 = mesh.triangles[j];
                        var idx1 = mesh.triangles[j + 1];
                        var idx2 = mesh.triangles[j + 2];
                        triangles.Add(new Triangle(wposList[idx0], wposList[idx1], wposList[idx2]));
                    }

                    // draw the triangle3D
                    //renderer.Rasterizer.DrawTriangle()
                    //for (int j = 0; j < triangles.Count; j++)
                    //{
                    //    renderer.Rasterizer.DrawTriangle(triangles[j], Color.Pink, Color.White);
                    //}
                }
#endif
            }
        }

        private void PauseBtn_Click(object sender, EventArgs e)
        {
            paused = !paused;
            PauseBtn.Text = paused ? "Continue" : "Pause";
        }

        private void SelectCameraBtn_Click(object sender, EventArgs e)
        {
            PropertyGrid.SelectedObject = camera;
        }

        private void SelectRendererBtn_Click(object sender, EventArgs e)
        {
            PropertyGrid.SelectedObject = renderer;
        }

        private void SelectFormBtn_Click(object sender, EventArgs e)
        {
            PropertyGrid.SelectedObject = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var srcPaused = paused;
            paused = false;
            statusLabel.Text = $"Snapshotting...";
            if (!Directory.Exists("Snapshots")) Directory.CreateDirectory("Snapshots");
            var path = $"Snapshots/{DateTime.Now.Ticks}.png";
            renderer.BackbuffSaveAs(path);
            statusLabel.Text = $"Snapshot : {path} Complete.";
            paused = srcPaused;
        }

        private void resetTRS_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gameObjs.Count; i++)
            {
                gameObjs[i].LocalPosition = Vector3.zero;
                gameObjs[i].LocalRotation = Vector3.zero;
                gameObjs[i].LocalScale = Vector3.one;
            }
        }

        private void TimeScaleSlider_ValueChanged(object sender, EventArgs e)
        {
            TimeScaleValueLabel.Text = TimeScaleSlider.Value.ToString();
        }
    }
}