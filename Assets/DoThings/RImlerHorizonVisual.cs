using UnityEngine;

public class RImlerHorizonVisual : MonoBehaviour
{
    [SerializeField] Frame frame;

    // Update is called once per frame
    void Update()
    {
        if (frame.isInterial)
        {
            Vector3 rimler = frame.rimler;

            transform.position = rimler;
            transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(rimler.y,rimler.x)*Mathf.Rad2Deg);
        }
    }
}
