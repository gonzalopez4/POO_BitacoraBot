using NUnit.Framework;

namespace Library
{
    public class Tests
    {
        [Test]
        //Se testea el objetivo que se crea en el programa  
        public void objetivoCreado () 
        {
            string goal = "macciioli";
            //se crea una instancia de la clase objetivo 
            Objective objetive = new Objective (goal);
            //se comprueba que guarde el objetivo
            Assert.AreEqual ("macciioli", objetive.Goal);
        }

        [Test]
        //Se testea que se modifique el objetivo 
        public void objetivoModificado () 
        {

            string goal = "macciioli";
            //Se crea una instancia de la clase objetivo 
            Objective objetive = new Objective (goal);
            //Se guarda el primer objetivo 
            string goalPast = objetive.Goal;
            //se rescrive el objetivo
            objetive.Goal = "jhvgjhvbj";
            //se compara que no sean iguales
            Assert.AreNotEqual (goalPast, objetive.Goal);
        }

        [Test]
        //se testea que se elimine el objetivo
        public void objetivoEliminado () // Cambiá el nombre para indicar qué estás probando
        {   
        
            string goal = "macciioli"; 
            //Se crea una instancia de la clase objetivo 
            Objective objetive = new Objective (goal);
            //Se borra  el objetivo
            objetive.Goal = null;
            //se comprueba que este vacio el objetivo
            Assert.IsNull (objetive.Goal);
        }

    }

}
