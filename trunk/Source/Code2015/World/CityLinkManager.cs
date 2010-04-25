using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Scene;

namespace Code2015.World
{
    public class CityLinkManager
    {
        struct Entry
        {
            public CityObject A;
            public CityObject B;

            public override int GetHashCode()
            {
                return A.GetHashCode() + B.GetHashCode();
            }
            
        }

        class Comparar : IEqualityComparer<Entry>
        {
            #region IEqualityComparer<Entry> 成员

            public bool Equals(Entry x, Entry y)
            {
                return (x.A == y.A && x.B == y.B) || (x.A == y.B && x.B == y.A);
            }

            public int GetHashCode(Entry obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }

        Dictionary<Entry, CityLinkObject> linkTable = new Dictionary<Entry, CityLinkObject>(new Comparar());

        public CityLinkManager()
        {

        }

        public void Link(RenderSystem rs, SceneManagerBase mgr, CityObject a, CityObject b)
        {
            Entry e;
            e.A = a;
            e.B = b;

            CityLinkObject link;
            if (!linkTable.TryGetValue(e, out link))
            {
                link = new CityLinkObject(rs, a, b);
                linkTable.Add(e, link);
                mgr.AddObjectToScene(link);
            }
        }

        public void Unlink(SceneManagerBase mgr, CityObject a, CityObject b)
        {
            Entry e;
            e.A = a;
            e.B = b;

            CityLinkObject link;
            if (linkTable.TryGetValue(e, out link))
            {
                
                linkTable.Remove(e);
                mgr.RemoveObjectFromScene(link);
            }
        }
    }
}
