using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player1;
    public Enemy ufo;

    void Start()
    {
        //Aqui le meto el el vector 3 zero porque la posicion la sabe cada objeto. En su propio metodo start se setea la posicion
        Player Player1 = Instantiate(player1,Vector3.zero, Quaternion.identity);
        Enemy ufo1 = Instantiate(ufo,Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
