using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MySql.Data.MySqlClient;
using SkromPlexer.Configuration;
using SkromPlexer.Models;
using SkromPlexer.Tools;

#pragma warning disable 0649

namespace SkromPlexer.BDDORM
{
    public class MysqlBDDProfile
    {
        public string Host;
        public string Login;
        public string Password;
        public string Database;
    }

    public class MysqlBDDConfig
    {
        public Dictionary<string, MysqlBDDProfile> Profiles;
    }

    public class MysqlBDD : AConfigurable, ORM
    {
        private MysqlBDDConfig MysqlBDDConfig;
        private MysqlBDDProfile Profile;
        private MySqlConnection Connection;

        public MysqlBDD(string id)
        {
            ORMGetter.SetORM(id, this);
            Profile = MysqlBDDConfig.Profiles[id];
            Log.Info("({0})\n", Profile.Database);
        }

        public void Connect()
        {
            string auth = "SERVER=" + Profile.Host + ";" + "DATABASE=" + Profile.Database + ";" + "UID=" + Profile.Login + ";" + "PASSWORD=" + Profile.Password + ";";

            try
            {
                Connection = new MySqlConnection(auth);
                Connection.Open();
            }
            catch (MySqlException e)
            {
                Log.Error("Impossible to connect database {0}\n", Profile.Database);
                throw e;
            }
        }

        public ulong GetAutoIncrement(Type t, ORMManager manager)
        {
            ulong value;
            MySqlCommand command = new MySqlCommand(manager.BuildAutoIncrementString(Profile.Database), Connection);

            command.Prepare();
            using (MySqlDataReader result = command.ExecuteReader())
            {
                result.Read();
                value = (ulong)result["Auto_increment"];
            }
            return (value);
        }

        public void BuildModel(MySqlDataReader data, object obj)
        {
            FieldInfo Current;

            try
            {
                foreach (var field in obj.GetType().GetFields())
                {
                    if (ORMManager.ContainsAttribute(typeof(BDDField), field))
                    {
                        Current = field;
                        field.SetValue(obj, data[field.Name]);
                    }
                }
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

        public List<T> Select<T>()
        {
            return (Select<T>(null));
        }

        public List<T> Select<T>(ORMTuple[] where)
        {
            try
            {
                List<T> r = new List<T>();
                List<string> values = new List<string>();
                MySqlCommand command = new MySqlCommand("SELECT * FROM " + ORMManager.GetTableName(typeof(T)) + " WHERE ", Connection);

                if (where != null)
                {
                    foreach (ORMTuple tuple in where)
                    {
                        values.Add(tuple.field + " = @" + tuple.field);
                        command.Parameters.AddWithValue("@" + tuple.field, tuple.value);
                    }
                }
                else
                    values.Add("1");

                command.CommandText += string.Join(" AND ", values.ToArray());

                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        Model item = Activator.CreateInstance<T>() as Model;
                        T castItem = item.Convert<T>();

                        BuildModel(result, item);
                        r.Add(castItem);
                    }
                }

                return (r);
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

        public int Insert(object obj, ORMManager manager)
        {
            if (obj.GetType().IsGenericType)
                return (InsertList((IList)obj, manager));

            List<object> l = new List<object>();

            l.Add(obj);
            return (InsertList(l, manager));
        }

        public int InsertList(IList objects, ORMManager manager)
        {
            if (objects.Count <= 0)
                return (0);

            Model model = objects[0] as Model;
            FieldInfo primary = ORMManager.GetPrimaryKey(model.GetType());
            MySqlCommand command = new MySqlCommand(manager.BuildInsertString(model), Connection);
            bool preparationDone = false;
            int result = 0;

            command.Prepare();

            foreach (Model item in objects)
            {
                foreach (FieldInfo field in item.GetType().GetFields())
                {
                    if (ORMManager.ContainsAttribute(typeof(BDDField), field) && (!ORMManager.ContainsAttribute(typeof(Primary), field) || ((int)field.GetValue(item) != 0)))
                    {
                        if (!preparationDone)
                            command.Parameters.AddWithValue("@" + field.Name, field.GetValue(item));
                        else
                            command.Parameters["@" + field.Name].Value = field.GetValue(item);
                    }
                }
                preparationDone = true;
                result += command.ExecuteNonQuery();

                if (primary != null && (int)primary.GetValue(item) == 0)
                    primary.SetValue(model, (Int32)command.LastInsertedId);
            }
            return (result);
        }

        public int Update(object obj, ORMManager manager)
        {
            if (obj.GetType().IsGenericType)
                return (UpdateList((IList)obj, manager));

            List<object> l = new List<object> { obj };

            return (UpdateList(l, manager));
        }

        public int UpdateList(IList objects, ORMManager manager)
        {
            if (objects.Count <= 0)
                return (0);

            Model model = objects[0] as Model;
            MySqlCommand command = new MySqlCommand(manager.BuildUpdateString(), Connection);
            bool preparationDone = false;
            int result = 0;

            foreach (Model item in objects)
            {
                foreach (FieldInfo field in item.GetType().GetFields())
                {
                    if (ORMManager.ContainsAttribute(typeof(BDDField), field))
                    {
                        if (!preparationDone)
                            command.Parameters.AddWithValue("@" + field.Name, field.GetValue(item));
                        else
                            command.Parameters["@" + field.Name].Value = field.GetValue(item);
                    }
                }
                preparationDone = true;
                result += command.ExecuteNonQuery();
            }
            return (result);
        }

        public int Truncate(Type t)
        {
            MySqlCommand command = new MySqlCommand("TRUNCATE TABLE " + ORMManager.GetTableName(t), Connection);

            return (command.ExecuteNonQuery());
        }

        public int Delete(object obj, ORMManager manager)
        {
            if (obj.GetType().IsGenericType)
                return (DeleteList((IList)obj, manager));

            List<object> l = new List<object>();

            l.Add(obj);
            return (DeleteList(l, manager));
        }

        public int DeleteList(IList objects, ORMManager manager)
        {
            if (objects.Count <= 0)
                return (0);

            Model model = objects[0] as Model;
            FieldInfo primary = ORMManager.GetPrimaryKey(model.GetType());
            MySqlCommand command = new MySqlCommand(manager.BuildDeleteString(), Connection);
            bool preparationDone = false;
            int result = 0;

            command.Prepare();

            foreach (object item in objects)
            {
                if (!preparationDone)
                    command.Parameters.AddWithValue("@" + primary.Name, primary.GetValue(item));
                else
                    command.Parameters["@" + primary.Name].Value = primary.GetValue(item);
                preparationDone = true;
                result += command.ExecuteNonQuery();
            }
            return (result);
        }

        public int Save(IList objects, Type t, ORMManager manager)
        {
            Truncate(t);
            return (Insert(objects,  manager));
        }
    }
}
