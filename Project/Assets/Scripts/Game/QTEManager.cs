using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QTEType
{
    SinglePress,    // 单次按键
    RapidPress,     // 连按
    Sequence,        // 按键序列（可扩展）
    NoPress
}

[System.Serializable]
public class QTEEvent
{
    public string eventName;
    public QTEType type;
    public KeyCode targetKey;
    public int rapidPressCount = 5; // 连按次数要求
    public float timeLimit = 3f;    // 时间限制
    public UnityEvent onSuccess;
    public UnityEvent onFailure;

    // 新增：按键序列
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

    // 触发QTE事件
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

    // 按键序列QTE
    private IEnumerator SequenceQTE(QTEEvent qte)
    {
        float timer = qte.timeLimit;
        int currentIndex = 0;

        while (timer > 0f && currentIndex < qte.keySequence.Count)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer / qte.timeLimit);

            // 检测输入
            if (Input.GetKeyDown(qte.keySequence[currentIndex]))
            {
                currentIndex++;
                // 更新UI提示
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
            else if (Input.anyKeyDown) // 按错键
            {
                qte.onFailure?.Invoke();
                EndQTE();
                yield break;
            }

            yield return null;
        }

        // 判定结果
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


    // 禁止按键QTE
    private IEnumerator NoPressQTE(QTEEvent qte)
    {
        float timer = qte.timeLimit;
        bool failed = false;

        while (timer > 0f && !failed)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer / qte.timeLimit);

            // 检测是否按下了指定按键
            if (Input.GetKeyDown(qte.targetKey))
            {
                failed = true;
                qte.onFailure?.Invoke();
                EndQTE();
                yield break;
            }

            yield return null;
        }

        // 如果时间耗尽且没有按键 → 成功
        if (!failed)
        {
            qte.onSuccess?.Invoke();
        }

        EndQTE();
    }

    // 单次按键QTE
    private IEnumerator SinglePressQTE(QTEEvent qte)
    {
        float timer = qte.timeLimit;
        bool success = false;
        criticalArea.SetActive(true);

        while (timer > 0f && !success)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer / qte.timeLimit);

            // 检测按键输入
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

    // 连按QTE
    private IEnumerator RapidPressQTE(QTEEvent qte)
    {
        float timer = qte.timeLimit;
        currentRapidPressCount = 0;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay(timer / qte.timeLimit);
            UpdateRapidPressCounter();

            // 检测连按输入
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

        // 时间到，检查是否达到要求
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

    // UI相关方法
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

    // 强制结束当前QTE
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