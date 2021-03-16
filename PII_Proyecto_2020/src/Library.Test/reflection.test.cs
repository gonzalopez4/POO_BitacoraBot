using NUnit.Framework;

namespace Library
{
    public class TestWeeklyReflection
    {
        //Se testea la Reflection creada 
        [Test]
        public void ReflectionCreada () 
        {

            string texto = "Reflectionsita";
            //Se crea una instancia de la clase Reflection 
            Reflection reflection = new Reflection ();
            reflection.Text = texto;
            //se comprueba que guarde la Reflection
            Assert.AreEqual ("Reflectionsita", reflection.Text);
        }

        [Test]
        public void ReflectionModificada () 
        {

            string texto = "Reflectionsita";
            //Se crea una instancia de la clase Reflection 
            Reflection reflection = new Reflection ();
            reflection.Text = texto;
            //Se guarda la primera Reflection 
            string reflectionPast = reflection.Text;
            //se rescrive la Reflection
            reflection.Text = "asdasda";
            //se compara que no sean iguales
            Assert.AreNotEqual (reflectionPast, reflection.Text);
        }

        [Test]
        //se testea que se elimine el objetivo
        public void ReflectionBorrada () 
        {

            string texto = "Reflectionsita";
            //Se crea una instancia de la clase Reflection 
            Reflection reflection = new Reflection ();
            reflection.Text = texto;
            //se borra la reflection
            reflection.Text = null;
            //se comprueba que este vacio la Reflection
            Assert.IsNull (reflection.Text);
        }

        [Test]
        //se testea que se cree el titulo
        public void tituloCreado () 
        {

            string titulo = "titulito";
            //Se crea una instancia de la clase Reflection 
            Reflection reflection = new Reflection ();
            reflection.Title = titulo;
            //Se comprueba que este creado
            Assert.AreEqual ("titulito", reflection.Title);
        }

        [Test]
        //Se testea que se modifique el titulo 
        public void tituloModificado () 
        {

            string titulo = "titulito";
            Reflection reflection = new Reflection ();
            reflection.Title = titulo;
            //Se crea una instancia de la clase Reflection 
            string tituloPast = reflection.Title;
            //Se rescrive el titulo
            reflection.Title = "asdasda";
            //Se compara que no sean iguales
            Assert.AreNotEqual ("titulito", reflection.Title);
        }

        [Test]
        //se testea que se elimine el titulo
        public void tituloEliminado () 
        {

            string titulo = "titulito";
           //se testea que se elimine el titulo
            Reflection reflection = new Reflection ();
            reflection.Title = titulo;
            //Se borra  el titulo
            reflection.Title = null;
            //se comprueba que este vacio el titulo
            Assert.IsNull (reflection.Title);
        }

    }
}
