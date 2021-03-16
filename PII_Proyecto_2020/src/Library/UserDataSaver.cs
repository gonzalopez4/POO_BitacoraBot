using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.Json;

namespace Library
{
    /// <summary>
    /// UserDataSaver: Clase encargada de leer y guardar la informacion del usuario.
    /// 
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, almacenar la informacion del usuario.
    /// OCP: Cumple el principio al poder ser extendido sin verse afectado.
    /// Expert: Cumple el patron al ser experto en la informacion que utiliza.
    /// </summary>
    public class UserDataSaver
    {
        private List<FieldInfo> readOrder { get; set; }
        public Reflection metacogRef;
        public Reflection weeklyRef;
        public WeeklyObjective weeklyObj;
        public WeeklyPlanning weeklyPlan;
        public Semester semester;
        public string[] order = null;

        ///Read: Metodo encargado de leer los datos del usuario.
        public UserDataSaver Read(int chatId)
        {
            if(!Directory.Exists(@"..\Userdata\"))
            {
                Directory.CreateDirectory(@"..\Userdata\");
            }
            if(File.Exists(@"..\Userdata\" + chatId + ".json"))
            {
                dynamic f = JsonConvert.DeserializeObject(File.ReadAllText(@"..\Userdata\" + chatId + ".json"));
                // var f = JArray.Parse(File.ReadAllText(@"..\Userdata\" + chatId + ".json"));

                if(readOrder == null)
                {
                    readOrder = new List<FieldInfo>();

                    if(f.Count != 0)
                    {
                        int i = 0;
                        foreach(var field in typeof(UserDataSaver).GetFields())
                        {
                            field.SetValue(this, f[i].ToObject(field.FieldType));
                            i++;                       
                        }
                    }
                    else
                    {
                        foreach(var field in typeof(UserDataSaver).GetFields())
                        {
                            try
                            {
                                field.SetValue(this, Activator.CreateInstance(field.FieldType));
                            }
                            catch(MissingMethodException)
                            {}
                        }
                        try
                        {
                            metacogRef.Title = "Reflexión Metacognitiva";
                            weeklyRef.Title = "Reflexión Semanal";
                            weeklyPlan.Title = "Planificación Semanal";
                            weeklyObj.Title = "Objetivos Semanales";
                        }
                        catch(NullReferenceException)
                        {}
                        Save(chatId);
                    }
                }
                else
                {
                    if(f.Count != 0)
                    {
                        var temp = new List<FieldInfo>();
                        int i = 0;
                        foreach(var field in readOrder)
                        {
                            field.SetValue(this, f[i].ToObject(field.FieldType));
                            i++;
                        }
                    }
                    else
                    {
                        foreach(var field in typeof(UserDataSaver).GetFields())
                        {
                            try
                            {
                                field.SetValue(this, Activator.CreateInstance(field.FieldType));
                            }
                            catch(MissingMethodException)
                            {}
                        }
                        try
                        {
                            metacogRef.Title = "Reflexión Metacognitiva";
                            weeklyRef.Title = "Reflexión Semanal";
                            weeklyPlan.Title = "Planificación Semanal";
                            weeklyObj.Title = "Objetivos Semanales";
                        }
                        catch(NullReferenceException)
                        {}
                        Save(chatId);
                    }
                }
            }
            else
            {
                foreach(var field in typeof(UserDataSaver).GetFields())
                {
                    try
                    {
                        field.SetValue(this, Activator.CreateInstance(field.FieldType));
                    }
                    catch(MissingMethodException)
                    {}
                }
                try
                {
                    metacogRef.Title = "Reflexión Metacognitiva";
                    weeklyRef.Title = "Reflexión Semanal";
                    weeklyPlan.Title = "Planificación Semanal";
                    weeklyObj.Title = "Objetivos Semanales";
                }
                catch(NullReferenceException)
                {}
                Save(chatId);
            }

            return this;
        }

        ///Save: Metodo encargado de guardar los datos del usuario.
        public UserDataSaver Save(int chatId)
        {
            readOrder = new List<FieldInfo>();
            var data = new List<object>();
            foreach(var field in typeof(UserDataSaver).GetFields())
            {
                readOrder.Add(field);
                data.Add(field.GetValue(this));
            }

            if(!Directory.Exists(@"..\Userdata\"))
            {
                Directory.CreateDirectory(@"..\Userdata\");
            }
            File.WriteAllText(@"..\Userdata\"+ chatId + ".json", JsonConvert.SerializeObject( data.ToArray(), Formatting.Indented));

            return this;
        }
    }
}

