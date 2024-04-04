using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class UIRadialMenu : MonoBehaviour
    {
        [SerializeField] private int nbObject;
        [SerializeField] private GameObject defaultbackground;
        [SerializeField] private int startAngle = 0;
        [SerializeField] private int spaceFromCenter = 10;

        public int selectPart;

        public RectTransform centerIndicator;


        public bool isDisplay;

        private float angle;
        private Vector2 centerOfScreen;
        private List<GameObject> objList = new List<GameObject>();
        private int oldNbObject = 0;


        public void DisplayInventory()
        {
            gameObject.SetActive(true);

            if (oldNbObject != nbObject)
            {
                for (int i = 0; i < objList.Count; i++)
                {
                    Destroy(objList[i]);
                }

                objList.Clear();

                angle = 360 / nbObject;
                for (int i = 0; i < nbObject; i++)
                {

                    var wheel = Instantiate(defaultbackground, new Vector3(0, 0, 0), Quaternion.identity);
                    objList.Add(wheel);
                    wheel.transform.SetParent(transform);
                    wheel.SetActive(true);
                    wheel.transform.localScale = new Vector3(1, 1, 1);
                    wheel.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        spaceFromCenter * Mathf.Cos((startAngle + angle * i) * Mathf.Deg2Rad),
                        spaceFromCenter * Mathf.Sin((startAngle + angle * i) * Mathf.Deg2Rad));
                }
                oldNbObject = nbObject;
            }

            gameObject.SetActive(true);
            isDisplay = true;
        }

        public void HideInventory()
        {
            /*for (int i = 0; i < objList.Count; i++)
            {
                objList[i].SetActive(false);
            }*/
            isDisplay = false;
            gameObject.SetActive(false);
        }




    }
}
