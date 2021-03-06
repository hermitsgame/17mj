﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {
    public Image _targetImg;
    public Text _targetName;
    public Text _targetPrice;
    public Image _spotLightImg;

    [HideInInspector]
    public string Name;
    [HideInInspector]
    public int Price;
    public int Id;

    private bool _changeFlag = false;
    private Sprite _spot1;
    private Sprite _spot2;

    void Start() {
        InvokeRepeating("ChangeSpotSprite", .5f, .5f);
    }

    public void setInfo(ShopItemInfo info)
    {
        _targetImg.sprite = Resources.Load<Sprite>("Image/Items/" + string.Format("{0:00}", info.Path2D));
        _targetImg.SetNativeSize();
        _targetName.text = info.Name;
        _targetPrice.text = string.Format("{0:0,0}", info.Price);
        Id = info.Id;
    }

    private void ChangeSpotSprite() {
        string _num = (_changeFlag) ? "1" : "2";
        if (_spotLightImg)
            _spotLightImg.sprite = Resources.Load<Sprite>("Image/shop_glow" + _num);

        _changeFlag = !_changeFlag;
    }
}