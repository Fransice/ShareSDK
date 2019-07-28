using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cn.sharesdk.unity3d;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class Loding : MonoBehaviour
{
    /// <summary>
    /// ShareSDK
    /// </summary>
    private ShareSDK shareSdk;

    /// <summary>
    /// 为了防止有的朋友不方便看log，在这里做一个假的控制台
    /// </summary>
    public Text myConsole;

    private static Loding _Instance;


    /// <summary>
    /// 获取单例
    /// 测试
    /// </summary>
    /// <returns></returns>
    public static Loding GetInstance()
    {
        if (null == _Instance)
        {
            _Instance = new Loding();
        }

        return _Instance;
    }

    /// <summary>
    /// 当前登录的QQ用户信息
    /// </summary>
    public QQuserInfo CurrentQQuserInfo;

    /// <summary>
    /// QQ用户信息
    /// sada asda sd asd a s
    /// 说明：我只用到了四个属性，所以构造器只写了四个，如果用到更多可以自行增加
    /// </summary>
    public class QQuserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string userID = "";

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName = "";

        /// <summary>
        /// 用户头像
        /// </summary>
        public string userIcon = "";

        /// <summary>
        /// 用户记号
        /// </summary>
        public string token = "";


        public string refresh_token = "";
        public int openID;
        public int expiresIn;
        public string userGender = "";
        public string tokenSecret = "";
        public string unionID = "";
        public long expiresTime;

        public QQuserInfo()
        {
        }

        public QQuserInfo(string uID, string uName, string uIcon, string uToken)
        {
            this.userID = uID;
            this.userName = uName;
            this.userIcon = uIcon;
            this.token = uToken;
        }
    }

    void Start()
    {
        //获取ShareSDK
        shareSdk = GetComponent<ShareSDK>();
        shareSdk.shareHandler = ShareResultHandler;
    }


    /// <summary>
    /// 用户授权  QQ
    /// </summary>
    public void QQLogin()
    {
        //设置回掉函数
        shareSdk.authHandler = AuthResultHandler;

        //请求授权
        shareSdk.Authorize(PlatformType.QQ);
    }

    /*
     * Text ：分享的文字
        Title: 分享的标题
        TitleUrl ：标题的网络链接（QQ和QQ空间使用 ）
        SetImageUrl  ：iOS平台，本地以及网络图片都使用此方法
        image：android平台分享本地图片与网络图片都用此方法
        Url： 分享的链接（微信，微博，易信，Facebook等平台）
        ShareType：分享类型（微信，易信）
        FilePath：分享文件路径 （微信，易信）
        MusicUrl ：分享的音乐链接（微信，QQ，易信）
     */
    public void fenxiang__QQ() //QQ分享
    {
        ShareContent content = new ShareContent();
        content.SetTitle("测试");
        content.SetText("测试文本");
        content.SetTitleUrl(
            "https://www.gamersky.com/showimage/id_gamersky.shtml?http://img1.gamersky.com/image2019/07/20190725_ls_red_141_2/gamersky_042origin_083_2019725182972C.jpg");
        content.SetImageUrl(
            "https://www.gamersky.com/showimage/id_gamersky.shtml?http://img1.gamersky.com/image2019/07/20190725_ls_red_141_2/gamersky_042origin_083_2019725182972C.jpg");
        content.SetShareType(ContentType.Image);
        shareSdk.ShareContent(PlatformType.QQ, content);
//        shareSdk.ShowPlatformList(null, content, 100, 100);
    }

    public void fenxiang_WX()
    {
        ShareContent content = new ShareContent();
        content.SetText("文本");
        content.SetImagePath("/Snapseed/1.jpep");
        content.SetTitle("标题");
        content.SetShareType(ContentType.Image);
        shareSdk.ShareContent(PlatformType.WeChat, content);
    }

    //以下为回调的定义:
    void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        myConsole.text += type.ToString();
        switch (state)
        {
            case ResponseState.Success:
                myConsole.text += "\n分享成功" + MiniJSON.jsonEncode(result);
                break;
            case ResponseState.Fail:
                myConsole.text += "\n分享失败" + "fail! throwable stack = " + result["stack"] + "; error msg = " +
                                  result["msg"];
                break;
            case ResponseState.Cancel:
                myConsole.text += "\n分享取消";
                break;
        }
    }


    /// <summary>
    /// 授权回掉
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        switch (state)
        {
            case ResponseState.Success:
                print("authorize success !");

                //在控制台输出
                myConsole.text += "\n" + "authorize success !";

                Hashtable user = shareSdk.GetAuthInfo(PlatformType.QQ);

                print("get user info result =====================>:");
                print(MiniJSON.jsonEncode(user));

                JsonData jd = JsonMapper.ToObject(MiniJSON.jsonEncode(user));

                //实例化当前登录的QQ用户信息
                CurrentQQuserInfo = new QQuserInfo(jd["userID"].ToString(), jd["userName"].ToString(),
                    jd["userIcon"].ToString(), jd["token"].ToString());

                //打印用户信息
                print("userID:" + CurrentQQuserInfo.userID + "===" + "userName:" + CurrentQQuserInfo.userName + "===" +
                      "userIcon:" + CurrentQQuserInfo.userIcon + "===" +
                      "token:" + CurrentQQuserInfo.token);


                //在控制台输出
                myConsole.text += "\n" + "userID:" + CurrentQQuserInfo.userID + "===" + "userName:" +
                                  CurrentQQuserInfo.userName + "===" + "userIcon:" + CurrentQQuserInfo.userIcon +
                                  "===" +
                                  "token:" + CurrentQQuserInfo.token;
                break;
            case ResponseState.Fail:
                print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
                break;
            case ResponseState.Cancel:
                print("cancel !");
                break;
        }
    }
}