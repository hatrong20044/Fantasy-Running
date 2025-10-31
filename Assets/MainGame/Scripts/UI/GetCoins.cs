using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GetCoins : MonoBehaviour
{
    [SerializeField] private Camera UICamera;
    [SerializeField] private RectTransform coinStartPosition;
    [SerializeField] private RectTransform coinEndPosition;
    [SerializeField] private RectTransform coinBox;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject coinParticalEffect;

    private ParticleSystem coinParticle;

    public void RewardCoins()
    {
        int coinCount = Random.Range(10, 12);
        float delayIncrement = 0.1f;
        float incrementZ = -1;

        Vector3 startPos = ConvertUIToWorldPosition(coinStartPosition);
        Vector3 endPos = ConvertUIToWorldPosition(coinEndPosition);

        coinParticalEffect.transform.position = endPos;
        coinParticle = coinParticalEffect.GetComponent<ParticleSystem>();
        for (int i = 0; i < coinCount; i++)
        {
            SpawnCoin(startPos,endPos,ref delayIncrement, ref incrementZ);
        }
    }

    private void SpawnCoin(Vector3 startPos, Vector3 endPos, ref float delayIncrement, ref float incrementZ)
    {
        GameObject coin = Instantiate(coinPrefab, startPos, Quaternion.identity);
        Vector3 prefabOriginalScale = coinPrefab.transform.localScale;
        Vector3 spawnPosition = startPos + new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(-0.7f, 0.7f), incrementZ);
        coin.transform.localScale = Vector3.zero;
        coin.transform.DORotate(new Vector3(0, 360, 0), 0.8f,RotateMode.FastBeyond360)
            .SetEase(Ease.Flash).SetLoops(-1, LoopType.Incremental).SetDelay(delayIncrement);
        Sequence coinSequence = DOTween.Sequence();
        coinSequence.Append(coin.transform.DOScale(prefabOriginalScale, 0.5f).SetEase(Ease.OutQuad).SetDelay(delayIncrement))
        .Join(coin.transform.DOMove(spawnPosition, 0.5f).SetEase(Ease.InSine).SetDelay(delayIncrement))
        .Append(coin.transform.DOMove(endPos, 0.2f).SetEase(Ease.InQuad))
        .AppendCallback(() => OnCoinReached(coin));
        delayIncrement += 0.04f;
        incrementZ -= 0.02f;


    }

    private void OnCoinReached(GameObject coin)
    {
        if (coin == null) return;
        Transform coinboxTranform = coin.transform;
        coinParticle.Play();
        coinboxTranform.DOKill();
        
        coinboxTranform.localScale = Vector3.one;
        coinboxTranform.DOPunchScale(Vector3.one * -0.2f, 0.2f, 10, 1).SetEase(Ease.OutBounce)
            .OnComplete(() => coinboxTranform.localScale = Vector3.one);

        //int coins = int.Parse(coi)
        //CoinEvent.Instance.OnCoinCollected.Invoke(coin);

        // 👉 Destroy coin sau khi animation xong
        Destroy(coin);
    }
    private Vector3 ConvertUIToWorldPosition(RectTransform uiElement)
    {
        Vector3 screenPos = UICamera.WorldToScreenPoint(uiElement.position);
        Vector3 worldPos = UICamera.ScreenToWorldPoint(screenPos);
        worldPos.z = -1;
        return worldPos;
    }

}
