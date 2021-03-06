﻿using UnityEngine;
using UnityEngine.UI;
#if UNITY_IPHONE || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class UnityAds : MonoBehaviour {
    public GameObject _earnRewardPanel;
    private Text _earnRewardTitle;
    private Text _earnRewardContent;

    void Start () {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate
        {
            ShowRewardedAd();
        });

        if (_earnRewardPanel)
        {
            _earnRewardTitle = _earnRewardPanel.transform.Find("main/Title").GetComponent<Text>();
            _earnRewardContent = _earnRewardPanel.transform.Find("main/midBg/Content").GetComponent<Text>();
            _earnRewardPanel.SetActive(false);
        }
            
    }

    public void ShowRewardedAd()
    {
		#if UNITY_IPHONE || UNITY_ANDROID
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
		#endif
    }

	#if UNITY_IPHONE || UNITY_ANDROID

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
        ShowEarnReward(result);
    }


    private void ShowEarnReward(ShowResult result)
    {
        int _reward = 0;
        string _title = string.Empty;
        string _content = string.Empty;
        //_rewardCoin
        if (_earnRewardPanel) {
            switch (result) {
                case ShowResult.Finished:
                    _title = "恭喜您獲得";
                    _reward = Random.Range(1, 10)*200;
                    _content = "$ " + string.Format("{0:0,0}", _reward);
                    break;
                case ShowResult.Skipped:
                    _title = "真可惜，您跳過了廣告";
                    _content = "$ 0";
                    break;
                case ShowResult.Failed:
                    _title = "抱歉，廣告商遇到了一些問題";
                    _content = "@#$%&*!";
                    break;
            }

            _earnRewardTitle.text = _title;
            _earnRewardContent.text = _content;

            com.Lobby.Launcher.instance.ChangeCoin(_reward);
            _earnRewardPanel.SetActive(true);

        }
    }
	#endif

    public void ClickAcceptEarnReward() {
        if (_earnRewardPanel)
            _earnRewardPanel.SetActive(false);
    }

}
