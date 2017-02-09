using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SkromPlexer.Models;
using SkromPlexer.Tools;

namespace SkromPlexer.BDDORM
{
    public class Primary : Attribute
    {
    }

    public class BDDField : Attribute
    {
    }

    public abstract class ORMManager
    {
        private ulong AutoIncrement;
        private Type Type;
        public FieldInfo PrimaryKey;
        public string ORMIndex;
        private ORM ORM;

        public ORMManager(Type t, string index)
        {
            Type = t;
            ORMIndex = index;
            ORM = ORMGetter.GetORM(index);

            try
            {
                AutoIncrement = ORM.GetAutoIncrement(Type, this);
            }
            catch (Exception e)
            {
                Log.Error(String.Format("The model {0} doesn't have an AutoIncrement", GetType()));
                throw new Exception();
            }
            PrimaryKey = GetPrimaryKey(Type);
        }

        public Type GetModelType()
        {
            return (Type);
        }

        public static string GetTableName(Type type)
        {
            return (type.Name.ToLower() + 's');
        }

        public static FieldInfo GetPrimaryKey(Type type)
        {
            foreach (FieldInfo field in type.GetFields())
            {
                if (ContainsAttribute(typeof(Primary), field))
                    return (field);
            }
            return (null);
        }

        public static bool ContainsAttribute(Type attrib, FieldInfo field)
        {
            return (field.GetCustomAttributes(attrib, false).Any());
        }

        public int Insert(object obj)
        {
            return (ORM.Insert(obj, this));
        }

        public int Update(object obj)
        {
            return (ORM.Update(obj, this));
        }

        public int Delete(object obj)
        {
            return (ORM.Delete(obj, this));
        }

        public string BuildInsertString(object obj)
        {
            List<string> fields = new List<string>();
            List<string> values = new List<string>();

            string tableDescription = "INSERT INTO " + GetTableName(Type) + "(";

            foreach (FieldInfo field in Type.GetFields())
            {
                if (ContainsAttribute(typeof(BDDField), field) &&
                    (!ContainsAttribute(typeof(Primary), field) || ((int) field.GetValue(obj) != 0)))
                {
                    fields.Add(field.Name);
                    values.Add("@" + field.Name);
                }
            }

            tableDescription += String.Join(", ", fields.ToArray()) + ") ";
            tableDescription += "VALUES(" + String.Join(", ", values.ToArray()) + ")";

            return (tableDescription);
        }


        public string BuildDeleteString()
        {
            FieldInfo primary = GetPrimaryKey(Type);
            string query = "DELETE FROM " + GetTableName(Type) + " WHERE " + primary.Name + " = @" + primary.Name;

            return (query);
        }

        public static string BuildTotalDeleteString(Type type)
        {
            return ("DELETE FROM " + GetTableName(type) + " WHERE 1");
        }

        public string BuildUpdateString()
        {
            FieldInfo primary = GetPrimaryKey(Type);
            List<string> fields = new List<string>();
            string query = "UPDATE " + GetTableName(Type) + " SET ";

            foreach (FieldInfo field in Type.GetFields())
            {
                if (ContainsAttribute(typeof(BDDField), field) && !ContainsAttribute(typeof(Primary), field))
                    fields.Add(field.Name + " = @" + field.Name);
            }

            query += String.Join(", ", fields.ToArray());
            query += " WHERE " + primary.Name + " = @" + primary.Name;
            return (query);
        }

        public string BuildAutoIncrementString(string database)
        {
            string query = "SHOW TABLE STATUS FROM `" + database + "` LIKE '" + GetTableName(Type) + "'";
            return (query);
        }

        public Model CreateInstance()
        {
            Model model = (Model)Activator.CreateInstance(Type);

            PrimaryKey.SetValue(model, Convert.ToInt32(AutoIncrement++));
            return (model);
        }

        public T CreateInstance<T>()
        {
            T model = (T) Activator.CreateInstance(Type);

            PrimaryKey.SetValue(model, Convert.ToInt32(AutoIncrement++));
            return (model);
        }

        public int Truncate()
        {
            return (ORM.Truncate(Type));
        }

        public int Save(IList list)
        {
            return (ORM.Save(list, Type, this));
        }

        public List<T> Select<T>()
        {
            return (ORM.Select<T>());
        }

        public List<T> Select<T>(ORMTuple[] t)
        {
            return (ORM.Select<T>(t));
        }
    }
}