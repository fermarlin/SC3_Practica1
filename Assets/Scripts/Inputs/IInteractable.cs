
/*Esta interfaz permite que distintos objetos del juego puedan ser interactuables
Asi puedo utilizar el mismo sistema de interaccion sin importar con que objeto estoy interactuando*/
public interface IInteractable
{
    //Obligo a los objetos que implementen esta interfaz a tener una funcion para interactuar, paso deltaTime para poder realizar interacciones continuas, como reparar el generador
    void Interact(float deltaTime);
}
