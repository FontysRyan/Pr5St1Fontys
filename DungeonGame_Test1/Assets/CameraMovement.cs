using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject Target;

    public int cam_mode, CAM, zPosition, yPosition;
    public GameObject sky;
    public Sprite day;
    public Sprite night;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {

        //CAM = PlayerPrefs.GetInt("cam");

        Target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // CAM = PlayerPrefs.GetInt("cam");

        if (cam_mode != CAM)
        {
            cam_mode = CAM;
            Movecam();
        }
        Movecam();


    }
    private void Movecam()
    {
        //sky.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        /*
         CAM1: NO FROZEN CAMERA
         CAM2: HORIZONTAL CAMERA
         CAM3: VERTICAL CAMERA
         CAM4: FREE MOVING CAMERA*/
        switch (cam_mode)
        {
            case 1:

                break;
            case 2:
                transform.position = new Vector3(Target.transform.position.x, transform.position.y, zPosition);
                break;
            case 3:
                transform.position = new Vector3(transform.position.x, Target.transform.position.y, zPosition);
                break;
            case 4:
                transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, zPosition);
                break;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
