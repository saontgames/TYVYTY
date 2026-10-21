
using System.Collections;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    public static JobManager Instance { get; private set; }

    public enum JobStatus
    {
        Available,
        Accepted,
        InProgress,
        Completed
    }

    public enum JobType
    {
        FlowerRemoval,
        GravestoneCleaning
    }

    [System.Serializable]
    public class CemeteryJob
    {
        public string id;
        public string family;
        public string title;
        public string location;
        public string description;
        public int reward;
        public JobType type;

        [HideInInspector] public bool received;
        [HideInInspector] public JobStatus status;
    }

    public CemeteryJob[] jobs;

    [Header("Tiempos de prueba")]
    public float firstMessageDelay = 10f;
    public float secondMessageDelay = 30f;
    public float notificationDuration = 7f;

    private string notificationText = "";
    private float notificationEndTime;

    public int JobCount => jobs == null ? 0 : jobs.Length;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        jobs = new CemeteryJob[]
        {
            new CemeteryJob
            {
                id = "001",
                family = "Familia Gonzalez",
                title = "Retirar flores marchitas",
                location = "Sector A - Tumba 01",
                description = "Retirar flores y llevarlas al basurero.",
                reward = 15000,
                type = JobType.FlowerRemoval,
                status = JobStatus.Available
            },
            new CemeteryJob
            {
                id = "002",
                family = "Familia Benitez",
                title = "Limpieza de lapida",
                location = "Sector A - Tumba 02",
                description = "Limpiar la lapida completamente.",
                reward = 25000,
                type = JobType.GravestoneCleaning,
                status = JobStatus.Available
            }
        };
    }

    void Start()
    {
        StartCoroutine(DeliverRequests());
    }

    IEnumerator DeliverRequests()
    {
        yield return new WaitForSeconds(firstMessageDelay);
        ReceiveJob(0);

        yield return new WaitForSeconds(secondMessageDelay);
        ReceiveJob(1);
    }

    void ReceiveJob(int index)
    {
        CemeteryJob job = GetJob(index);

        if (job == null || job.received)
            return;

        job.received = true;

        notificationText =
            "Nuevo mensaje de " + job.family +
            "\nRevisa la computadora de la oficina.";

        notificationEndTime = Time.time + notificationDuration;

        Debug.Log("Nuevo pedido recibido: " + job.family);
    }

    public CemeteryJob GetJob(int index)
    {
        if (jobs == null || index < 0 || index >= jobs.Length)
            return null;

        return jobs[index];
    }

    public bool AcceptJob(int index)
    {
        CemeteryJob job = GetJob(index);

        if (job == null ||
            !job.received ||
            job.status != JobStatus.Available)
            return false;

        job.status = JobStatus.Accepted;

        Debug.Log("Pedido aceptado: " + job.title);
        return true;
    }

    public bool CanCollectFlowers()
    {
        CemeteryJob job = GetJob(0);

        return job != null &&
            (job.status == JobStatus.Accepted ||
             job.status == JobStatus.InProgress);
    }

    public void RegisterFlowerPickup()
    {
        CemeteryJob job = GetJob(0);

        if (job != null && job.status == JobStatus.Accepted)
            job.status = JobStatus.InProgress;
    }

    public bool CompleteFlowerDisposal()
    {
        CemeteryJob job = GetJob(0);

        if (job == null || job.status != JobStatus.InProgress)
            return false;

        return CompleteJob(0);
    }

    public bool CanCleanGravestone()
    {
        CemeteryJob job = GetJob(1);

        return job != null &&
            (job.status == JobStatus.Accepted ||
             job.status == JobStatus.InProgress);
    }

    public void RegisterCleaningProgress()
    {
        CemeteryJob job = GetJob(1);

        if (job != null && job.status == JobStatus.Accepted)
            job.status = JobStatus.InProgress;
    }

    public bool CompleteGravestoneCleaning()
    {
        CemeteryJob job = GetJob(1);

        if (job == null || job.status != JobStatus.InProgress)
            return false;

        return CompleteJob(1);
    }

    bool CompleteJob(int index)
    {
        CemeteryJob job = GetJob(index);

        if (job == null ||
            job.status != JobStatus.InProgress ||
            CemeteryEconomy.Instance == null)
            return false;

        // Se marca antes de pagar para evitar duplicaciones.
        job.status = JobStatus.Completed;

        CemeteryEconomy.Instance.AddMoney(job.reward);

        Debug.Log("Pedido #" + job.id + " completado.");
        return true;
    }

    public string GetStatusText(int index)
    {
        CemeteryJob job = GetJob(index);

        if (job == null || !job.received)
            return "Sin recibir";

        switch (job.status)
        {
            case JobStatus.Available: return "Disponible";
            case JobStatus.Accepted: return "Aceptado";
            case JobStatus.InProgress: return "En progreso";
            case JobStatus.Completed: return "Completado";
            default: return "Desconocido";
        }
    }

    public int GetGuidedJobIndex()
    {
        for (int i = 0; i < JobCount; i++)
        {
            CemeteryJob job = GetJob(i);

            if (job.status == JobStatus.Accepted ||
                job.status == JobStatus.InProgress)
                return i;
        }

        return -1;
    }

    void OnGUI()
    {
        if (OfficeComputer.IsOpen)
            return;

        // Mensaje temporal que aparece cuando llega el pedido.
        if (Time.time < notificationEndTime)
        {
            GUI.Box(
                new Rect(
                    Screen.width / 2f - 215,
                    15,
                    430,
                    75
                ),
                notificationText
            );
        }

        // No mostramos la lista de pedidos disponibles.
        // Solo aparece la checklist de trabajos aceptados.
        int index = GetGuidedJobIndex();

        if (index < 0)
            return;

        CemeteryJob job = GetJob(index);

        float x = Mathf.Max(10, Screen.width - 365);

        GUI.Box(
            new Rect(x, 105, 350, 145),
            "ENCARGO ACTIVO #" + job.id
        );

        GUI.Label(
            new Rect(x + 15, 137, 320, 24),
            job.family + " - " + job.title
        );

        if (job.type == JobType.FlowerRemoval)
        {
            bool pickedUp =
                job.status == JobStatus.InProgress;

            GUI.Label(
                new Rect(x + 15, 172, 325, 25),
                (pickedUp ? "[X] " : "[ ] ") +
                "Recoger las flores marchitas"
            );

            GUI.Label(
                new Rect(x + 15, 202, 325, 25),
                "[ ] Depositar flores en el basurero"
            );
        }
        else
        {
            GUI.Label(
                new Rect(x + 15, 172, 325, 25),
                "[ ] Limpiar la lapida de la tumba 02"
            );

            GUI.Label(
                new Rect(x + 15, 202, 325, 25),
                "Manten E para limpiar"
            );
        }
    }
}
