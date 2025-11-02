using DG.Tweening;
using TMPro;
using UnityEngine;

public class GetCoins : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform coinStartPosition;
    [SerializeField] private RectTransform coinEndPosition;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject coinParticalEffect;
    [SerializeField] private CoinManager coinManager;

    private ParticleSystem coinParticle;
    //void Start()
    //{
    //   RewardCoins();
    //}

    public void RewardCoins()
    {
        int coinCount = Random.Range(10, 12);
        float delayIncrement = 0.1f;
        float incrementZ = -1;

        // ✅ Canvas Overlay không có camera → dùng trực tiếp position trong canvas space
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
        coinParticalEffect.transform.position = worldPos;
        Debug.Log("Set particle effect position to: " + endPos); 
        coinParticle = coinParticalEffect.GetComponent<ParticleSystem>();

        for (int i = 0; i < coinCount; i++)
        {
            SpawnCoin(startPos, endPos, ref delayIncrement, ref incrementZ);
        }
    }

    private void SpawnCoin(Vector3 startPos, Vector3 endPos, ref float delayIncrement, ref float incrementZ)
    {
        GameObject coin = Instantiate(coinPrefab, startPos, Quaternion.identity, transform);
        Vector3 prefabOriginalScale = coinPrefab.transform.localScale;

        Vector3 spawnPosition = startPos + new Vector3(Random.Range(-60f, 60f), Random.Range(-70f, 70f), 0);
        coin.transform.localScale = Vector3.zero;

        // ✅ Vì đang trong UI nên đảm bảo có RectTransform
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
        Debug.Log("Play coin particle effect not null");
        coinParticle.Play();

        Transform coinTransform = coin.transform;
        coinTransform.DOKill();

        coinTransform.localScale = Vector3.one;
        coinTransform.DOPunchScale(Vector3.one * -0.2f, 0.2f, 10, 1)
                     .SetEase(Ease.OutBounce)
                     .OnComplete(() => coinTransform.localScale = Vector3.one);

        if (coinManager != null)
            coinManager.AddCoin(1);

        Destroy(coin);
    }
}
