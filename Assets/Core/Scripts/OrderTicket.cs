using UnityEngine;

namespace Core.Scripts
{
    /// <summary>
    /// Ticket de commande spawné sur le tableau : garde en mémoire le plat commandé.
    /// </summary>
    public class OrderTicket : MonoBehaviour
    {
        public Plat Plat { get; private set; }

        public void Init(Plat plat)
        {
            Plat = plat;
        }
    }
}
