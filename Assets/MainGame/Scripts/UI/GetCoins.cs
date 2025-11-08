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
        if (coinManager == null)
            coinManager = FindObjectOfType<CoinManager>();
    }

    public void RewardCoins()
    {
        Debug.Log($"🪙 RewardCoins called - Start: {coinStartPosition?.name}, End: {coinEndPosition?.name}");

        // ✅ Kiểm tra null thật sự
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

        // ✅ Kiểm tra Canvas, nhưng KHÔNG return nếu disable (để tránh mất logic khi scene reload)
        Canvas canvas = coinStartPosition.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("⚠️ Canvas not found, continue anyway");
        }
        else if (!canvas.enabled)
        {
            Debug.LogWarning("⚠️ Canvas disabled, continue anyway");
        }

        // ✅ Kiểm tra UI active — chỉ cảnh báo, KHÔNG return
        if (!coinStartPosition.gameObject.activeInHierarchy)
            Debug.LogWarning("⚠️ coinStartPosition inactive (ignored check)");

        if (!coinEndPosition.gameObject.activeInHierarchy)
            Debug.LogWarning("⚠️ coinEndPosition inactive (ignored check)");

        int coinCount = 10;
        float delayIncrement = 0.1f;
        float incrementZ = -1;

        Vector3 startPos = coinStartPosition.position;
        Vector3 endPos = coinEndPosition.position;

        Debug.Log($"✅ Spawning {coinCount} coins from {startPos} to {endPos}");

        for (int i = 0; i < coinCount; i++)
        {
            SpawnCoin(startPos, endPos, ref delayIncrement, ref incrementZ);
        }
    }

    private void SpawnCoin(Vector3 startPos, Vector3 endPos, ref float delayIncrement, ref float incrementZ)
    {
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

        RectTransform coinRect = coin.GetComponent<RectTransform>();
        if (coinRect == null)
            coinRect = coin.AddComponent<RectTransform>();

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

        // ✅ Đảm bảo coinManager tồn tại thật sự
        if (coinManager == null || coinManager.Equals(null))
        {
            coinManager = FindObjectOfType<CoinManager>();
            if (coinManager == null)
            {
                Debug.LogError("❌ Không tìm thấy CoinManager trong scene!");
                Destroy(coin);
                return;
            }
        }

        Debug.Log("Adding 1 coin to CoinManager");
        Debug.Log("🧩 CoinManager ref: " + coinManager);
        coinManager.AddCoin(1);

        Destroy(coin);
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}
