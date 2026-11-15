
/*Esta interfaz permite que distintos objetos del juego puedan recibir dano
Asi puedo utilizar el mismo sistema de dano para jugadores, enemigos, generadores y barriles*/
public interface IDamageable
{
    //Obligo a los objetos que implementen esta interfaz a tener una funcion para recibir dano
    void TakeDamage(int damage);
}
