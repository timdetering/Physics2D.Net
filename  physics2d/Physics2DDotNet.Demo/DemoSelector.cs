using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using Graphics2DDotNet;

namespace Physics2DDotNet.Demo
{
    /// <summary>
    /// This Form is automatically populated with Physics Demos that impliment the IPhysicsDemo interface
    /// and has the PhysicsDemoAttribute
    /// </summary>
    public partial class DemoSelector : Form
    {
        sealed class DemoType
        {
            private Type type;
            private PhysicsDemoAttribute physicsDemoAttribute;
            private ConstructorInfo constructor;
            public DemoType(Type type)
            {
                this.type = type;
                this.physicsDemoAttribute = (PhysicsDemoAttribute)(type.GetCustomAttributes(typeof(PhysicsDemoAttribute), false)[0]);
                this.constructor = type.GetConstructor(Type.EmptyTypes);
            }
            public Type Type
            {
                get { return type; }
            }
            public PhysicsDemoAttribute DemoAttribute
            {
                get { return physicsDemoAttribute; }
            }
            public ConstructorInfo Constructor
            {
                get { return constructor; }
            }
        }
        sealed class TypeComparer : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
        sealed class DemoTypeComparer : IComparer<DemoType>
        {
            public int Compare(DemoType x, DemoType y)
            {
                return x.DemoAttribute.Group.CompareTo(y.DemoAttribute.Group);
            }
        }
        static TypeComparer typeComparer = new TypeComparer();
        static DemoTypeComparer demoTypeComparer = new DemoTypeComparer();
        static bool IsDemoInterface(Type t, object o)
        {
            return t == typeof(IPhysicsDemo);
        }
        static bool IsDemo(Type t, object o)
        {
            return t.GetCustomAttributes(typeof(PhysicsDemoAttribute), false).Length > 0 &&
                 t.FindInterfaces(IsDemoInterface,null).Length >0;
        }
        static Type[] GetDemos()
        {
            List<Type> types = new List<Type>();
            foreach (Module module in Assembly.GetCallingAssembly().GetLoadedModules(true))
            {
                types.AddRange(module.FindTypes(IsDemo, null));
            }
            foreach (Module module in Assembly.GetEntryAssembly().GetLoadedModules(true))
            {
                types.AddRange(module.FindTypes(IsDemo, null));
            }
            foreach (Module module in Assembly.GetExecutingAssembly().GetLoadedModules(true))
            {
                types.AddRange(module.FindTypes(IsDemo, null));
            }
            types.Sort(typeComparer);
            Type last = null;
            types.RemoveAll(delegate(Type t)
            {
                bool result = t == last;
                last = t;
                return result;
            });
            return types.ToArray();
        }


        Window window;
        Viewport viewport;
        Scene scene;
        DemoType selected;
        IPhysicsDemo demo;
        public DemoSelector()
        {
            this.InitializeComponent();
            Type[] types = GetDemos();
            DemoType[] demoTypes = new DemoType[types.Length];
            for (int index = 0; index < types.Length; ++index)
            {
                demoTypes[index] = new DemoType(types[index]);
            }
            Array.Sort<DemoType>(demoTypes, demoTypeComparer);
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();
            lvDemos.Clear();
            lvDemos.Columns.Add("Demo Names", lvDemos.Width - 24, HorizontalAlignment.Left);
            for (int index = 0; index < types.Length; ++index)
            {
                DemoType demoType = demoTypes[index];
                PhysicsDemoAttribute att = demoType.DemoAttribute;
                ListViewGroup group;
                if (!groups.TryGetValue(att.Group, out group))
                {
                    group = new ListViewGroup(att.Group, HorizontalAlignment.Left);
                    groups.Add(att.Group, group);
                    lvDemos.Groups.Add(group);
                }
                ListViewItem item = new ListViewItem(att.Name, group);
                item.Tag = demoType;
                lvDemos.Items.Add(item);
            }
        }
        public void Initialize(Window window, Viewport viewport, Scene scene)
        {
            this.window = window;
            this.viewport = viewport;
            this.scene = scene;
        }

        void Start()
        {
            scene.Timer.IsRunning = false;
            if (demo != null)
            {
                demo.Dispose();
                demo = null;
            }
            viewport.ToScreen = AdvanceMath.Matrix2x3.Identity;
            scene.Clear();
            if (selected != null)
            {
                demo = (IPhysicsDemo)selected.Constructor.Invoke(null);
                demo.Open(new DemoOpenInfo(window, viewport, scene));
            }
            scene.Timer.IsRunning = true;
        }
        void SelectionChanged()
        {
            DemoType type;
            if (lvDemos.SelectedItems.Count == 0 ||
                (type = lvDemos.SelectedItems[0].Tag as DemoType) == null)
            {
                //if nothing is selected;
                this.rtbDescription.Text = String.Empty;
                selected = null;
                bStart.Enabled = false;
            }
            else
            {
                this.rtbDescription.Text = type.DemoAttribute.Description;
                selected = type;
                bStart.Enabled = true;
            }
        }
        private void OnStartClick(object sender, EventArgs e)
        {
            Start();
        }
        private void OnSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            SelectionChanged();
        }
    }
}