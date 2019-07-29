﻿// jave.lin 2019.07.18
using RendererCommon.SoftRenderer.Common.Shader;
using SoftRenderer.Common.Mathes;
using SoftRenderer.SoftRenderer;
using SoftRenderer.SoftRenderer.Primitives;
using System;
using System.ComponentModel;
using System.Drawing;

namespace SoftRenderer.Games
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("网格对象")]
    public class Mesh
    {
        public Vector3[] vertices;                  // 顶点坐标，目前使用到
        public int[] triangles { get; set; }        // 顶点索引，目前使用到

        public Vector3[] normals { get; set; }      // 顶点法线，暂时没用到
        public Vector3[] tangents { get; set; }     // 顶点切线，暂时没用到
        public Vector2[] uv { get; set; }           // 顶点uv，暂时没用到
        public ColorNormalized[] colors { get; set; }         // 顶点颜色，暂时没用到
    }

    [Description("网格渲染器")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MeshRenderer
    {
        public Mesh Mesh;
        public Renderer Renderer;

        public MeshRenderer()
        {
            Renderer = Renderer.Instance;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("摄像机")]
    public class Camera : IDisposable
    {
        // transform
        [Category("transform")]
        [Description("视图变换")]
        public Matrix4x4 View { get; private set; }
        [Category("transform")]
        [Description("投影变换")]
        public Matrix4x4 Proj { get; private set; }

        // look at
        [Category("look at")]
        [Description("是否有看向的目标点")]
        public Vector3? target { get; set; }
        [Category("look at")]
        [Description("看着目标点时镜头向上的方位")]
        public Vector3 up { get; set; } = Vector3.up;

        [Category("Viewport")]
        [Description("Window视图")]
        public Rectangle viewport { get; set; } = Rectangle.Empty;

        // view
        [Category("view")]
        [Description("位移量")]
        public Vector3 Translate { get; set; }              // 位移量
        [Category("view")]
        [Description("旋转量")]
        public Vector3 Euler { get; set; }                  // 旋转量
                                                            // proj
                                                            // proj-ortho
        [Category("proj-ortho")]
        [Description("屏幕高度的一半")]
        public float size { get; set; } = 3;              // 屏幕高度的一半，单位：像素
                                                          // proj-perspective
        [Category("proj-perspective")]
        [Description("纵向张开角度")]
        public float fov { get; set; } = 60f;               // 纵向张开角度
                                                            // proj-both
        [Category("proj-both")]
        [Description("宽高比")]
        public float aspect { get; set; } = 800 / 600f;     // 宽高比
        [Category("proj-both")]
        [Description("近裁面，必须大于0")]
        public float near { get; set; } = 0.3f;             // 近裁面，必须大于0
        [Category("proj-both")]
        [Description("远裁面")]
        public float far { get; set; } = 1000f;             // 远裁面
        [Category("proj-both")]
        [Description("是否使用正交投影")]
        public bool isOrtho { get; set; } = false;          // 是否使用正交投影

        public Camera()
        {
            View = Matrix4x4.GetMat();
        }
        public void Move(Vector3 t)
        {
            Translate += t;
        }
        public void TranslateTo(Vector3 t)
        {
            Translate = t;
        }
        public void Rotate(Vector3 e)
        {
            Euler += e;
        }
        public void RotateTo(Vector3 e)
        {
            Euler = e;
        }
        public void Update(float delaMs)
        {
            // view
            Vector3 t = this.Translate;
            if (target.HasValue)
            {
                // look at
                View = Matrix4x4.GenLookAt(t, target.Value, up);
                // 将view matrix的前三行三列(3x3)，每一行对应：Left, Up, Forward三轴，来求得各个轴的当前角度
                // 因为LookAt实际上是求了view的反向变换：逆矩阵，下面的矩阵的转置为原来的变换矩阵
                /*
                 * view 是右手坐标系
                 * lx,ly,lz是x，左边为正，所以命名为：Left:l
                 * ux,uy,uz是y，上边为正，所以命名为：Up:u
                 * fx,fy,fz是z，向前为正，所以命名为：Forward:f
                 * http://www.songho.ca/opengl/gl_camera.html#lookat
                 |lx|ly|lz|0 |  |1 |0 |0 |-tx|  |lx|ly|lz|lx*(-tx)+ly*(-ty)+lz*(-tz)|
                 |ux|uy|uz|0 |* |0 |1 |0 |-ty|= |ux|uy|uz|ux*(-tx)+uy*(-ty)+uz*(-tz)|
                 |fx|fy|fz|0 |  |0 |0 |1 |-tz|  |fx|fy|fz|fx*(-tx)+fy*(-ty)+fz*(-tz)|
                 |0 | 0| 0|1 |  |0 |0 |0 |1  |  |0 |0 |0 |1                         |
                 * */
                // 后面再实现
            }
            else
            {
                // translate
                View = Matrix4x4.GenTranslateMat(t.x, t.y, t.z);
                // rotate
                View = Matrix4x4.GenEulerMat(Euler.x, Euler.y, Euler.z).MulMat(View);
            }
            // proj
            if (isOrtho)
            {
                // orthogonal
                Proj = Matrix4x4.GenOrthoFrustum(aspect, size, near, far);
            }
            else
            {
                // perspective
                Proj = Matrix4x4.GenFrustum(fov, aspect, near, far);
            }
        }
        public void Dispose()
        {
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("游戏对象")]
    public class GameObject : IDisposable
    {
        private static readonly int MVP_Hash = "MVP".GetHashCode();

        private const int poolSize = 128;
        private static Triangle[] pool = new Triangle[poolSize];
        private static int topIdx = -1;

        private static Triangle fromPool()
        {
            if (topIdx == -1) return new Triangle();
            return pool[topIdx--];
        }
        private static void toPool(Triangle t)
        {
            if (topIdx + 1 < poolSize) pool[++topIdx] = t;
        }

        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }

        public Matrix4x4 ModelMat { get; private set; }

        public Matrix4x4 ModelViewMat { get; private set; }
        public Matrix4x4 ModelViewProjMat { get; private set; }

        [Description("名字")]
        public string Name { get; set; }

        [Description("父对象")]
        public GameObject Parent { get; private set; }
        [Description("本地坐标")]
        public Vector3 LocalPosition { get; set; } = Vector3.zero;
        [Description("本地旋转")]
        public Vector3 LocalRotation { get; set; } = Vector3.zero;
        [Description("本地缩放")]
        public Vector3 LocalScale { get; set; } = Vector3.one;
        [Description("网格对象，后面重构成Component")]
        public Mesh Mesh { get; set; }
        [Description("网格渲染器，后面重构成Component")]
        public MeshRenderer MR { get; set; }
        [Description("材质对象，后面重构成Component")]
        public Material Material { get; set; }
        [Description("世界坐标")]
        public Vector3 WorldPosition { get; private set; }

        public GameObject(string name = null)
        {
            ModelMat = Matrix4x4.GetMat();

            Mesh = new Mesh();
            Name = name;

            MR = new MeshRenderer();
            MR.Mesh = Mesh;
        }

        public void UpdateTransform(Camera camera)
        {
            ModelMat.Identity();
            ModelMat = ModelMat.TRS(LocalPosition, LocalRotation, LocalScale);

            if (Parent != null)
            {
                ModelMat = Parent.ModelMat * ModelMat;
            }

            ModelViewMat = camera.View * ModelMat;
            ModelViewProjMat = camera.Proj * ModelViewMat;

            WorldPosition = ModelMat * Vector4.zeroPos;
        }

        public void Draw()
        {
            if (VertexBuffer == null)
            {
                // Vector3 position;        // 3

                // Vector3 normal;          // 3
                // Vertex3 tangent;         // 3
                // Vector2 uv;              // 2
                // ColorNormalized color;   // 4
                // 

                var count = Mesh.vertices != null ? Mesh.vertices.Length * 3 : 0;
                //count += Mesh.normals != null ? Mesh.normals.Length * 3 : 0;
                //count += Mesh.tangents != null ? Mesh.tangents.Length * 3 : 0;
                count += Mesh.uv != null ? Mesh.uv.Length * 2 : 0;
                count += Mesh.colors != null ? Mesh.colors.Length * 4 : 0;

                var perVertexCount =
                    3       // Vector3 
                    + 2     // uv
                    + 4     // color
                    ;

                // 定义顶点格式
                VertexBuffer = new VertexBuffer(count, perVertexCount);

                VertexBuffer.SetFormat(new VertexDataFormat[]
                {
                new VertexDataFormat { type = VertexDataType.Position, num = 0, offset = 0, count = 3 },
                new VertexDataFormat { type = VertexDataType.UV, num = 0, offset = 3, count = 2 },
                new VertexDataFormat { type = VertexDataType.Color, num = 0, offset = 5, count = 4 },
                });

                // 顶点装配索引
                IndexBuffer = new IndexBuffer(Mesh.triangles.Length);
                IndexBuffer.Set(Mesh.triangles);

                // VertexBuffer按需是否需要实时更新到Shader，如果没有变换就不需要，一般不会有变化
                VertexBuffer.writePos = 0;
                var len = Mesh.vertices.Length;
                for (int i = 0; i < len; i++)
                {
                    var v = Mesh.vertices[i];
                    var uv = Mesh.uv[i];
                    var c = Mesh.colors[i];
                    VertexBuffer.Write(v);
                    VertexBuffer.Write(uv);
                    VertexBuffer.Write(c);
                }
            }

            //// VertexBuffer按需是否需要实时更新到Shader，如果没有变换就不需要，一般不会有变化
            //VertexBuffer.writePos = 0;
            //var len = Mesh.vertices.Length;
            //for (int i = 0; i < len; i++)
            //{
            //    var v = Mesh.vertices[i];
            //    var uv = Mesh.uv[i];
            //    var c = Mesh.colors[i];
            //    VertexBuffer.Write(v);
            //    VertexBuffer.Write(uv);
            //    VertexBuffer.Write(c);
            //}

            MR.Renderer.BindVertexBuff(VertexBuffer);
            MR.Renderer.BindIndexBuff(IndexBuffer);

            Material.VS.ShaderProperties.SetUniform(MVP_Hash, ModelViewProjMat);

            MR.Renderer.ShaderProgram.SetShader(ShaderType.VertexShader, Material.VS);
            MR.Renderer.ShaderProgram.SetShader(ShaderType.FragmentShader, Material.FS);

            MR.Renderer.Present();
        }

        public void Update(float deltaMs)
        {
            if (Mesh != null)
            {

            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (VertexBuffer != null)
            {
                VertexBuffer.Dispose();
                VertexBuffer = null;
            }
            if (IndexBuffer != null)
            {
                IndexBuffer.Dispose();
                IndexBuffer = null;
            }
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("材质")]
    public class Material : IDisposable
    {
        public bool DisposedShdaer = false;
        public ShaderBase VS { get; private set; }
        public ShaderBase FS { get; private set; }

        public Material(ShaderBase vs, ShaderBase fs)
        {
            VS = vs;
            FS = fs;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (DisposedShdaer)
            {
                if (VS != null)
                {
                    VS.Dispose();
                    VS = null;
                }
                if (FS != null)
                {
                    FS.Dispose();
                    FS = null;
                }
            }
        }
    }
}