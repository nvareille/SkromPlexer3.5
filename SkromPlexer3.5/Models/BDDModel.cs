using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkromPlexer.BDDORM;

namespace SkromPlexer.Models
{
    public delegate IList Binder();

    public abstract class BDDModel : Model
    {
        public Type ORMManagerType;
        public static Dictionary<Type, ORMManager> ManagerOf;
        public static Dictionary<Type, IList> Backup;

        [BDDField, Primary]
        public int Id;

        protected BDDModel(Type m)
        {
            ORMManagerType = m;

            if (ManagerOf == null)
            {
                ManagerOf = new Dictionary<Type, ORMManager>();
                Backup = new Dictionary<Type, IList>();
            }

            if (!ManagerOf.ContainsKey(GetType()) && m != null)
                ManagerOf.Add(GetType(), (ORMManager)Activator.CreateInstance(m));
        }

        public void SetBackupTarget(IList l)
        {
            if (l != null)
                Backup.Add(ORMManagerType, l);
        }

        public static void InitBDDModel(BDDModel model, Binder function, IList list = null)
        {
            model.SetBackupTarget(function());
            list?.Add(model);
        }

        public abstract ORMManager GetManager();
    }
}
