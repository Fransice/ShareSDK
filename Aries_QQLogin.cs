using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cn.sharesdk.unity3d;
using UnityEngine.UI;
using LitJson;
using System.IO;

public class Aries_QQLogin : MonoBehaviour
{
    /// <summary>
    /// ShareSDK
    /// </summary>
    private ShareSDK shareSdk;

    /// <summary>
    /// 为了防止有的朋友不方便看log，在这里做一个假的控制台
    /// </summary>
    public Text myConsole;

    public Text name;
    public RawImage icon;
    private static Aries_QQLogin _Instance;
    public InputField inputField;

    /// <summary>
    /// 获取单例
    /// </summary>
    /// <returns></returns>
    public static Aries_QQLogin GetInstance()
    {
        if (null == _Instance)
        {
            _Instance = new Aries_QQLogin();
        }

        return _Instance;
    }

    /// <summary>
    /// 当前登录的QQ用户信息
    /// </summary>
    public QQuserInfo CurrentQQuserInfo;

    /// <summary>
    /// QQ用户信息
    /// 
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
//        StartCoroutine(GetIcon("http://thirdqq.qlogo.cn/g?b=oidb&k=H8nl1yICMuZKuGJC9BaQnQ&s=100&t=1554373324"));
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

    /// <summary>
    /// 授权回掉
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        //如果授权成功
        if (state == ResponseState.Success)
        {
            print("authorize success !");
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

//            StartCoroutine(GetIcon(jd["userIcon"].ToString()));
            name.text = "昵称：" + CurrentQQuserInfo.userName;

            myConsole.text = "\n" + "userID:   " + CurrentQQuserInfo.userID + "\nuserName:" +
                             CurrentQQuserInfo.userName + "\nuserIcon:" + CurrentQQuserInfo.userIcon +
                             "\ntoken:" + CurrentQQuserInfo.token;
        }
        else if (state == ResponseState.Fail)
        {
            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
        }
        else if (state == ResponseState.Cancel)
        {
            print("cancel !");
        }
    }

    IEnumerator GetIcon(string url)
    {
        string path = Application.persistentDataPath + "/QQICON.jpg";
        WWW www = new WWW(url);
        yield return www;
        Texture2D texture2D = www.texture;
        byte[] bytes = texture2D.EncodeToPNG();
        myConsole.text += "\n" + path + "\n下载成功";
        try
        {
//            File.WriteAllBytes(path, bytes);
            myConsole.text += "\n写入成功";
            writeFile(path, bytes);
            StartCoroutine(SetIcon(path));
        }
        catch (Exception e)
        {
            inputField.text = e.ToString();
        }
    }

    IEnumerator SetIcon(string path)
    {
        WWW www = new WWW(path);
        myConsole.text += "\n正在加载";
        yield return www;
        if (www.isDone)
        {
            icon.texture = www.texture;
            myConsole.text += "\n加载成功";
        }
    }

    void CreateFile(string path, byte[] info)
    {
        //文件流信息
        StreamWriter sw;
        FileInfo t = new FileInfo(path);
        if (!t.Exists)
        {
            sw = t.CreateText();
        }
        else
        {
            sw = t.AppendText();
        }

        //以行的形式写入信息
//        sw.Write(info, 0, info.Length);
        //关闭流
        sw.Close();
        //销毁流
        sw.Dispose();
    }

    void writeFile(string path, byte[] bytes)
    {
        FileStream fs = new FileStream(path, FileMode.Create); //打开一个写入流
        fs.Write(bytes, 0, bytes.Length);
        fs.Flush(); //流会缓冲，此行代码指示流不要缓冲数据，立即写入到文件。
        fs.Close(); //关闭流并释放所有资源，同时将缓冲区的没有写入的数据，写入然后再关闭。
        fs.Dispose(); //释放流所占用的资源，Dispose()会调用Close(),Close()会调用Flush();    也会写入缓冲区内的数据。
    }
}