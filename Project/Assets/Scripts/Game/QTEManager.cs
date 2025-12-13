using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QTEType
{
    SinglePress,    // ���ΰ���
    RapidPress,     // ����
    Sequence,        // �������У�����չ��
    NoPress
}

[System.Serializable]
public class QTEEvent
{
    public string eventName;
    public QTEType type;
    public KeyCode targetKey;
    public int rapidPressCount = 5; // ��������Ҫ��
    public float timeLimit = 3f;    // ʱ������
    public UnityEvent onSuccess;
    public UnityEvent onFailure;

    // ��������������
    public List<KeyCode> keySequence = new List<KeyCode>();
}

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance;

    [Header("QTE Settings")]
    public List<QTEEvent> qteEvents = new List<QTEEvent>();

    [Header("UI References")]
    public GameObject qteDisplayPanel;
    public TMPro.TextMeshProUGUI keyDisplayText;
    public UnityEngine.UI.Slider timeSlider;
    public TMPro.TextMeshProUGUI rapidPressCounter;
    public GameObject criticalArea;

    private QTEEvent currentQTE;
    private Coroutine currentQTECoroutine;
    private int currentRapidPressCount = 0;
    private bool isQTEActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        HideQTEUI();
    }

    // ����QTE�¼�
    public void TriggerQTE(string eventName)
    {
        QTEEvent qte = qteEvents.Find(e => e.eventName == eventName);
        if (qte != null && !isQTEActive)
        {
            currentQTE = qte;
            StartQTE(qte);
        }
    }

    private void StartQTE(QTEEvent qte)
    {
        isQTEActive = true;
        ShowQTEUI();
        SetupQTEUI(qte);

        switch (qte.type)
        {
            case QTEType.SinglePress:
                currentQTECoroutine = StartCoroutine(SinglePressQTE(qte));
                break;
            case QTEType.RapidPress:
                currentQTECoroutine = StartCoroutine(RapidPressQTE(qte));
                break;
            case QTEType.NoPress:
                currentQTECoroutine = StartCoroutine(NoPressQTE(qte));
                break;
            case QTEType.Sequence:
                currentQTECoroutine = StartCoroutine(SequenceQTE(qte));
                break;
        }
    }

    // ��������QTE
    private IEnumerator SequenceQTE(QTEEvent qte)
    {
        float timer = qte.timeLimit;
        int currentIndex = 0;

        while (timer > 0f && currentIndex < qte.keySequence.Count)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer / qte.timeLimit);

            // �������
            if (Input.GetKeyDown(qte.keySequence[currentIndex]))
            {
                currentIndex++;
                // ����UI��ʾ
                if (keyDisplayText != null) 
                {
                    Debug.Log(qte.keySequence.Count + "and" + currentIndex);
                    if ((currentIndex + 1) < qte.keySequence.Count)
                    {
                        keyDisplayText.text = $"<color=Yellow>{qte.keySequence[currentIndex].ToString()}</color> " + $"{qte.keySequence[currentIndex + 1].ToString()}";
                    }
                    else if ((currentIndex + 1) == qte.keySequence.Count)
                    {
                        keyDisplayText.text = $"<color=Yellow>{qte.keySequence[currentIndex].ToString()}</color>";
                    }
                }

            }
            else if (Input.anyKeyDown) // �����
            {
                qte.onFailure?.Invoke();
                EndQTE();
                yield break;
            }

            yield return null;
        }

        // �ж����
        if (currentIndex >= qte.keySequence.Count)
        {
            qte.onSuccess?.Invoke();
        }
        else
        {
            qte.onFailure?.Invoke();
        }

        EndQTE();
    }


    // ��ֹ����QTE
    private IEnumerator NoPressQTE(QTEEvent qte)
    {
        float timer = qte.timeLimit;
        bool failed = false;

        while (timer > 0f && !failed)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer / qte.timeLimit);

            // ����Ƿ�����ָ������
            if (Input.GetKeyDown(qte.targetKey))
            {
                failed = true;
                qte.onFailure?.Invoke();
                EndQTE();
                yield break;
            }

            yield return null;
        }

        // ���ʱ��ľ���û�а��� �� �ɹ�
        if (!failed)
        {
            qte.onSuccess?.Invoke();
        }

        EndQTE();
    }

    // ���ΰ���QTE
    private IEnumerator SinglePressQTE(QTEEvent qte)
    {
        float timer = qte.timeLimit;
        bool success = false;
        criticalArea.SetActive(true);

        while (timer > 0f && !success)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer / qte.timeLimit);

            // ��ⰴ������
            if (Input.GetKeyDown(qte.targetKey))
            {
                if (timer <= (qte.timeLimit / 4))
                {
                    success = true;
                    qte.onSuccess?.Invoke();
                }
                else 
                {
                    success = false;
                    qte.onFailure?.Invoke();
                    EndQTE();
                    yield return null;
                }
            }
            yield return null;
        }

        if (!success)
        {
            qte.onFailure?.Invoke();
        }

        EndQTE();
    }

    // ����QTE
    private IEnumerator RapidPressQTE(QTEEvent qte)
    {
        float timer = qte.timeLimit;
        currentRapidPressCount = 0;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer / qte.timeLimit);
            UpdateRapidPressCounter();

            // �����������
            if (Input.GetKeyDown(qte.targetKey))
            {
                currentRapidPressCount++;
                if (currentRapidPressCount >= qte.rapidPressCount)
                {
                    qte.onSuccess?.Invoke();
                    EndQTE();
                    yield break;
                }
            }

            yield return null;
        }

        // ʱ�䵽������Ƿ�ﵽҪ��
        if (currentRapidPressCount >= qte.rapidPressCount)
        {
            qte.onSuccess?.Invoke();
        }
        else
        {
            qte.onFailure?.Invoke();
        }

        EndQTE();
    }

    // UI��ط���
    private void ShowQTEUI()
    {
        if (qteDisplayPanel != null)
            qteDisplayPanel.SetActive(true);
    }

    private void HideQTEUI()
    {
        if (qteDisplayPanel != null)
            qteDisplayPanel.SetActive(false);
    }

    private void SetupQTEUI(QTEEvent qte)
    {
        if ((keyDisplayText != null) && (qte.type == QTEType.Sequence))
        {
            keyDisplayText.text = $"<color=Yellow>{qte.keySequence[0].ToString()}</color> " + $"{qte.keySequence[1].ToString()}";
        }
        else 
        {
            keyDisplayText.text = qte.targetKey.ToString();
        }

        if (qte.type == QTEType.NoPress)
        {
            keyDisplayText.text = $"<color=Red>No</color> {qte.targetKey}";
        }

        if (rapidPressCounter != null)
        {
            bool showCounter = qte.type == QTEType.RapidPress;
            rapidPressCounter.gameObject.SetActive(showCounter);
            if (showCounter)
                rapidPressCounter.text = $"0 / {qte.rapidPressCount}";
        }

        if (timeSlider != null)
            timeSlider.value = 1f;
    }

    private void UpdateTimerDisplay(float progress)
    {
        if (timeSlider != null)
            timeSlider.value = progress;
    }

    private void UpdateRapidPressCounter()
    {
        if (rapidPressCounter != null && currentQTE != null)
        {
            rapidPressCounter.text = $"{currentRapidPressCount} / {currentQTE.rapidPressCount}";
        }
    }

    private void EndQTE()
    {
        isQTEActive = false;
        criticalArea.SetActive(false);
        HideQTEUI();

        if (currentQTECoroutine != null)
        {
            StopCoroutine(currentQTECoroutine);
            currentQTECoroutine = null;
        }
    }

    // ǿ�ƽ�����ǰQTE
    public void ForceEndQTE(bool success = false)
    {
        if (isQTEActive && currentQTE != null)
        {
            if (success)
                currentQTE.onSuccess?.Invoke();
            else
                currentQTE.onFailure?.Invoke();

            EndQTE();
        }
    }
}