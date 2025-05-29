# -*- coding: utf-8 -*-
import sys
import platform
import shutil
from selenium import webdriver
from selenium.webdriver.common.by import By
import time
import json

# 自动管理 WebDriver
try:
    from webdriver_manager.chrome import ChromeDriverManager
    from webdriver_manager.microsoft import EdgeChromiumDriverManager
    from webdriver_manager.firefox import GeckoDriverManager
except ImportError:
    print("未检测到 webdriver-manager，正在自动安装...")
    import subprocess
    subprocess.check_call([sys.executable, "-m", "pip", "install", "webdriver-manager"])
    from webdriver_manager.chrome import ChromeDriverManager
    from webdriver_manager.microsoft import EdgeChromiumDriverManager
    from webdriver_manager.firefox import GeckoDriverManager

try:
    with open("config.json", "r", encoding="utf-8") as f:
        config = json.load(f)
    username = config.get("username")
    password = config.get("password")
    if not username or not password:
        print("错误：config.json 文件中未找到 username 或 password。")
        sys.exit(1)
except FileNotFoundError:
    print("错误：未找到 config.json 文件。请创建该文件并填入用户名和密码。")
    sys.exit(1)
except json.JSONDecodeError:
    print("错误：config.json 文件格式不正确。")
    sys.exit(1)
except Exception as e:
    print(f"读取配置文件时发生错误: {e}")
    sys.exit(1)

driver = None

def try_local_driver():
    # 优先查找本地 chromedriver
    chromedriver_path = shutil.which("chromedriver")
    if chromedriver_path:
        try:
            from selenium.webdriver.chrome.service import Service as ChromeService
            from selenium.webdriver.chrome.options import Options as ChromeOptions
            options = ChromeOptions()
            options.add_argument("--headless")
            options.add_argument("--disable-gpu")
            service = ChromeService(chromedriver_path)
            return webdriver.Chrome(service=service, options=options)
        except Exception as e:
            print(f"本地 ChromeDriver 启动失败: {e}")

    # 查找本地 msedgedriver
    edgedriver_path = shutil.which("msedgedriver")
    if edgedriver_path:
        try:
            from selenium.webdriver.edge.service import Service as EdgeService
            from selenium.webdriver.edge.options import Options as EdgeOptions
            options = EdgeOptions()
            options.add_argument("--headless")
            options.add_argument("--disable-gpu")
            service = EdgeService(edgedriver_path)
            return webdriver.Edge(service=service, options=options)
        except Exception as e:
            print(f"本地 EdgeDriver 启动失败: {e}")

    # 查找本地 geckodriver
    geckodriver_path = shutil.which("geckodriver")
    if geckodriver_path:
        try:
            from selenium.webdriver.firefox.service import Service as FirefoxService
            from selenium.webdriver.firefox.options import Options as FirefoxOptions
            options = FirefoxOptions()
            options.add_argument("--headless")
            service = FirefoxService(geckodriver_path)
            return webdriver.Firefox(service=service, options=options)
        except Exception as e:
            print(f"本地 GeckoDriver 启动失败: {e}")

    # macOS 下尝试 Safari
    if platform.system() == "Darwin":
        try:
            return webdriver.Safari()  # Safari 不支持 headless
        except Exception as e:
            print(f"本地 Safari 启动失败: {e}")

    return None

def try_download_driver():
    # 依次尝试下载 Chrome、Edge、Firefox
    try:
        from selenium.webdriver.chrome.service import Service as ChromeService
        from selenium.webdriver.chrome.options import Options as ChromeOptions
        options = ChromeOptions()
        options.add_argument("--headless")
        options.add_argument("--disable-gpu")
        service = ChromeService(ChromeDriverManager().install())
        return webdriver.Chrome(service=service, options=options)
    except Exception as e:
        print(f"Chrome (自动下载) 启动失败: {e}")

    try:
        from selenium.webdriver.edge.service import Service as EdgeService
        from selenium.webdriver.edge.options import Options as EdgeOptions
        options = EdgeOptions()
        options.add_argument("--headless")
        options.add_argument("--disable-gpu")
        service = EdgeService(EdgeChromiumDriverManager().install())
        return webdriver.Edge(service=service, options=options)
    except Exception as e:
        print(f"Edge (自动下载) 启动失败: {e}")

    try:
        from selenium.webdriver.firefox.service import Service as FirefoxService
        from selenium.webdriver.firefox.options import Options as FirefoxOptions
        options = FirefoxOptions()
        options.add_argument("--headless")
        service = FirefoxService(GeckoDriverManager().install())
        return webdriver.Firefox(service=service, options=options)
    except Exception as e:
        print(f"Firefox (自动下载) 启动失败: {e}")

    return None

# 先尝试本地driver，再自动下载
driver = try_local_driver() or try_download_driver()

if driver is None:
    print("未检测到可用的浏览器驱动，请确保已安装 Chrome、Edge、Firefox 或 Safari 浏览器。")
    sys.exit(1)

# 打开登录页面
driver.get("https://sendeltastudent.schoolis.cn")
time.sleep(2)

driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.loginName']").send_keys(username)
driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.passWord']").send_keys(password)
driver.find_element(By.XPATH, "//button[@ng-click='$ctrl.login()']").click()
time.sleep(5)

cookies = driver.get_cookies()
with open("cookies.json", "w", encoding="utf-8") as f:
    json.dump(cookies, f, ensure_ascii=False, indent=2)

driver.quit()
