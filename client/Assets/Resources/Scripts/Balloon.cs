﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Balloon : MonoBehaviour {
    public Image _balloonImg;
    public GameObject _particle;
    public Transform _star;
    public int _clickEarn = 5;
    private Sprite[] _sprite;

    void Start() {
        _sprite = Resources.LoadAll<Sprite>("Image/balloon");
        _balloonImg.sprite = _sprite[Random.Range(0, 5)];
    }

    public void ClickBalloon() {
        //if (!com.Lobby.Launcher.instance.coinAPIcallback)
        //    return;
		#if UNITY_IOS || UNITY_ANDROID
        if(PlayerPrefExtension.GetBool("Vibrate_enabled"))
            Handheld.Vibrate();
		#endif
        transform.DOScale(new Vector3(1.3f, 1.3f, 1), 0.2f).SetEase(Ease.OutElastic);
        _balloonImg.DOFade(0, 0.1f).SetEase(Ease.InSine);
        _particle.SetActive(true);

        com.Lobby.Launcher.instance.ChangeCoin(_clickEarn);

        if (_star) {
            _star.gameObject.SetActive(true);
            _star.DOMoveX(-2.76f, 0.5f, false).SetEase(Ease.InOutFlash);
            _star.DOMoveY(3.21f, 0.5f, false).SetEase(Ease.InOutFlash);
            _star.DOLocalRotate(new Vector3(0, 0, -300), 0.1f).SetLoops(-1, LoopType.Yoyo);
            _star.GetComponent<Image>().DOFade(0, 0.1f).SetDelay(0.5f);
        }
    }
}
