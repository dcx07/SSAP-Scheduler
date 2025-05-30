import json
from playwright.sync_api import sync_playwright
import time

def fetch_cookies(username, password):
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
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
        formatted = [{"name": c["name"], "value": c["value"]} for c in cookies]
        with open("cookies.json", "w", encoding="utf-8") as f:
            json.dump(formatted, f, indent=2, ensure_ascii=False)

        print("✅ 获取 Cookie 成功，已保存 cookies.json")

        browser.close()

# 从 config.json 获取账号密码
if __name__ == "__main__":
    with open("config.json", "r", encoding="utf-8") as f:
        config = json.load(f)
        username = config["username"]
        password = config["password"]
        fetch_cookies(username, password)
