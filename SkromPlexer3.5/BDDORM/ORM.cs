using System;
using System.Collections;
using System.Collections.Generic;

namespace SkromPlexer.BDDORM
{
    public class ORMTuple
    {
        public string field;
        public object value;
    }

    public interface ORM
    {
        void Connect();
        ulong GetAutoIncrement(Type type, ORMManager manager);
        List<T> Select<T>();
        List<T> Select<T>(ORMTuple[] where);
        int Insert(object obj, ORMManager manager);
        int Update(object obj, ORMManager manager);
        int Delete(object obj, ORMManager manager);
        int Truncate(Type t);
        int Save(IList objects, Type t, ORMManager manager);
    }
}
