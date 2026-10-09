using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public StepsSounds stepsoundsScript;
    public NavMeshAgent agent;
    public GameObject player;
    public GameObject cam;
    public float playerSpeed = 2.5f;
    public float sensitivity = 200f;
    private float xRotation = 0f; //контроль вертикального угла

    public Animator weaponAnimator;
    public GameObject shootSound;
    public Light flashLight;
    public float shootDelay = 0.3f;

    public Text ammoText;
    public GameObject reloadSound;
    public float reloadDelay = 4.5f;

    public float weaponRange = 100f; //дальность полета пули

    public GameObject zombieHitLightPrefab;

    private int currentAmmo = 7;
    private int maxAmmo = 7;
    private bool isShooting = false;
    private bool isReloading = false;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateAmmoUI();
    }
    private void Update()
    {
        RotateCam();
        MovePlayer();
        bool isMovingNow = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        if (isMovingNow && stepsoundsScript != null)
        {
            stepsoundsScript.ProcessSteps(player.transform.position);
        }
        else if (!isMovingNow && stepsoundsScript != null)
        {
            stepsoundsScript.ResetTimer(); //сбрасываем таймер, если стоим на месте
        }

        if (isReloading) return;
        if (Input.GetMouseButtonDown(0) && !isShooting && !isReloading)
        {
            if (currentAmmo > 0)
            {
                StartCoroutine(PerformShoot());
            }
            else
            {
                //если патронов 0 и мы жмем ЛКМ -> автоматически запускается перезарядка
                StartCoroutine(PerformReload());
            }
        }

        //не перезаряжаемся, если магазин и так полон (currentAmmo == maxAmmo) или если сейчас идет выстрел
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isShooting)
        {
            StartCoroutine(PerformReload());
        }
    }

    private IEnumerator PerformShoot()
    {
        isShooting = true;
        currentAmmo--; //тратим один патрон
        UpdateAmmoUI(); //обновляем интерфейс

        RaycastHit hit;

        //пускаем луч: откуда (позиция камеры), куда (вперед камеры), инфа о попадании, дальность
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weaponRange))
        {
            if (hit.collider.CompareTag("Zombie"))
            {
                Debug.Log("Попало в зомбаря!");

                //ищем скрипт здоровья на зомби (включая его родительские объекты)
                ZombieHealth zombie = hit.collider.GetComponentInParent<ZombieHealth>();
                if (zombie != null)
                {
                    zombie.TakeDamage(1); //наносим 1 единицу урона
                }

                if (zombieHitLightPrefab != null)
                {
                    GameObject lightClone = Instantiate(zombieHitLightPrefab, hit.point, Quaternion.identity);
                    Destroy(lightClone, 0.35f); //ровно через n сек движок удалит объект со сцены
                }
            }
        }

        //1) включаем анимацию выстрела
        //(перематываем анимацию на самый 0-й кадр и принудительно запускаем)
        if (weaponAnimator != null)
        {
            //0 -> базовый слой аниматора, 0f -> начало анимации (0% прогресса)
            weaponAnimator.Play("ShotWeapon", 0, 0f);
        }

        //2) звук выстрела
        if (shootSound != null)
        {
            GameObject soundClone = Instantiate(shootSound, cam.transform.position, Quaternion.identity);
            AudioSource audio = soundClone.GetComponent<AudioSource>();
            if (audio != null) audio.Play();
            Destroy(soundClone, 3f);
        }

        //3) вспышка света
        if (flashLight != null)
        {
            flashLight.enabled = true;
        }

        //4) жесткое подергивание камеры
        float kickUp = Random.Range(3f, 5f);    //сила отдачи вверх
        float kickSide = Random.Range(-2f, 2f); //отдача влево/вправо
        xRotation -= kickUp;
        player.transform.Rotate(Vector3.up * kickSide);

        //ждем крошечный миг (0.05 секунды), чтобы вспышка света исчезла
        yield return new WaitForSeconds(0.05f);
        if (flashLight != null)
        {
            flashLight.enabled = false;
        }

        //ждем оставшееся время до конца анимации, чтобы игрок не мог спамить кликами!!!
        yield return new WaitForSeconds(shootDelay - 0.05f);

        isShooting = false;

        //авто-перезарядка сразу после завершения анимации выстрела, если патрон был последним
        if (currentAmmo == 0)
        {
            StartCoroutine(PerformReload());
        }
    }

    private IEnumerator PerformReload()
    {
        isReloading = true; //блокируем стрельбу и повторную перезарядку

        //умный запуск анимации перезарядки с принудительным сбросом в 0 кадр
        if (weaponAnimator != null)
        {
            weaponAnimator.Play("ReloadWeapon", 0, 0f);
        }

        if (reloadSound != null)
        {
            GameObject reloadSoundClone = Instantiate(reloadSound, cam.transform.position, Quaternion.identity);
            AudioSource audio = reloadSoundClone.GetComponent<AudioSource>();
            if (audio != null) audio.Play();
            Destroy(reloadSoundClone, reloadDelay);
        }

        yield return new WaitForSeconds(reloadDelay);

        currentAmmo = maxAmmo; //наполняем магазин патронами
        UpdateAmmoUI(); //обновляем интерфейс

        isReloading = false; //снимаем блокировку оружия
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo.ToString() + " / ∞";
        }
    }
    private void MovePlayer()
    {
        Vector3 direction = Vector3.zero;

        //сбор ввода по WASD относительно направления камеры
        if (Input.GetKey(KeyCode.W)) direction += cam.transform.forward;
        if (Input.GetKey(KeyCode.S)) direction -= cam.transform.forward;
        if (Input.GetKey(KeyCode.A)) direction -= cam.transform.right;
        if (Input.GetKey(KeyCode.D)) direction += cam.transform.right;

        //ОБНУЛЯЕМ Y ДО НОРМАЛИЗАЦИИ: исключаем влияние наклона камеры на скорость
        direction.y = 0;

        //НОРМАЛИЗУЕМ ОДИН РАЗ В КОНЦЕ: выравниваем скорость по диагоналям до честной единицы
        direction = direction.normalized;

        //двигаем капсулу по NavMesh сетке
        agent.Move(direction * playerSpeed * Time.deltaTime);
    }
    private void RotateCam()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //вращаем саму капсулу влево-вправо
        player.GetComponent<Transform>().Rotate(Vector3.up * mouseX);

        //расчет и ограничение угла вверх-вниз для камеры
        xRotation = xRotation - mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        //вращаем камеру
        cam.GetComponent<Transform>().localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
