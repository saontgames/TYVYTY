
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public static JobManager Instance { get; private set; }

    public enum JobState
    {
        Pending,
        FlowersCollected,
        Completed
    }

    [Header("Configuracion del trabajo")]
    [SerializeField] private string jobName =
        "Limpieza de tumba 01";

    [SerializeField] private int reward = 15000;

    public JobState CurrentState { get; private set; }
        = JobState.Pending;

    private bool rewardPaid = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterFlowerPickup()
    {
        if (CurrentState != JobState.Pending)
            return;

        CurrentState = JobState.FlowersCollected;

        Debug.Log("Objetivo 1 completado: flores recogidas.");
    }

    public bool CompleteFlowerDisposal()
    {
        if (CurrentState != JobState.FlowersCollected)
            return false;

        if (rewardPaid)
            return false;

        if (CemeteryEconomy.Instance == null)
        {
            Debug.LogWarning("Falta CemeteryEconomy.");
            return false;
        }

        CurrentState = JobState.Completed;
        rewardPaid = true;

        CemeteryEconomy.Instance.AddMoney(reward);

        Debug.Log("Mision completada: " + jobName);

        return true;
    }

    void OnGUI()
    {
        GUI.Box(
            new Rect(15, 155, 340, 125),
            "TYVYTY - TRABAJO ACTUAL"
        );

        GUI.Label(
            new Rect(30, 185, 310, 25),
            jobName
        );

        string objective1 =
            CurrentState == JobState.Pending
            ? "[ ] Recoger flores marchitas"
            : "[X] Recoger flores marchitas";

        string objective2 =
            CurrentState == JobState.Completed
            ? "[X] Depositar flores en el basurero"
            : "[ ] Depositar flores en el basurero";

        GUI.Label(new Rect(30, 210, 310, 25), objective1);
        GUI.Label(new Rect(30, 235, 310, 25), objective2);

        if (CurrentState == JobState.Completed)
        {
            GUI.Label(
                new Rect(30, 258, 310, 20),
                "COMPLETADO"
            );
        }
    }
}
