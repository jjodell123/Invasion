using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selecter : MonoBehaviour
{
    public GameObject arrowReg, bomb, arrowFreeze;
    public Color normalColor;
    public Color highlightColor;

    Image arrowRegImg, bombImg, arrowFreezeImg;

    int scrollPoint = 0;

    private ShootProjectile shooter;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        arrowRegImg = arrowReg.GetComponent<Image>();
        bombImg = bomb.GetComponent<Image>();
        arrowFreezeImg = arrowFreeze.GetComponent<Image>();

        arrowRegImg.color = highlightColor;

        // Assumes one player.
        shooter = FindObjectOfType<ShootProjectile>();
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player isn't drawing the bow.
        if (playerController.CanChangeArrow())
        {
            // Checks key input.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                scrollPoint = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                scrollPoint = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                scrollPoint = 2;
            }

            // Checks scroll input.
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                scrollPoint--;
            }
            else if (scroll < 0f)
            {
                scrollPoint++;
            }

            // Keeps between 0 and 2
            if (scrollPoint > 2)
                scrollPoint = 0;
            else if (scrollPoint < 0)
                scrollPoint = 2;

            // Sets selected
            if (scrollPoint == 0)
            {
                arrowRegImg.color = highlightColor;
                bombImg.color = normalColor;
                arrowFreezeImg.color = normalColor;
            }
            else if (scrollPoint == 1)
            {
                arrowRegImg.color = normalColor;
                bombImg.color = highlightColor;
                arrowFreezeImg.color = normalColor;
            }
            else
            {
                arrowRegImg.color = normalColor;
                bombImg.color = normalColor;
                arrowFreezeImg.color = highlightColor;
            }

            shooter.SetProjectile(scrollPoint);
        }
    }
}
