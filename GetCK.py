# -*- coding: utf-8 -*-
import sys
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.edge.service import Service
from selenium.webdriver.edge.options import Options
import time
import json

# 从命令行参数获取用户名和密码
username = sys.argv[1]
password = sys.argv[2]

# 配置 Edge 浏览器为无头模式
options = Options()
options.add_argument("--headless")  # 启用无头模式
options.add_argument("--disable-gpu")  # 禁用 GPU 加速（可选，提升兼容性）

# 启动 Edge
service = Service("msedgedriver.exe")
driver = webdriver.Edge(service=service, options=options)

# 打开登录页面
driver.get("https://sendeltastudent.schoolis.cn")
time.sleep(2)

# 定位用户名输入框并输入
driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.loginName']").send_keys(username)

# 定位密码输入框并输入
driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.passWord']").send_keys(password)

# 点击登录按钮
driver.find_element(By.XPATH, "//button[@ng-click='$ctrl.login()']").click()

time.sleep(5)

# 获取登录后的 cookies
cookies = driver.get_cookies()


# 保存 cookies
with open("cookies.json", "w", encoding="utf-8") as f:
    json.dump(cookies, f, ensure_ascii=False, indent=2)

driver.quit()
