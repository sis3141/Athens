using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class RealControl : MonoBehaviour
{
    //플레이어 이동속도
    public float moveSpeed = 2f;
    //플레이어 회전속도
    public float rotationSpeed = 360f;

    CharacterController characterController;

    float speed = 20.0f;

    float rotSpeed = 1.0f;

    public Gamemanger manager;

    public IMIM imanager;

    public Menu menu;


    
    //"캐릭터 컨트롤러 컴포넌트를 사용하겠다"
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        manager.NonAction();
        imanager.NonAction();
        menu.NonAction();

    }

    void Update()
    {
        //움직이자!                                       좌우                            상하
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //부드러운 움직임
        if (direction.sqrMagnitude > 0.01f)
        {

            Vector3 forward = Vector3.Slerp(
                transform.forward,
                direction,
                rotationSpeed * Time.deltaTime / Vector3.Angle(transform.forward, direction));

            transform.LookAt(transform.position + forward);

        }
        //충돌처리
        characterController.Move(direction * moveSpeed * Time.deltaTime);

        click();

        //인벤토리
        if (Input.GetKey(KeyCode.I))
        {
            imanager.Action();
           
        }
        else if (Input.GetKey(KeyCode.O))
        {
            imanager.NonAction();
        }

        //메뉴화면
        if (Input.GetKey(KeyCode.Escape))
        {
            menu.Action();

        }
        else if (Input.GetKey(KeyCode.F1))
        {
            menu.NonAction();
        }


        float MouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * rotSpeed * MouseX);
    }
    public void click()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //이거할때 카메라 무조건 메인카메라 설정해줘야함

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform != null)
                {
                    manager.Action(hit.transform.gameObject);

                }
            }
        }
    }
   

}
