using DG.Tweening;
using TMPro;
using UnityEngine;

public class GetCoins : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform coinStartPosition;
    [SerializeField] private RectTransform coinEndPosition;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private CoinManager coinManager;

    private ParticleSystem coinParticle;

    private void Start()
    {
        Debug.Log($"🔍 GetCoins initialized:");
        Debug.Log($"  - coinStartPosition: {(coinStartPosition != null ? "OK" : "NULL")}");
        Debug.Log($"  - coinEndPosition: {(coinEndPosition != null ? "OK" : "NULL")}");
        Debug.Log($"  - coinPrefab: {(coinPrefab != null ? "OK" : "NULL")}");
        Debug.Log($"  - coinManager: {(coinManager != null ? "OK" : "NULL")}");
    }

    public void RewardCoins()
    {
        Debug.Log($"🪙 RewardCoins called - Start: {coinStartPosition?.name}, End: {coinEndPosition?.name}");

        // ✅ KIỂM TRA NULL CHO TẤT CẢ UI REFERENCES TRƯỚC KHI DÙNG
        if (coinStartPosition == null)
        {
            Debug.LogError("❌ coinStartPosition is NULL!");
            return;
        }

        if (coinEndPosition == null)
        {
            Debug.LogError("❌ coinEndPosition is NULL!");
            return;
        }

        if (coinPrefab == null)
        {
            Debug.LogError("❌ coinPrefab is NULL!");
            return;
        }

        // ✅ KIỂM TRA CANVAS
        Canvas canvas = coinStartPosition.GetComponentInParent<Canvas>();
        if (canvas == null || !canvas.enabled)
        {
            Debug.LogError("❌ Canvas is disabled or not found!");
            return;
        }

        // ✅ KIỂM TRA GAMEOBJECT CÒN ACTIVE
        if (!coinStartPosition.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("⚠️ coinStartPosition is not active!");
            return;
        }

        if (!coinEndPosition.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("⚠️ coinEndPosition is not active!");
            return;
        }

        int coinCount = Random.Range(10, 12);
        float delayIncrement = 0.1f;
        float incrementZ = -1;

        // Canvas Overlay không có camera → dùng trực tiếp position trong canvas space
        Vector3 startPos = coinStartPosition.position;
        Vector3 endPos = coinEndPosition.position;

        // Hiệu ứng hạt
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            coinEndPosition,
            RectTransformUtility.WorldToScreenPoint(null, coinEndPosition.position),
            null,
            out worldPos
        );

        Debug.Log($"✅ Spawning {coinCount} coins from {startPos} to {endPos}");

        for (int i = 0; i < coinCount; i++)
        {
            SpawnCoin(startPos, endPos, ref delayIncrement, ref incrementZ);
        }
    }

    private void SpawnCoin(Vector3 startPos, Vector3 endPos, ref float delayIncrement, ref float incrementZ)
    {
        // ✅ KIỂM TRA TRANSFORM CÒN TỒN TẠI
        if (this == null || transform == null)
        {
            Debug.LogWarning("⚠️ GetCoins transform is destroyed - cannot spawn coin!");
            return;
        }

        if (coinPrefab == null)
        {
            Debug.LogWarning("⚠️ coinPrefab became null!");
            return;
        }

        GameObject coin = Instantiate(coinPrefab, startPos, Quaternion.identity, transform);
        if (coin == null)
        {
            Debug.LogWarning("⚠️ Failed to instantiate coin!");
            return;
        }

        Vector3 prefabOriginalScale = coinPrefab.transform.localScale;

        Vector3 spawnPosition = startPos + new Vector3(Random.Range(-60f, 60f), Random.Range(-70f, 70f), 0);
        coin.transform.localScale = Vector3.zero;

        // Vì đang trong UI nên đảm bảo có RectTransform
        RectTransform coinRect = coin.GetComponent<RectTransform>();
        if (coinRect == null)
            coinRect = coin.AddComponent<RectTransform>();

        // Animation bay
        Sequence coinSequence = DOTween.Sequence();
        coinSequence.Append(coinRect.DOScale(prefabOriginalScale, 0.5f).SetEase(Ease.OutQuad).SetDelay(delayIncrement))
                    .Join(coinRect.DOMove(spawnPosition, 0.5f).SetEase(Ease.InSine).SetDelay(delayIncrement))
                    .Append(coinRect.DOMove(endPos, 0.3f).SetEase(Ease.InQuad))
                    .AppendCallback(() => OnCoinReached(coin));

        delayIncrement += 0.04f;
        incrementZ -= 0.02f;
    }

    private void OnCoinReached(GameObject coin)
    {
        if (coin == null) return;

        if (coinParticle != null)
        {
            Debug.Log("Play coin particle effect");
            coinParticle.Play();
        }

        Transform coinTransform = coin.transform;
        if (coinTransform == null)
        {
            Destroy(coin);
            return;
        }

        coinTransform.DOKill();

        coinTransform.localScale = Vector3.one;
        coinTransform.DOPunchScale(Vector3.one * -0.2f, 0.2f, 10, 1)
                     .SetEase(Ease.OutBounce)
                     .OnComplete(() => {
                         if (coinTransform != null)
                             coinTransform.localScale = Vector3.one;
                     });

        if (coinManager != null)
            coinManager.AddCoin(1);

        Destroy(coin);
    }

    // ✅ CLEANUP KHI OBJECT BỊ DESTROY/DISABLE
    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}