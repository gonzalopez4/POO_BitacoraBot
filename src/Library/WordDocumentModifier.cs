using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PII_Word_API;
using System.Collections.Generic;
using TelegramApi;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace Library
{
    /// <summary>
    /// WordDocumentModifier: Clase que implementa la interfaz IDocumentModifier, encargada de reunir la informacion del documento Word.
    /// 
    /// Principios y patrones: 
    /// SRP: Utiliza el principio al tener una unica responsabilidad de modificar el documento Word.
    /// OCP: No utiliza el principio debido a que esta clase no esta diseñada con el objetivo de extenderlo sin modificarlo.
    /// ISP: Utiliza el principio al no depender de tipos que no utiliza.
    /// Expert: Aplica el patron al ser experta en la informacion que utiliza.
    /// Creator: Aplica el patron al ser la responsable de crear instancias de WordDocument.
    /// </summary>
    public class WordDocumentModifier : IDocumentModifier
    {
        private WordDocument doc;

        private Dictionary<string, List<Paragraph>> elements = new Dictionary<string, List<Paragraph>>();

        public WordDocumentModifier()
        {
            foreach(var field in typeof(UserDataSaver).GetFields())
            {
                elements.Add(field.Name, new List<Paragraph>());
            }
            elements.Remove("semester");
            elements.Remove("order");
        }

        public void Create(MessageResponse msgR)
        {
            doc = new WordDocument(@"..\Userdata\" + msgR.chatId + ".docx");

            foreach(var str in msgR.userData.order.Reverse())
            {
                typeof(WordDocumentModifier).GetMethod(str + "Create", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[1]{ msgR });
            }

            Modify(msgR);
        }

        public void Modify(MessageResponse msgR)
        {
            WordprocessingDocument Word = WordprocessingDocument.Open( @"..\Userdata\" + msgR.chatId + ".docx",
                                                                        true );

            List<Paragraph> paragraphs = Word.MainDocumentPart.Document.Body.Descendants<Paragraph>().ToList();

            List<int> indexes = new List<int>();
            foreach(var element in elements)
            {
                var ind = paragraphs.FindIndex( delegate(Paragraph p) { return p.InnerText == typeof(UserDataSaver).GetField(element.Key).GetValue(msgR.userData).GetType().GetProperty("Title").GetValue(typeof(UserDataSaver).GetField(element.Key).GetValue(msgR.userData)).ToString(); });
                indexes.Add(ind);
                ind = paragraphs.FindIndex(ind + 1, delegate(Paragraph p) { return p.InnerText == typeof(UserDataSaver).GetField(element.Key).GetValue(msgR.userData).GetType().GetProperty("Title").GetValue(typeof(UserDataSaver).GetField(element.Key).GetValue(msgR.userData)).ToString(); }); 
                if(ind != -1)
                {
                    indexes.Add(ind);
                }
            }

            indexes.Sort();
            if(indexes.Count < typeof(UserDataSaver).GetFields().Count())
            {
                indexes.Add(paragraphs.Count-2);
            }
            else
            {
                indexes = indexes.GetRange(0,typeof(UserDataSaver).GetFields().Count());
            }

            Dictionary<string,List<Paragraph>> ranges = new Dictionary<string,List<Paragraph>>();
            for( int i = 0; i < msgR.userData.order.Length - 1; i++)
            {
                ranges.Add(msgR.userData.order[i], paragraphs.GetRange(indexes[i], indexes[i+1] - indexes[i]));
            }

            foreach(var pair in ranges)
            {
                var format = (Dictionary<string, Dictionary<string, string>>) typeof(UserDataSaver).GetField(pair.Key).GetValue(msgR.userData).GetType().GetProperty("Format").GetValue(typeof(UserDataSaver).GetField(pair.Key).GetValue(msgR.userData));
                
                if(format.Count != 0)
                {
                    var formatWord = format["Word"];
                    foreach(var f in formatWord)
                    {
                        if(f.Value == null)
                        {
                            formatWord.Remove(f.Key);
                        }
                    }

                    foreach(var f in formatWord)
                    {
                        //Title
                        if(f.Key.ToLower().Contains("titulo") || f.Key.ToLower().Contains("título") || f.Key.ToLower().Contains("title"))
                        {
                            var paragraph = pair.Value[0];
                            if(paragraph.ParagraphProperties == null)
                            {
                                paragraph.ParagraphProperties = new ParagraphProperties();
                            }

                            if(f.Key.ToLower().Contains("alinea") || f.Key.ToLower().Contains("align") || f.Key.ToLower().Contains("justifi"))
                            {
                                if(f.Value.ToLower().Contains("cent"))
                                {
                                    paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
                                }
                                else if(f.Value.ToLower().Contains("izq") || f.Value.ToLower().Contains("left"))
                                {
                                    paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Left };
                                }
                                else if(f.Value.ToLower().Contains("der") || f.Value.ToLower().Contains("right"))
                                {
                                    paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                                }
                                else if(f.Value.ToLower().Contains("just"))
                                {
                                    paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Both };
                                }
                                else
                                {
                                    //VUELVO
                                    msgR.bot.SendMessage("No reconozco el tipo de alineación \"" + f.Value + "\", ¿Deseas cambiarlo por otro?", msgR.chatId);

                                    var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                    if(msgReceived.StartsWith("/"))
                                    {
                                        msgReceived = msgReceived.Substring(1);
                                    }

                                    if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                    {
                                        msgR.bot.SendMessage("Ingrese un nuevo tipo, o /atras para volver.", msgR.chatId);

                                        while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                        {
                                            msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                            if(msgReceived.StartsWith("/"))
                                            {
                                                msgReceived = msgReceived.Substring(1);
                                            }

                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                if(msgReceived.Contains("cent"))
                                                {
                                                    paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
                                                }
                                                else if(msgReceived.Contains("izq") || msgReceived.Contains("left"))
                                                {
                                                    paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Left };
                                                }
                                                else if(msgReceived.Contains("der") || msgReceived.Contains("right"))
                                                {
                                                    paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                                                }
                                                else if(msgReceived.Contains("just"))
                                                {
                                                    paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Both };
                                                }
                                                else
                                                {
                                                    msgR.bot.SendMessage("Ese tampoco lo es 😱, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                }
                                            }
                                        }
                                        if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                        {
                                            Word.Dispose();
                                            return;
                                        }
                                    }
                                }
                            }
                            else if(f.Key.ToLower().Contains("spac"))
                            {
                                if(paragraph.ParagraphProperties.SpacingBetweenLines == null)
                                {
                                    paragraph.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines();
                                }

                                if( f.Key.ToLower().Contains("ant") || f.Key.ToLower().Contains("befor") )
                                {
                                    if(f.Value.All(char.IsDigit))
                                    {
                                        paragraph.ParagraphProperties.SpacingBetweenLines.Before = (Convert.ToUInt32(f.Value)*20).ToString();
                                    }
                                    else
                                    {
                                        //VUELVO
                                        msgR.bot.SendMessage("El valor de espaciado anterior \"" + f.Value + "\" no es válido, ¿Deseas cambiarlo por otro?", msgR.chatId);

                                        var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                        if(msgReceived.StartsWith("/"))
                                        {
                                            msgReceived = msgReceived.Substring(1);
                                        }

                                        if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                        {
                                            msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                if(msgReceived.StartsWith("/"))
                                                {
                                                    msgReceived = msgReceived.Substring(1);
                                                }

                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    if(msgReceived.All(char.IsDigit))
                                                    {
                                                        paragraph.ParagraphProperties.SpacingBetweenLines.After = (Convert.ToUInt32(msgReceived)*20).ToString();
                                                    }
                                                    else
                                                    {
                                                        msgR.bot.SendMessage("Ese tampoco lo es 😫, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                    }
                                                }
                                            }
                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                            {
                                                Word.Dispose();
                                                return;
                                            }
                                        }
                                    }
                                }
                                else if( f.Key.ToLower().Contains("desp") || f.Key.ToLower().Contains("post") || f.Key.ToLower().Contains("aft") )
                                {
                                    if(f.Value.All(char.IsDigit))
                                    {
                                        paragraph.ParagraphProperties.SpacingBetweenLines.After = (Convert.ToUInt32(f.Value)*20).ToString();
                                    }
                                    else
                                    {
                                        //VUELVO
                                        msgR.bot.SendMessage("El valor de espaciado posterior \"" + f.Value + "\" no es válido, ¿Deseas cambiarlo por otro?", msgR.chatId);

                                        var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                        if(msgReceived.StartsWith("/"))
                                        {
                                            msgReceived = msgReceived.Substring(1);
                                        }

                                        if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                        {
                                            msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                if(msgReceived.StartsWith("/"))
                                                {
                                                    msgReceived = msgReceived.Substring(1);
                                                }

                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    if(msgReceived.All(char.IsDigit))
                                                    {
                                                        paragraph.ParagraphProperties.SpacingBetweenLines.After = (Convert.ToUInt32(msgReceived)*20).ToString();
                                                    }
                                                    else
                                                    {
                                                        msgR.bot.SendMessage("Ese tampoco lo es ☹️, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                    }
                                                }
                                            }
                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                            {
                                                Word.Dispose();
                                                return;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if(f.Value.All(delegate(char c) { return char.IsDigit(c) || c == Convert.ToChar(",") || c == Convert.ToChar("."); }))
                                    {
                                        paragraph.ParagraphProperties.SpacingBetweenLines.Line = (Convert.ToInt32(Convert.ToDouble(f.Value.Replace(",", "."))*240)).ToString();
                                    }
                                    else
                                    {
                                        //VUELVO
                                        msgR.bot.SendMessage("El valor de interlineado \"" + f.Value + "\" no es válido, ¿Deseas cambiarlo por otro?", msgR.chatId);

                                        var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                        if(msgReceived.StartsWith("/"))
                                        {
                                            msgReceived = msgReceived.Substring(1);
                                        }

                                        if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                        {
                                            msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                if(msgReceived.StartsWith("/"))
                                                {
                                                    msgReceived = msgReceived.Substring(1);
                                                }

                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    if(msgReceived.All(delegate(char c) { return char.IsDigit(c) || c == Convert.ToChar(",") || c == Convert.ToChar("."); }))
                                                    {
                                                        paragraph.ParagraphProperties.SpacingBetweenLines.Line = (Convert.ToInt32(Convert.ToDouble(msgReceived.Replace(",", "."))*240)).ToString();
                                                    }
                                                    else
                                                    {
                                                        msgR.bot.SendMessage("Ese tampoco lo es 😔, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                    }
                                                }
                                            }
                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                            {
                                                Word.Dispose();
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                            if(f.Key.ToLower().Contains("interl"))
                            {
                                if(paragraph.ParagraphProperties.SpacingBetweenLines == null)
                                {
                                    paragraph.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines();
                                }

                                if(f.Value.All(delegate(char c) { return char.IsDigit(c) || c == Convert.ToChar(",") || c == Convert.ToChar("."); }))
                                {
                                    paragraph.ParagraphProperties.SpacingBetweenLines.Line = (Convert.ToInt32(Convert.ToDouble(f.Value.Replace(",", "."))*240)).ToString();
                                }
                                else
                                {
                                    //VUELVO
                                    msgR.bot.SendMessage("El valor de interlineado \"" + f.Value + "\" no es válido, ¿Deseas cambiarlo por otro?", msgR.chatId);

                                    var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                    if(msgReceived.StartsWith("/"))
                                    {
                                        msgReceived = msgReceived.Substring(1);
                                    }

                                    if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                    {
                                        msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                        while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                        {
                                            msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                            if(msgReceived.StartsWith("/"))
                                            {
                                                msgReceived = msgReceived.Substring(1);
                                            }

                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                if(msgReceived.All(delegate(char c) { return char.IsDigit(c) || c == Convert.ToChar(",") || c == Convert.ToChar("."); }))
                                                {
                                                    paragraph.ParagraphProperties.SpacingBetweenLines.Line = (Convert.ToInt32(Convert.ToDouble(msgReceived.Replace(",", "."))*240)).ToString();
                                                }
                                                else
                                                {
                                                    msgR.bot.SendMessage("Ese tampoco lo es 😔, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                }
                                            }
                                        }
                                        if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                        {
                                            Word.Dispose();
                                            return;
                                        }
                                    }
                                }
                            }

                            var runs = paragraph.Descendants<Run>();
                            foreach( var run in runs)
                            {
                                if(run.RunProperties == null)
                                {
                                    run.RunProperties = new RunProperties();
                                }

                                if(f.Key.ToLower().Contains("color"))
                                {
                                    var translate = new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
                                    {
                                        {"dorado", "gold"},
                                        {"marron", "brown"},
                                        {"purpura", "purple"},
                                        {"violeta", "violet"},
                                        {"rojo", "red"},
                                        {"fucsia", "fuchsia"},
                                        {"rosa", "pink"},
                                        {"naranja", "orange"},
                                        {"amarillo", "yellow"},
                                        {"blanco", "white"},
                                        {"plateado", "silver"},
                                        {"gris", "gray"},
                                        {"negro", "black"},
                                        {"azul", "blue"},
                                        {"celeste", "skyblue"},
                                        {"cian", "cyan"},
                                        {"turquesa", "turquoise"},
                                        {"verde", "green"},
                                        {"lima", "lime"},
                                        {"oliva", "olive"}
                                    };

                                    string value;
                                    if(f.Value.StartsWith("#"))
                                    {
                                        value = f.Value.Substring(1);
                                    }
                                    else
                                    {
                                        value = f.Value;
                                    }

                                    if(Int32.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out int result))
                                    {
                                        run.RunProperties.Color = new Color() { Val = value };
                                    }
                                    else if(value == "black" || System.Drawing.Color.FromName(value).ToArgb() != 0)
                                    {
                                        run.RunProperties.Color = new Color() { Val = String.Format("{0:X6}", System.Drawing.Color.FromName(value).ToArgb()) };
                                    }
                                    else if(translate.Keys.Contains(value))
                                    {
                                        run.RunProperties.Color = new Color() { Val = String.Format("{0:X6}", System.Drawing.Color.FromName(translate[value]).ToString()) };
                                    }
                                    else
                                    {
                                        //VUELVO
                                        msgR.bot.SendMessage("No reconozco el color \"" + f.Value + "\", ¿Deseas cambiarlo por otro?", msgR.chatId);

                                        var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                        if(msgReceived.StartsWith("/"))
                                        {
                                            msgReceived = msgReceived.Substring(1);
                                        }

                                        if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                        {
                                            msgR.bot.SendMessage("Ingrese un nuevo color, o /atras para volver.", msgR.chatId);

                                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                if(msgReceived.StartsWith("/"))
                                                {
                                                    msgReceived = msgReceived.Substring(1);
                                                }

                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    if(msgReceived.StartsWith("#"))
                                                    {
                                                        msgReceived = msgReceived.Substring(1);
                                                    }

                                                    if(Int32.TryParse(msgReceived, System.Globalization.NumberStyles.HexNumber, null, out int r))
                                                    {
                                                        run.RunProperties.Color = new Color() { Val = msgReceived };
                                                    }
                                                    else if(msgReceived == "black" || System.Drawing.Color.FromName(msgReceived).ToArgb() != 0)
                                                    {
                                                        var color = System.Drawing.Color.FromName(msgReceived);
                                                        run.RunProperties.Color = new Color() { Val = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") };
                                                    }
                                                    else if(translate.Keys.Contains(msgReceived))
                                                    {
                                                        var color = System.Drawing.Color.FromName(translate[msgReceived]);
                                                        run.RunProperties.Color = new Color() { Val = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") };
                                                    }
                                                    else
                                                    {
                                                        msgR.bot.SendMessage("Tampoco reconozco ese 😣, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                    }
                                                }
                                            }
                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                            {
                                                Word.Dispose();
                                                return;
                                            }
                                        }
                                    }
                                }
                                else if(f.Key.ToLower().Contains("tama") || f.Key.ToLower().Contains("size"))
                                {
                                    if(f.Value.All(char.IsDigit))
                                    {
                                        run.RunProperties.FontSize = new FontSize() { Val = (Convert.ToInt32(f.Value)*2).ToString() };
                                    }
                                    else
                                    {
                                        //VUELVO
                                        msgR.bot.SendMessage("El tamaño \"" + f.Value + "\" no es un tamaño válido, ¿Deseas cambiarlo por otro?", msgR.chatId);
                                            
                                        var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                        if(msgReceived.StartsWith("/"))
                                        {
                                            msgReceived = msgReceived.Substring(1);
                                        }

                                        if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                        {
                                            msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                if(msgReceived.StartsWith("/"))
                                                {
                                                    msgReceived = msgReceived.Substring(1);
                                                }

                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    if(msgReceived.All(char.IsDigit))
                                                    {
                                                        run.RunProperties.FontSize = new FontSize() { Val = (Convert.ToInt32(msgReceived)*2).ToString() };
                                                    }
                                                    else
                                                    {
                                                        msgR.bot.SendMessage("Ese tampoco es un tamaño válido 🤨, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                    }
                                                }
                                            }
                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                            {
                                                Word.Dispose();
                                                return;
                                            }
                                        }
                                    }
                                }
                                else if(f.Key.ToLower().Contains("fuente") || f.Key.ToLower().Contains("font") || f.Key.ToLower().Contains("tipo") || f.Key.ToLower().Contains("type"))
                                {
                                    var fonts = new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
                                    {
                                        {"calibri", "Calibri (Body)"},
                                        {"arial", "Arial"},
                                        {"comicsansms", "Comic Sans MS"},
                                        {"comicsans", "Comic Sans MS"},
                                        {"sans", "Comic Sans MS"},
                                        {"timesnewroman", "Times New Roman"},
                                        {"timesroman", "Times New Roman"},
                                        {"algerian", "Algerian"},
                                        {"bahnschrift", "Bahnschrift"},
                                        {"baskerville", "Baskerville Old Face"},
                                        {"baskervilleold", "Baskerville Old Face"},
                                        {"baskervilleoldface", "Baskerville Old Face"},
                                        {"bernardmtcondensed", "Bernard MT Condensed"},
                                        {"bernardcondensed", "Bernard MT Condensed"},
                                        {"bernard", "Bernard MT Condensed"},
                                        {"bauhaus", "Bauhaus 93"},
                                        {"bauhaus93", "Bauhaus 93"},
                                        {"broadway", "Broadway"},
                                        {"bookantiqua", "Book Antiqua"},
                                        {"bradley", "Bradley Hand ITC"},
                                        {"bradleyhand", "Bradley Hand ITC"},
                                        {"bradleyhanditc", "Bradley Hand ITC"},
                                        {"cambria", "Cambria"},
                                        {"castellar", "Castellar"}
                                    };

                                    if(fonts.ContainsKey(f.Value.Replace(" ", "")))
                                    {
                                        run.RunProperties.RunFonts = new RunFonts() { Ascii = fonts[f.Value.Replace(" ", "")] };
                                    }
                                    else
                                    {
                                        //VUELVO
                                        msgR.bot.SendMessage("No reconozco tu fuente guardada \"" + f.Value + "\", ¿Deseas cambiarla por otra?", msgR.chatId);
                                            
                                        var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                        if(msgReceived.StartsWith("/"))
                                        {
                                            msgReceived = msgReceived.Substring(1);
                                        }

                                        if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                        {
                                            msgR.bot.SendMessage("Ingrese su nueva fuente, o /atras para volver.", msgR.chatId);

                                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                if(msgReceived.StartsWith("/"))
                                                {
                                                    msgReceived = msgReceived.Substring(1);
                                                }

                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    if(fonts.ContainsKey(msgReceived.Replace(" ", "")))
                                                    {
                                                        run.RunProperties.RunFonts = new RunFonts() { Ascii = fonts[msgReceived.Replace(" ", "")] };
                                                    }
                                                    else
                                                    {
                                                        msgR.bot.SendMessage("Tampoco reconozco esa 😟, prueba con otra o ingresa /atras para volver.", msgR.chatId);
                                                    }
                                                }
                                            }
                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                            {
                                                Word.Dispose();
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Text
                            for( int n = 1; n < pair.Value.Count; n++)
                            {
                                var paragraph = pair.Value[n];
                                if(paragraph.ParagraphProperties == null)
                                {
                                    paragraph.ParagraphProperties = new ParagraphProperties();
                                }

                                if(f.Key.ToLower().Contains("alinea") || f.Key.ToLower().Contains("align") || f.Key.ToLower().Contains("justifi"))
                                {
                                    if(f.Value.Contains("cent"))
                                    {
                                        paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
                                    }
                                    else if(f.Value.Contains("izq") || f.Value.Contains("left"))
                                    {
                                        paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Left };
                                    }
                                    else if(f.Value.Contains("der") || f.Value.Contains("right"))
                                    {
                                        paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                                    }
                                    else if(f.Value.Contains("just"))
                                    {
                                        paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Both };
                                    }
                                    else
                                    {
                                        //VUELVO
                                        msgR.bot.SendMessage("No reconozco el tipo de alineación \"" + f.Value + "\", ¿Deseas cambiarlo por otro?", msgR.chatId);

                                        var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                        if(msgReceived.StartsWith("/"))
                                        {
                                            msgReceived = msgReceived.Substring(1);
                                        }

                                        if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                        {
                                            msgR.bot.SendMessage("Ingrese un nuevo tipo, o /atras para volver.", msgR.chatId);

                                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                if(msgReceived.StartsWith("/"))
                                                {
                                                    msgReceived = msgReceived.Substring(1);
                                                }

                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    if(msgReceived.Contains("cent"))
                                                    {
                                                        paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
                                                    }
                                                    else if(msgReceived.Contains("izq") || msgReceived.Contains("left"))
                                                    {
                                                        paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Left };
                                                    }
                                                    else if(msgReceived.Contains("der") || msgReceived.Contains("right"))
                                                    {
                                                        paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Right };
                                                    }
                                                    else if(msgReceived.Contains("just"))
                                                    {
                                                        paragraph.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Both };
                                                    }
                                                    else
                                                    {
                                                        msgR.bot.SendMessage("Ese tampoco lo reconozco 😨, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                    }
                                                }
                                            }
                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                            {
                                                Word.Dispose();
                                                return;
                                            }
                                        }
                                    }
                                }
                                else if(f.Key.ToLower().Contains("spac"))
                                {
                                    if(paragraph.ParagraphProperties.SpacingBetweenLines == null)
                                    {
                                        paragraph.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines();
                                    }

                                    if( f.Key.ToLower().Contains("ant") || f.Key.ToLower().Contains("befor") )
                                    {
                                        if(f.Value.All(char.IsDigit))
                                        {
                                            paragraph.ParagraphProperties.SpacingBetweenLines.Before = (Convert.ToUInt32(f.Value)*20).ToString();
                                        }
                                        else
                                        {
                                            //VUELVO
                                            msgR.bot.SendMessage("El valor de espaciado anterior \"" + f.Value + "\" no es válido, ¿Deseas cambiarlo por otro?", msgR.chatId);

                                            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                            if(msgReceived.StartsWith("/"))
                                            {
                                                msgReceived = msgReceived.Substring(1);
                                            }

                                            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                            {
                                                msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                                while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                    if(msgReceived.StartsWith("/"))
                                                    {
                                                        msgReceived = msgReceived.Substring(1);
                                                    }
                                    
                                                    if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                    {
                                                        if(msgReceived.All(char.IsDigit))
                                                        {
                                                            paragraph.ParagraphProperties.SpacingBetweenLines.After = (Convert.ToUInt32(msgReceived)*20).ToString();
                                                        }
                                                        else
                                                        {
                                                            msgR.bot.SendMessage("Ese tampoco lo es 😥, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                        }
                                                    }
                                                }
                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                                {
                                                    Word.Dispose();
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else if( f.Key.ToLower().Contains("desp") || f.Key.ToLower().Contains("aft") )
                                    {
                                        if(f.Value.All(char.IsDigit))
                                        {
                                            paragraph.ParagraphProperties.SpacingBetweenLines.After = (Convert.ToUInt32(f.Value)*20).ToString();
                                        }
                                        else
                                        {
                                            //VUELVO
                                            msgR.bot.SendMessage("El valor de espaciado posterior \"" + f.Value + "\" no es válido, ¿Deseas cambiarlo por otro?", msgR.chatId);

                                            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                            if(msgReceived.StartsWith("/"))
                                            {
                                                msgReceived = msgReceived.Substring(1);
                                            }

                                            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                            {
                                                msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                                while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                    if(msgReceived.StartsWith("/"))
                                                    {
                                                        msgReceived = msgReceived.Substring(1);
                                                    }

                                                    if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                    {
                                                        if(msgReceived.All(char.IsDigit))
                                                        {
                                                            paragraph.ParagraphProperties.SpacingBetweenLines.After = (Convert.ToUInt32(msgReceived)*20).ToString();
                                                        }
                                                        else
                                                        {
                                                            msgR.bot.SendMessage("Ese tampoco lo es 😭, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                        }
                                                    }
                                                }
                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                                {
                                                    Word.Dispose();
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if(f.Value.All(delegate(char c) { return char.IsDigit(c) || c == Convert.ToChar(",") || c == Convert.ToChar("."); }))
                                        {
                                            paragraph.ParagraphProperties.SpacingBetweenLines.Line = (Convert.ToInt32(Convert.ToDouble(f.Value.Replace(",", "."))*240)).ToString();
                                        }
                                        else
                                        {
                                            //VUELVO
                                            msgR.bot.SendMessage("El valor de interlineado \"" + f.Value + "\" no es válido, ¿Deseas cambiarlo por otro?", msgR.chatId);

                                            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                            if(msgReceived.StartsWith("/"))
                                            {
                                                msgReceived = msgReceived.Substring(1);
                                            }

                                            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                            {
                                                msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                                while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                    if(msgReceived.StartsWith("/"))
                                                    {
                                                        msgReceived = msgReceived.Substring(1);
                                                    }

                                                    if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                    {
                                                        if(msgReceived.All(delegate(char c) { return char.IsDigit(c) || c == Convert.ToChar(",") || c == Convert.ToChar("."); }))
                                                        {
                                                            paragraph.ParagraphProperties.SpacingBetweenLines.Line = (Convert.ToInt32(Convert.ToDouble(msgReceived.Replace(",", "."))*240)).ToString();
                                                        }
                                                        else
                                                        {
                                                            msgR.bot.SendMessage("Ese tampoco lo es 🥺, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                        }
                                                    }
                                                }
                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                                {
                                                    Word.Dispose();
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                                if(f.Key.ToLower().Contains("interl"))
                                {
                                    if(paragraph.ParagraphProperties.SpacingBetweenLines == null)
                                    {
                                        paragraph.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines();
                                    }

                                    if(f.Value.All(delegate(char c) { return char.IsDigit(c) || c == Convert.ToChar(",") || c == Convert.ToChar("."); }))
                                    {
                                        paragraph.ParagraphProperties.SpacingBetweenLines.Line = (Convert.ToInt32(Convert.ToDouble(f.Value.Replace(",", "."))*240)).ToString();
                                    }
                                    else
                                    {
                                        //VUELVO
                                        msgR.bot.SendMessage("El valor de interlineado \"" + f.Value + "\" no es válido, ¿Deseas cambiarlo por otro?", msgR.chatId);

                                        var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                        if(msgReceived.StartsWith("/"))
                                        {
                                            msgReceived = msgReceived.Substring(1);
                                        }

                                        if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                        {
                                            msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                            {
                                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                if(msgReceived.StartsWith("/"))
                                                {
                                                    msgReceived = msgReceived.Substring(1);
                                                }

                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    if(msgReceived.All(delegate(char c) { return char.IsDigit(c) || c == Convert.ToChar(",") || c == Convert.ToChar("."); }))
                                                    {
                                                        paragraph.ParagraphProperties.SpacingBetweenLines.Line = (Convert.ToInt32(Convert.ToDouble(msgReceived.Replace(",", "."))*240)).ToString();
                                                    }
                                                    else
                                                    {
                                                        msgR.bot.SendMessage("Ese tampoco lo es 🥺, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                    }
                                                }
                                            }
                                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                            {
                                                Word.Dispose();
                                                return;
                                            }
                                        }
                                    }
                                }

                                var runs = paragraph.Descendants<Run>();
                                foreach( var run in runs)
                                {
                                    if(run.RunProperties == null)
                                    {
                                        run.RunProperties = new RunProperties();
                                    }

                                    if(f.Key.ToLower().Contains("color"))
                                    {
                                        var translate = new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
                                        {
                                            {"dorado", "gold"},
                                            {"marron", "brown"},
                                            {"purpura", "purple"},
                                            {"violeta", "violet"},
                                            {"rojo", "red"},
                                            {"fucsia", "fuchsia"},
                                            {"rosa", "pink"},
                                            {"naranja", "orange"},
                                            {"amarillo", "yellow"},
                                            {"blanco", "white"},
                                            {"plateado", "silver"},
                                            {"gris", "gray"},
                                            {"negro", "black"},
                                            {"azul", "blue"},
                                            {"celeste", "skyblue"},
                                            {"cian", "cyan"},
                                            {"turquesa", "turquoise"},
                                            {"verde", "green"},
                                            {"lima", "lime"},
                                            {"oliva", "olive"}
                                        };

                                        string value;
                                        if(f.Value.StartsWith("#"))
                                        {
                                            value = f.Value.Substring(1);
                                        }
                                        else
                                        {
                                            value = f.Value;
                                        }

                                        if(Int32.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out int result))
                                        {
                                            run.RunProperties.Color = new Color() { Val = value };
                                        }
                                        else if(value == "black" || System.Drawing.Color.FromName(value).ToArgb() != 0)
                                        {
                                            var color = System.Drawing.Color.FromName(value);
                                            run.RunProperties.Color = new Color() { Val = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") };
                                        }
                                        else if(translate.Keys.Contains(value))
                                        {
                                            var color = System.Drawing.Color.FromName(translate[value]);
                                            run.RunProperties.Color = new Color() { Val = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") };
                                        }
                                        else
                                        {
                                            //VUELVO
                                            msgR.bot.SendMessage("No reconozco el color \"" + f.Value + "\", ¿Deseas cambiarlo por otro?", msgR.chatId);

                                            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                            if(msgReceived.StartsWith("/"))
                                            {
                                                msgReceived = msgReceived.Substring(1);
                                            }

                                            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                            {
                                                msgR.bot.SendMessage("Ingrese un nuevo color, o /atras para volver.", msgR.chatId);

                                                while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                    if(msgReceived.StartsWith("/"))
                                                    {
                                                        msgReceived = msgReceived.Substring(1);
                                                    }

                                                    if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                    {
                                                        if(msgReceived.StartsWith("#"))
                                                        {
                                                            msgReceived = msgReceived.Substring(1);
                                                        }

                                                        if(Int32.TryParse(msgReceived, System.Globalization.NumberStyles.HexNumber, null, out int r))
                                                        {
                                                            run.RunProperties.Color = new Color() { Val = msgReceived };
                                                        }
                                                        else if(msgReceived == "black" || System.Drawing.Color.FromName(msgReceived).ToArgb() != 0)
                                                        {
                                                            var color = System.Drawing.Color.FromName(msgReceived);
                                                            run.RunProperties.Color = new Color() { Val = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") };
                                                        }
                                                        else if(translate.Keys.Contains(msgReceived))
                                                        {
                                                            var color = System.Drawing.Color.FromName(translate[msgReceived]);
                                                            run.RunProperties.Color = new Color() { Val = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") };
                                                        }
                                                        else
                                                        {
                                                            msgR.bot.SendMessage("Tampoco reconozco ese 😓, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                        }
                                                    }
                                                }
                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                                {
                                                    Word.Dispose();
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else if(f.Key.ToLower().Contains("tama") || f.Key.ToLower().Contains("size"))
                                    {
                                        if(f.Value.All(char.IsDigit))
                                        {
                                            run.RunProperties.FontSize = new FontSize() { Val = (Convert.ToInt32(f.Value)*2).ToString() };
                                        }
                                        else
                                        {
                                            //VUELVO
                                            msgR.bot.SendMessage("El tamaño \"" + f.Value + "\" no es un tamaño válido, ¿Deseas cambiarlo por otro?", msgR.chatId);
                                            
                                            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                            if(msgReceived.StartsWith("/"))
                                            {
                                                msgReceived = msgReceived.Substring(1);
                                            }

                                            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                            {
                                                msgR.bot.SendMessage("Ingrese un nuevo número, o /atras para volver.", msgR.chatId);

                                                while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                    if(msgReceived.StartsWith("/"))
                                                    {
                                                        msgReceived = msgReceived.Substring(1);
                                                    }

                                                    if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                    {
                                                        if(msgReceived.All(char.IsDigit))
                                                        {
                                                            run.RunProperties.FontSize = new FontSize() { Val = (Convert.ToInt32(msgReceived)*2).ToString() };
                                                        }
                                                        else
                                                        {
                                                            msgR.bot.SendMessage("Ese tampoco es un tamaño válido 😔, prueba con otro o ingresa /atras para volver.", msgR.chatId);
                                                        }
                                                    }
                                                }
                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                                {
                                                    Word.Dispose();
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else if(f.Key.ToLower().Contains("fuente") || f.Key.ToLower().Contains("font") || f.Key.ToLower().Contains("tipo") || f.Key.ToLower().Contains("type"))
                                    {
                                        var fonts = new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
                                        {
                                            {"calibri", "Calibri (Body)"},
                                            {"arial", "Arial"},
                                            {"comicsansms", "Comic Sans MS"},
                                            {"comicsans", "Comic Sans MS"},
                                            {"sans", "Comic Sans MS"},
                                            {"timesnewroman", "Times New Roman"},
                                            {"timesroman", "Times New Roman"},
                                            {"algerian", "Algerian"},
                                            {"bahnschrift", "Bahnschrift"},
                                            {"baskerville", "Baskerville Old Face"},
                                            {"baskervilleold", "Baskerville Old Face"},
                                            {"baskervilleoldface", "Baskerville Old Face"},
                                            {"bernardmtcondensed", "Bernard MT Condensed"},
                                            {"bernardcondensed", "Bernard MT Condensed"},
                                            {"bernard", "Bernard MT Condensed"},
                                            {"bauhaus", "Bauhaus 93"},
                                            {"bauhaus93", "Bauhaus 93"},
                                            {"broadway", "Broadway"},
                                            {"bookantiqua", "Book Antiqua"},
                                            {"bradley", "Bradley Hand ITC"},
                                            {"bradleyhand", "Bradley Hand ITC"},
                                            {"bradleyhanditc", "Bradley Hand ITC"},
                                            {"cambria", "Cambria"},
                                            {"castellar", "Castellar"}
                                        };

                                        if(fonts.ContainsKey(f.Value.Replace(" ", "")))
                                        {
                                            run.RunProperties.RunFonts = new RunFonts() { Ascii = fonts[f.Value.Replace(" ", "")] };
                                        }
                                        else
                                        {
                                            //VUELVO
                                            msgR.bot.SendMessage("No reconozco tu fuente guardada \"" + f.Value + "\", ¿Deseas cambiarla por otra?", msgR.chatId);
                                            
                                            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                            if(msgReceived.StartsWith("/"))
                                            {
                                                msgReceived = msgReceived.Substring(1);
                                            }

                                            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                                            {
                                                msgR.bot.SendMessage("Ingrese su nueva fuente, o /atras para volver.", msgR.chatId);

                                                while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                {
                                                    msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                                    if(msgReceived.StartsWith("/"))
                                                    {
                                                        msgReceived = msgReceived.Substring(1);
                                                    }

                                                    if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                                    {
                                                        if(fonts.ContainsKey(msgReceived.Replace(" ", "")))
                                                        {
                                                            run.RunProperties.RunFonts = new RunFonts() { Ascii = fonts[msgReceived.Replace(" ", "")] };
                                                        }
                                                        else
                                                        {
                                                            msgR.bot.SendMessage("Tampoco reconozco esa 😰, prueba con otra o ingresa /atras para volver.", msgR.chatId);
                                                        }
                                                    }
                                                }
                                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                                {
                                                    Word.Dispose();
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                                var date = DateTime.Today;
                                foreach(var weekday in new DayOfWeek[7]{ DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday })
                                {
                                    if(date.DayOfWeek != weekday)
                                    {
                                        int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(date.DayOfWeek)) % 7;
                                        date = date.AddDays(daysTo);
                                    }
                                    if(paragraph.InnerText == date.ToString("dddd dd/MM", new CultureInfo("es-ES")).ToUpper()[0] + date.ToString("dddd dd/MM", new CultureInfo("es-ES")).Substring(1))
                                    {
                                        if(Word.MainDocumentPart.NumberingDefinitionsPart == null)
                                        {
                                            Word.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>();
                                            Word.MainDocumentPart.NumberingDefinitionsPart.Numbering = new Numbering();
                                        }
                                        var numInst = new NumberingInstance() { NumberID = Word.MainDocumentPart.NumberingDefinitionsPart.Numbering.Elements<NumberingInstance>().Count() + 1 };
                                        var absNum = new AbstractNum(new Level( new NumberingFormat() { Val = NumberFormatValues.Bullet } ));
                                        absNum.AbstractNumberId = Word.MainDocumentPart.NumberingDefinitionsPart.Numbering.Elements<NumberingInstance>().Count() + 1;
                                        numInst.AbstractNumId = new AbstractNumId() {Val = Word.MainDocumentPart.NumberingDefinitionsPart.Numbering.Elements<NumberingInstance>().Count() + 1};

                                        Word.MainDocumentPart.NumberingDefinitionsPart.Numbering.Append(numInst);
                                        Word.MainDocumentPart.NumberingDefinitionsPart.Numbering.Append(absNum);

                                        foreach(var planPar in paragraphs.GetRange(paragraphs.FindIndex(delegate(Paragraph p) { return p.InnerText == paragraph.InnerText; })+1, msgR.userData.weeklyPlan.ShowDayPlans(date).Count))
                                        {
                                            if(planPar.ParagraphProperties == null)
                                            {
                                                planPar.ParagraphProperties = new ParagraphProperties();
                                            }
                                            var planRuns = planPar.ParagraphProperties.Descendants<Run>();
                                            if(planPar.ParagraphProperties.NumberingProperties == null)
                                            {
                                                planPar.ParagraphProperties.NumberingProperties = new NumberingProperties();

                                            }
                                            planPar.ParagraphProperties.NumberingProperties.NumberingId = new NumberingId() { Val = 1 };
                                            planPar.ParagraphProperties.NumberingProperties.NumberingLevelReference = new NumberingLevelReference() { Val = 0 };
                                            planPar.ParagraphProperties.Indentation = new Indentation() { Left = "720", Hanging = "360" };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(indexes[indexes.Count-1]+1 < paragraphs.Count)
            {
                var finalParagraph = paragraphs[indexes[indexes.Count-1]];
                if(finalParagraph.ParagraphProperties == null)
                {
                    finalParagraph.ParagraphProperties = new ParagraphProperties();
                }
                finalParagraph.ParagraphProperties.PageBreakBefore = new PageBreakBefore();
            }

            Word.Dispose();
            msgR.bot.SendDocument(@"..\Userdata\" + msgR.chatId + ".docx", msgR.chatId, "Aquí va tu bitácora 😁");
        }

        private void metacogRefCreate(MessageResponse msgR)
        {
            doc.AddContent(new TextContent(msgR.userData.metacogRef.Text), DocumentPosition.TOP);
            doc.AddContent(new TextContent(msgR.userData.metacogRef.Title), DocumentPosition.TOP);
        }

        private void weeklyRefCreate(MessageResponse msgR)
        {
            doc.AddContent(new TextContent(msgR.userData.weeklyRef.Text), DocumentPosition.TOP);
            doc.AddContent(new TextContent(msgR.userData.weeklyRef.Title), DocumentPosition.TOP);
        }

        private void weeklyObjCreate(MessageResponse msgR)
        {
            bool empty = true;
            foreach(var obj in msgR.userData.weeklyObj.ShowObjectives())
            {
                doc.AddContent(new TextContent( "- " + obj.Goal ), DocumentPosition.TOP);
                empty = false;
            }
            if(empty)
            {
                doc.AddContent(new TextContent(""), DocumentPosition.TOP);
            }
            doc.AddContent(new TextContent(msgR.userData.weeklyObj.Title), DocumentPosition.TOP);
        }

        private void weeklyPlanCreate(MessageResponse msgR)
        {
            var date = DateTime.Today;
            foreach(var weekday in new DayOfWeek[7]{ DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday })
            {
                if(date.DayOfWeek != weekday)
                {
                    int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(date.DayOfWeek)) % 7;
                    date = date.AddDays(daysTo);
                }
                var planList = msgR.userData.weeklyPlan.ShowDayPlans(date);
                planList.Reverse();
                foreach(var plan in planList)
                {
                    string text = null;
                    if(plan.ActivityTime.Second != 0)
                    {
                        text += plan.ActivityTime.ToString("hh:mm", new CultureInfo("es-ES")) + ", ";
                    }
                    doc.AddContent(new TextContent( text + plan.Goal ), DocumentPosition.TOP);
                }
                doc.AddContent(new TextContent(date.ToString("dddd dd/MM", new CultureInfo("es-ES")).ToUpper()[0] + date.ToString("dddd dd/MM", new CultureInfo("es-ES")).Substring(1)), DocumentPosition.TOP);
            }
            doc.AddContent(new TextContent(msgR.userData.weeklyPlan.Title), DocumentPosition.TOP);
        }
    }
}

