
using Unity.Behavior;

/*Este enum define los diferentes objetivos que puede tener un enemigo, 
lo marco como BlackboardEnum para poder utilizarlo dentro del Behavior Graph*/
[BlackboardEnum]
public enum EnemyObjective
{
    Player,
    Generator,
    Computer
}
