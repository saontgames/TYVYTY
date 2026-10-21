
using UnityEngine;

public class CemeteryEconomy : MonoBehaviour
{
    public static CemeteryEconomy Instance { get; private set; }

    [SerializeField]
    private int startingMoney = 0;

    public int Money { get; private set; }

    private string lastMessage = "";
    private float messageUntil;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Money = startingMoney;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        Money += amount;

        lastMessage = "Trabajo completado: +G. " +
                      amount.ToString("N0");

        messageUntil = Time.time + 4f;

        Debug.Log(lastMessage);
    }

    public bool SpendMoney(int amount)
    {
        if (amount < 0 || Money < amount)
            return false;

        Money -= amount;
        return true;
    }

    void OnGUI()
    {
        GUI.Box(
            new Rect(15, 15, 260, 70),
            "TYVYTY - ECONOMIA"
        );

        GUI.Label(
            new Rect(30, 48, 230, 25),
            "Saldo: G. " + Money.ToString("N0")
        );

        if (Time.time < messageUntil)
        {
            GUI.Box(
                new Rect(15, 95, 320, 45),
                lastMessage
            );
        }
    }
}
