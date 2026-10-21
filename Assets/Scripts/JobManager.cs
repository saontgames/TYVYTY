
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public static JobManager Instance { get; private set; }

    public enum JobState
    {
        Available,
        Pending,
        FlowersCollected,
        Completed
    }

    [Header("Configuracion del trabajo")]
    [SerializeField] private string jobName =
        "Limpieza de tumba 01";

    [SerializeField] private int reward = 15000;

    public JobState CurrentState { get; private set; }
        = JobState.Available;

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

    // Aceptar el trabajo desde la computadora
    public bool AcceptJob()
    {
        if (CurrentState != JobState.Available)
            return false;

        CurrentState = JobState.Pending;

        Debug.Log("Trabajo aceptado: " + jobName);
        return true;
    }

    // Registrar que el jugador recogio las flores
    public void RegisterFlowerPickup()
    {
        if (CurrentState != JobState.Pending)
            return;

        CurrentState = JobState.FlowersCollected;

        Debug.Log("Objetivo 1: flores recogidas.");
    }

    // Solo permite recoger flores con un trabajo activo
    public bool CanCollectFlowers()
    {
        return CurrentState == JobState.Pending ||
               CurrentState == JobState.FlowersCollected;
    }

    // Completar el trabajo y pagar una sola vez
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

        rewardPaid = true;
        CurrentState = JobState.Completed;

        CemeteryEconomy.Instance.AddMoney(reward);

        Debug.Log("Mision completada: " + jobName);

        return true;
    }

    // Estado para mostrar en la computadora
    public string GetStatusText()
    {
        switch (CurrentState)
        {
            case JobState.Available:
                return "Disponible";

            case JobState.Pending:
                return "Aceptado - pendiente";

            case JobState.FlowersCollected:
                return "Flores recogidas";

            case JobState.Completed:
                return "Completado";

            default:
                return "Desconocido";
        }
    }

    // Interfaz provisional de objetivos
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

        if (CurrentState == JobState.Available)
        {
            GUI.Label(
                new Rect(30, 215, 310, 25),
                "Visita la oficina para aceptar el trabajo."
            );

            return;
        }

        string objective1 =
            CurrentState == JobState.Pending
            ? "[ ] Recoger flores marchitas"
            : "[X] Recoger flores marchitas";

        string objective2 =
            CurrentState == JobState.Completed
            ? "[X] Depositar flores en el basurero"
            : "[ ] Depositar flores en el basurero";

        GUI.Label(
            new Rect(30, 210, 310, 25),
            objective1
        );

        GUI.Label(
            new Rect(30, 235, 310, 25),
            objective2
        );

        if (CurrentState == JobState.Completed)
        {
            GUI.Label(
                new Rect(30, 258, 310, 20),
                "COMPLETADO"
            );
        }
    }
}
