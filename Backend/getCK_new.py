import json
from playwright.sync_api import sync_playwright
import time
import sys
import os

def fetch_cookies(username, password):
    try:
        with sync_playwright() as p:
            browser = None
            # 尝试使用系统 Chrome 浏览器
            try:
                browser = p.chromium.launch(headless=True)
                print("✅ 使用 Playwright Chromium")
            except Exception as e:
                print(f"⚠️ Playwright Chromium 启动失败: {e}")
                print("正在尝试使用系统浏览器...")
                
                # 尝试使用系统安装的浏览器（优先 Edge，再尝试 Chrome）
                browser_paths = [
                    # Microsoft Edge 路径
                    r"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                    r"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
                    os.path.expanduser(r"~\AppData\Local\Microsoft\Edge\Application\msedge.exe"),
                    # Google Chrome 路径
                    r"C:\Program Files\Google\Chrome\Application\chrome.exe",
                    r"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                    os.path.expanduser(r"~\AppData\Local\Google\Chrome\Application\chrome.exe")
                ]
                
                browser_path = None
                browser_name = None
                for path in browser_paths:
                    if os.path.exists(path):
                        browser_path = path
                        if "msedge.exe" in path:
                            browser_name = "Microsoft Edge"
                        elif "chrome.exe" in path:
                            browser_name = "Google Chrome"
                        break
                
                if browser_path:
                    browser = p.chromium.launch(executable_path=browser_path, headless=True)
                    print(f"✅ 使用系统浏览器: {browser_name} ({browser_path})")
                else:
                    print("❌ 未找到可用的浏览器")
                    print("请安装 Microsoft Edge 或 Google Chrome，或运行: playwright install chromium")
                    return False
            
            if browser is None:
                print("❌ 无法启动浏览器")
                return False
                
            page = browser.new_page()

            page.goto("https://sendeltastudent.schoolis.cn")
            time.sleep(2)  # 保证加载完 Angular

            # 填写用户名和密码
            page.fill("input[ng-model='$ctrl.loginName']", username)
            page.fill("input[ng-model='$ctrl.passWord']", password)

            # 点击登录按钮
            page.click("button[ng-click='$ctrl.login()']")

            # 等待跳转或页面加载
            page.wait_for_url("**/Home", timeout=10000)

            # 获取所有 Cookie
            cookies = page.context.cookies()

            # 保存为 cookies.json（与 requests 兼容格式）
            formatted = [{"name": c.get("name", ""), "value": c.get("value", "")} for c in cookies]
            with open("cookies.json", "w", encoding="utf-8") as f:
                json.dump(formatted, f, indent=2, ensure_ascii=False)

            print("✅ 获取 Cookie 成功，已保存 cookies.json")

            browser.close()
            return True
            
    except Exception as e:
        print(f"❌ 获取 Cookie 失败: {e}")
        return False

# 从 config.json 获取账号密码
if __name__ == "__main__":
    with open("config.json", "r", encoding="utf-8") as f:
        config = json.load(f)
        username = config["Username"]
        password = config["Password"]
        fetch_cookies(username, password)
