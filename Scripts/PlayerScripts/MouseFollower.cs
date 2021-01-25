using UnityEngine;
using System.Collections;

namespace Light2D.Examples
{
    public class MouseFollower : MonoBehaviour
    {

        private void Update()
        {
            facemouse();
        }
        

        void facemouse()
        {
            Vector2 mousePosition = Input.mousePosition;
            //mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

            transform.up = direction;
        }

    }
}
