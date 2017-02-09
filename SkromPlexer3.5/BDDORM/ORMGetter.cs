using System.Collections.Generic;

namespace SkromPlexer.BDDORM
{
    public class ORMGetter
    {
        private static Dictionary<string, ORM> ORM;

        public static void SetORM(string index, ORM orm)
        {
            if (ORM == null)
                ORM = new Dictionary<string, ORM>();
            ORM.Add(index, orm);
        }

        public static ORM GetORM(string index)
        {
            return (ORM[index]);
        }
    }
}
