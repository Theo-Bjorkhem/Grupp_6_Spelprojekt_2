using UnityEngine;

public class Player : Entity
{
    [Header("Objects")]
    [Tooltip("The Player GameObject")]
    [SerializeField] private GameObject myPlayer;
    [Header("Settings")]
    [Tooltip("The x-axis size of the array the player can move on.")]
    [SerializeField] private int myXSize;
    [Tooltip("The y-axis size of the array the player can move on.")]
    [SerializeField] private int myZSize;

    private int[,] myPositionArray;
    private int myXPos;
    private int myZPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        myXPos = 0;
        myZPos = 0;
        myPositionArray = new int[myXSize, myZSize];
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        TouchDetection();
    }

    void PlayerMovement()
    {
        myPlayer.transform.position = new Vector3(myXPos, 0, myZPos);

        if (Input.GetKeyDown("up"))
        {
            if (myXPos < myXSize)
            {
                myXPos++;
            }
        }
        if (Input.GetKeyDown("down"))
        {
            if (myXPos > 0)
            {
                myXPos--;
            }
        }
        if (Input.GetKeyDown("right"))
        {
            if (myZPos > 0)
            {
                myZPos--;

            }
        }
        if (Input.GetKeyDown("left"))
        {
            if (myZPos < myZSize)
            {
                myZPos++;
            }
        }
    }

    void TouchDetection()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //Converts ScrrenSpace to GameSpace.
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if (touchPosition.x > 0)
            {

            }

        }
    }
}
