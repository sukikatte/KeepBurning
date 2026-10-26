using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public class AuthService : MonoBehaviour
{
    public static AuthService Instance { get; private set; }

    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore Db { get; private set; }

    public bool IsReady { get; private set; }
    public bool IsLoggedIn => IsReady && Auth != null && Auth.CurrentUser != null;
    public string CurrentUid => IsLoggedIn ? Auth.CurrentUser.UserId : null;

    private static readonly Regex UsernameRegex = new Regex(@"^[A-Za-z0-9_]{3,20}$");
    private static readonly Regex PinRegex = new Regex(@"^\d{4,12}$");

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await InitFirebase();
    }

    public async Task<bool> InitFirebase()
    {
        try
        {
            var dep = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dep != DependencyStatus.Available)
            {
                Debug.LogError("Firebase dependency error: " + dep);
                return false;
            }

            Auth = FirebaseAuth.DefaultInstance;
            Db = FirebaseFirestore.DefaultInstance;
            IsReady = true;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase init failed: " + e.Message);
            return false;
        }
    }

    public static bool ValidateUsername(string username)
    {
        return !string.IsNullOrEmpty(username) && UsernameRegex.IsMatch(username);
    }

    public static bool ValidatePin(string pin)
    {
        return !string.IsNullOrEmpty(pin) && PinRegex.IsMatch(pin);
    }

    private string BuildEmail(string username)
    {
        return username.Trim().ToLowerInvariant() + "@keepburning.local";
    }

    // 注册：用户名唯一 + 数字密码
    public async Task<(bool ok, string msg)> Register(string username, string pin)
    {
        if (!IsReady) return (false, "Firebase not ready.");

        username = username.Trim();
        if (!ValidateUsername(username)) return (false, "Invalid username (A-Z, 0-9, _, 3-20).");
        if (!ValidatePin(pin)) return (false, "Invalid PIN (4-12 digits).");

        string unameLower = username.ToLowerInvariant();

        try
        {
            // 1) 检查用户名是否已存在
            var unameRef = Db.Collection("usernames").Document(unameLower);
            var unameSnap = await unameRef.GetSnapshotAsync();
            if (unameSnap.Exists) return (false, "Username already exists.");

            // 2) 创建 Auth 用户（伪邮箱）
            string email = BuildEmail(username);
            AuthResult authResult = await Auth.CreateUserWithEmailAndPasswordAsync(email, pin);
            FirebaseUser user = authResult.User;
            if (user == null) return (false, "Register failed: user is null.");

            string uid = user.UserId;

            // 3) 写 Firestore 用户档案
            var userRef = Db.Collection("users").Document(uid);
            var userData = new Dictionary<string, object>
            {
                { "username", username },
                { "totalRescued", 0 },
                { "bestSingleRunRescued", 0 },
                { "createdAt", Timestamp.GetCurrentTimestamp() }
            };

            await userRef.SetAsync(userData);
            await unameRef.SetAsync(new Dictionary<string, object> { { "uid", uid } });

            return (true, "Register success.");
        }
        catch (Exception e)
        {
            return (false, "Register failed: " + e.Message);
        }
    }

    // 登录：用户名 + PIN -> 伪邮箱登录
    public async Task<(bool ok, string msg)> Login(string username, string pin)
    {
        if (!IsReady) return (false, "Firebase not ready.");

        username = username.Trim();
        if (!ValidateUsername(username)) return (false, "Invalid username.");
        if (!ValidatePin(pin)) return (false, "Invalid PIN.");

        string email = BuildEmail(username);

        try
        {
            AuthResult authResult = await Auth.SignInWithEmailAndPasswordAsync(email, pin);
            FirebaseUser user = authResult.User;
            if (user == null) return (false, "Login failed.");

            return (true, "Login success.");
        }
        catch (Exception e)
        {
            return (false, "Login failed: " + e.Message);
        }
    }

    public void Logout()
    {
        if (!IsReady || Auth == null) return;
        Auth.SignOut();
    }

    public async Task<Dictionary<string, object>> GetMyProfile()
    {
        if (!IsLoggedIn) return null;

        var snap = await Db.Collection("users").Document(CurrentUid).GetSnapshotAsync();
        return snap.Exists ? snap.ToDictionary() : null;
    }

    // 每局结束更新统计
    public async Task SubmitRunRescued(int rescuedInRun)
    {
        if (!IsLoggedIn) return;

        var userRef = Db.Collection("users").Document(CurrentUid);
        var snap = await userRef.GetSnapshotAsync();
        if (!snap.Exists) return;

        var data = snap.ToDictionary();

        int total = data.ContainsKey("totalRescued") ? Convert.ToInt32(data["totalRescued"]) : 0;
        int best = data.ContainsKey("bestSingleRunRescued") ? Convert.ToInt32(data["bestSingleRunRescued"]) : 0;

        total += rescuedInRun;
        best = Math.Max(best, rescuedInRun);

        await userRef.UpdateAsync(new Dictionary<string, object>
        {
            { "totalRescued", total },
            { "bestSingleRunRescued", best }
        });
    }
}