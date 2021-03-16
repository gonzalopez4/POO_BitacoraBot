using NUnit.Framework;

namespace Library
{
    public class TestWeeklyPlanning
    {
        //se testea que se cree el titulo
        [Test]
        public void tituloCreado ()
        {

            string titulo = "titulito";
            //Se crea una instancia de la clase WeeklyPlanning 
            WeeklyPlanning WeeklyPlanning = new WeeklyPlanning ();
            WeeklyPlanning.Title = titulo;
            //Se comprueba que este creado
            Assert.AreEqual ("titulito", WeeklyPlanning.Title);
        }

        [Test]
        //Se testea que se modifique el titulo 
        public void tituloModificado ()
        {

            string titulo = "titulito";
            //Se crea una instancia de la clase WeeklyPlanning
            WeeklyPlanning WeeklyPlanning = new WeeklyPlanning ();
            WeeklyPlanning.Title = titulo;
            string reflectionPast = WeeklyPlanning.Title;
            //Se rescrive el titulo
            WeeklyPlanning.Title = "asdasda";
            //Se compara que no sean iguales
            Assert.AreNotEqual ("titulito", WeeklyPlanning.Title);
        }
        //se testea que se elimine el titulo
        [Test]
        public void tituloEliminado ()
        {

            string titulo = "titulito";
            //se testea que se elimine el titulo
            WeeklyPlanning WeeklyPlanning = new WeeklyPlanning ();
            WeeklyPlanning.Title = titulo;
            //Se borra  el titulo
            WeeklyPlanning.Title = null;
            //se comprueba que este vacio el titulo
            Assert.IsNull (WeeklyPlanning.Title);
        }

    }
}
