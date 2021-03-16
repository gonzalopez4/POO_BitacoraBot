namespace Library
{
    /// <summary>
    /// Objective: Clase encargada de crear y modificar objetivos, marcando como cumplidos los que son logrados.
    /// 
    /// Principios y Patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, los objetivos.
    /// OCP: Cumple el principio al poder extender cualquier tipo de Objectives(Weekly, Monthly, Annual) sin verse afectado.
    /// Expert: Cumple el patron al ser experto en la informacion que utiliza.
    /// </summary>
    public class Objective
    {
        public Objective(string goal)
        {
            this.Goal = goal;
        }

        //Goal: String que representa el objetivo.
        public string Goal {get; set;}

        //Done: Bool que representa si el objetivo está cumplido.
        private bool Done {get; set;} = false;

        //Ended: Método encargado de marcar un objetivo como realizado en caso que se haya cumplido.
        public void Ended()
        {
            this.Done = true;
        }
        
        public void NotEnded()
        {
            this.Done = false;
        }
    }
}
