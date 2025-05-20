# -*- coding: utf-8 -*-
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.edge.service import Service
import time
import json

# 启动 Edge
service = Service("msedgedriver.exe")
driver = webdriver.Edge(service=service)

# 打开登录页面
driver.get("https://sendeltastudent.schoolis.cn")
time.sleep(2)

# 定位用户名输入框并输入
driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.loginName']").send_keys("dengchengxuan")

# 定位密码输入框并输入
driver.find_element(By.XPATH, "//input[@ng-model='$ctrl.passWord']").send_keys("dyjwo3-majjyg-Porris")

# 点击登录按钮
driver.find_element(By.XPATH, "//button[@ng-click='$ctrl.login()']").click()

time.sleep(5)

# 获取登录后的 cookies
cookies = driver.get_cookies()
print("Cookies:", cookies)

# 保存 cookies
with open("cookies.json", "w", encoding="utf-8") as f:
    json.dump(cookies, f, ensure_ascii=False, indent=2)

driver.quit()
